using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PutTraps : MonoBehaviour
{
    //InputSystems
    [SerializeField] InputActionReference trapAction;

    private GameSceneManager m_gameSceneManager;
    private PoolManager m_poolManager;
    private PlayerController m_playerController;
    [SerializeField] GameObject[] m_previewTrapPrefab; //トラップの設置プレビュー用オブジェクト.
    [SerializeField] Material m_prevMaterial; //プレビューの色を変更するためのマテリアル.
    private GameObject[] m_prevInstance; //生成したプレビューオブジェクトを格納するための変数.
    private Transform m_lastPreviewPos; //プレビューの最終座標.

    private string[] m_trapName = new string[4] { "Fire", "CrossBow", "Blade", "Spike" }; //プールから取り出すトラップの名前.
    [SerializeField] private float m_rotateSpeed; //プレビューを回転させる速度.
    [SerializeField] private float m_maxPlaceDistance; //トラップを設置可能なプレイヤーとの距離.
    [SerializeField] LayerMask m_fieldMask; //トラップ設置可能なレイヤー.
    public bool m_buildMode = true; //トラップ設置モードフラグ.
    private bool m_decided; //トラップの種類選択完了フラグ.
    private bool m_inRange; //設置可能距離フラグ.
    private bool m_collision; //プレビューがほかのオブジェクトに触れているかのフラグ.
    private bool m_canPlace; //トラップ設置可能フラグ.
    private bool m_isRotating; //回転させるフラグ.
    private Vector2 m_rotateDir; //回転方向.
    private int m_trapIndex = 0; //選択中のトラップ.
    private float m_nextAllowTime = 0f;
    private float m_scrollCoolDown = 0.1f; //マウスミドルボタンのスクロールクールダウン.

    private void Start()
    {
        //必要な要素を参照.
        m_gameSceneManager = GameSceneManager.instance;
        m_poolManager = m_gameSceneManager.GetPoolManager;
        m_playerController = m_gameSceneManager.GetPlayer;
        m_prevInstance = new GameObject[m_previewTrapPrefab.Length];
        for (int i = 0; i < m_previewTrapPrefab.Length; i++)
        {
            GameObject prev = Instantiate(m_previewTrapPrefab[i]);
            m_prevInstance[i] = prev;
            m_prevInstance[i].SetActive(false);
        }

    }

    private void Update()
    {
        if (m_isRotating == true)
        {
            RotateTrap();
        }

        //プレビューが設置可能距離内かつほかのオブジェクトに触れていない.
        if (m_inRange == true && m_collision == false)
        {
            m_canPlace = true; //設置可能.
        }
        else if (m_inRange == false || m_collision == true)
        {
            m_canPlace = false; //設置不可.
        }
    }

    private void OnEnable()
    {
        //InputActionMap有効化.
        trapAction.action.actionMap.Enable();
    }
    private void OnDisable()
    {
        //InputActionMap無効化.
        trapAction.action.actionMap.Disable();
    }

    //マウスミドルボタンスクロールでトラップの種類選択.
    public void OnSelectTrap(InputAction.CallbackContext context)
    {
        //スクロール時間がクールダウン以内またはトラップの種類未選択.
        if (Time.time < m_nextAllowTime || m_decided == true) return;

        Vector2 scrollValue = context.ReadValue<Vector2>();
        SelectTrap(scrollValue);
    }

    //スクロールの値からトラップの種類選択.
    void SelectTrap(Vector2 scrollValue)
    {
        //上にスクロール.
        if (scrollValue.y > 0.1f)
        {
            m_trapIndex++;
            m_nextAllowTime = Time.time + m_scrollCoolDown;

            //m_trapIndexの範囲制限.
            if (m_trapIndex > m_prevInstance.Length - 1)
            {
                m_trapIndex = 0;
            }

            //現在のトラップの一つ前のプレビューを非表示.
            if (m_trapIndex == 0)
            {
                m_prevInstance[m_prevInstance.Length - 1].SetActive(false);
            }
            else
            {
                m_prevInstance[m_trapIndex - 1].SetActive(false);
            }

            m_prevInstance[m_trapIndex].SetActive(true); //現在のトラップのプレビューを表示.
        }

        //下にスクロール.
        else if (scrollValue.y < -0.1f)
        {
            m_trapIndex--;
            m_nextAllowTime = Time.time + m_scrollCoolDown;

            //m_trapIndexの範囲制限.
            if (m_trapIndex < 0)
            {
                m_trapIndex = m_prevInstance.Length - 1;
            }

            //現在のトラップの一つ前のプレビューを非表示.
            if (m_trapIndex == m_prevInstance.Length - 1)
            {
                m_prevInstance[0].SetActive(false);
            }
            else
            {
                m_prevInstance[m_trapIndex + 1].SetActive(false);
            }

            m_prevInstance[m_trapIndex].SetActive(true);//現在のトラップのプレビューを表示.

        }
    }

    //ミドルボタン押し込みでトラップの種類決定.
    public void OnDecideTrap(InputAction.CallbackContext context)
    {
        if (m_prevInstance == null || m_prevInstance.Length == 0) return;

        //ボタンが押された瞬間だけ実行
        if (context.phase == InputActionPhase.Started)
        {
            if (m_decided == false)
            {
                m_decided = true; //トラップ選択完了.
            }
            else
            {
                m_decided = false; //トラップ未選択.
            }

            m_prevInstance[m_trapIndex].SetActive(true); //プレビュー表示.
        }
    }

    //プレビューを回転させる
    public void OnRotateTrap(InputAction.CallbackContext context)
    {
        //ボタンを押し続けている間実行.
        if (context.started || context.performed)
        {
            m_isRotating = true;
            m_rotateDir = context.ReadValue<Vector2>();
        }
        else
        {
            m_isRotating = false;
        }
    }

    //カーソル移動でトラップの設置位置を決める.
    public void OnMoveTrap(InputAction.CallbackContext context)
    {
        if (m_prevInstance == null || m_prevInstance.Length == 0) { return; }

        Vector2 pos = Mouse.current.position.ReadValue(); //マウスカーソルの座標を取得.
        Vector3 origin = m_playerController.transform.position; //プレイヤーの座標.
        Ray ray = Camera.main.ScreenPointToRay(pos); //カメラからray発射.

        if (Physics.Raycast(ray, out RaycastHit hit, 500, m_fieldMask)) //地面にrayヒット.
        {
            Vector3 hitPos = hit.point; //マウスの座標を3Dに変換.
            float distance = Vector3.Distance(origin, hitPos); //マウス位置とプレイヤーの距離計算.
            m_prevInstance[m_trapIndex].transform.position = hitPos; //プレビュー移動.

            //プレイヤーとの距離が設置可能距離以下.
            if (distance <= m_maxPlaceDistance)
            {
                m_inRange = true; //設置可能.
            }
            else
            {
                m_inRange = false; //設置不可.
            }
            TryPlaceTrap(hitPos);
        }
    }

    //トラップが設置可能か判断.
    void TryPlaceTrap(Vector3 hitPos)
    {

        m_lastPreviewPos = m_prevInstance[m_trapIndex].transform; //プレビューの最終座標保存.
        m_lastPreviewPos.localRotation = m_prevInstance[m_trapIndex].transform.localRotation; //プレビューの回転量保存.

        //マテリアルのnullチェック.
        if (m_prevMaterial != null)
        {
            //Colorを設置可能なら緑、不可能なら赤に変更.
            m_prevMaterial.color = m_canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
        }
        else
        {
            Debug.LogWarning("prevMaterial is null");
        }
    }

    //Eキーでトラップの設置位置を決定.
    public void OnPlaceTrap(InputAction.CallbackContext context)
    {
        //トラップ選択済みかつ設置可能な場所ならトラップ設置.
        if (m_decided == true && m_canPlace == true)
        {
            if (context.phase == InputActionPhase.Started)
            {
                SucceedPlaceTrap();
            }
        }
    }

    //トラップを設置する
    void SucceedPlaceTrap()
    {
        GameObject trap = m_poolManager.GetTrap(m_trapName[m_trapIndex]); //トラップをプールから取り出す.
        m_prevInstance[m_trapIndex].SetActive(false); //プレビュー非表示.

        trap.transform.rotation = m_lastPreviewPos.localRotation; //プレビューの回転をトラップに反映.
        trap.transform.position = m_lastPreviewPos.transform.position; //トラップをプレビューの位置に設置.
        Debug.Log($"prevPosition={m_lastPreviewPos.transform.position},trapPosition={trap.transform.position}");
        m_decided = false; //トラップを未選択にする.
    }

    //プレビューを回転させる.
    void RotateTrap()
    {
        //トラップの種類を決定済み.
        if (m_decided == true)
        {
            GameObject prev = m_prevInstance[m_trapIndex];

            //右回転.
            if (m_rotateDir.x > 0.1f)
            {
                prev.transform.Rotate(0, m_rotateSpeed * Time.deltaTime, 0);
            }
            //左回転.
            else if (m_rotateDir.x < -0.1f)
            {
                prev.transform.Rotate(0, -m_rotateSpeed * Time.deltaTime, 0);
            }
        }
    }

    //プレビューがほかのオブジェクトと衝突していれば設置不可.
    public void TriggerCheckResult(bool collision)
    {
        m_collision = collision; //衝突していればtrue.
    }

    //トラップ設置モードを切り替え.
    public void ModeChange(bool canChange)
    {
        //切り替え可能.
        if (canChange == true)
        {
            m_buildMode = !m_buildMode; //モード切替.

            if (m_buildMode == true)
            {
                m_prevInstance[m_trapIndex].SetActive(true);
            }
            else
            {
                m_prevInstance[m_trapIndex].SetActive(false);
            }
        }
        else
        {
            m_buildMode = false;
            m_prevInstance[m_trapIndex].SetActive(false);
        }
    }
}
