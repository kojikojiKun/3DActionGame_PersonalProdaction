using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PutTraps : MonoBehaviour
{
    //InputSystems
    [SerializeField] InputActionReference trapAction;
    //[SerializeField] InputActionReference rotateTrap;

    [SerializeField] GameManager m_gameManager;
    [SerializeField] PoolManager m_poolManager;
    private PlayerController m_playerController;
    private string[] m_trapName = new string[4] { "Fire", "CrossBow", "Blade", "Spike" };
    [SerializeField] GameObject[] m_previewTrapPrefab; //トラップの設置プレビュー用オブジェクト
    [SerializeField] Material m_prevMaterial;
    private GameObject[] m_prevInstance;
    private Collider[] m_prevCollider;
    private Transform m_lastPreviewPos; //プレビューの最終座標
    [SerializeField] private float m_rotateSpeed;
    [SerializeField] private float m_maxPlaceDistance; //トラップを設置可能なプレイヤーとの距離
    [SerializeField] LayerMask m_fieldMask; //トラップ設置可能なレイヤー
    public bool m_buildMode = true; //トラップ設置モードフラグ
    private bool m_decided; //トラップの種類選択完了フラグ
    private bool m_canPlace; //トラップ設置可能フラグ
    private bool m_isRotating;
    private bool m_isCollision;
    private Vector2 m_rotateDir;
    private int m_trapIndex = 0;
    private float m_nextAllowTime = 0f;
    private float m_scrollCoolDown = 0.1f; //マウスミドルボタンのスクロールクールダウン
    private bool m_isEnabled;
    private bool m_isDisabled;

    private void OnEnable()
    {
        trapAction.action.actionMap.Enable();
    }
    private void OnDisable()
    {
        trapAction.action.actionMap.Disable();
    }

    //マウスミドルボタンスクロールでトラップの種類選択
    public void OnSelectTrap(InputAction.CallbackContext context)
    {
        if (Time.time < m_nextAllowTime || m_decided == true) return;
        Vector2 scrollValue = context.ReadValue<Vector2>();
        SelectTrap(scrollValue);
    }

    //ミドルボタン押し込みでトラップの種類決定
    public void OnDecideTrap(InputAction.CallbackContext context)
    {
        if (m_prevInstance == null || m_prevInstance.Length == 0) { return; }
            
        if (context.phase == InputActionPhase.Started)
        {
            if (m_decided == false)
            {
                m_decided = true; //トラップ選択完了
            }
            else
            {
                m_decided = false; //トラップ未選択
            }

            if (m_prevInstance[m_trapIndex] != null)
            {
                Debug.Log("prev is not null");
            }

            m_prevInstance[m_trapIndex].SetActive(true);
            Debug.Log(m_decided);
        }
    }

    public void OnRotateTrap(InputAction.CallbackContext context)
    {
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

    void RotateTrap()
    {
        if (m_decided == true)
        {
            GameObject prev = m_prevInstance[m_trapIndex];
            if (m_rotateDir.x > 0.1f)
            {
                prev.transform.Rotate(0, m_rotateSpeed * Time.deltaTime, 0);
                Debug.Log(prev.transform.localRotation);

            }
            else if (m_rotateDir.x < -0.1f)
            {
                prev.transform.Rotate(0, -m_rotateSpeed * Time.deltaTime, 0);
                Debug.Log(prev.transform.localRotation);
            }
        }
    }

    //スクロールの値からトラップの種類選択
    void SelectTrap(Vector2 scrollValue)
    {
        if (scrollValue.y > 0.1f)
        {
            m_trapIndex++;
            Debug.Log(m_trapIndex);
            m_nextAllowTime = Time.time + m_scrollCoolDown;
            if (m_trapIndex > m_prevInstance.Length-1)
            {
                m_trapIndex = 0;
            }

            //現在のトラップの一つ前のプレビューを非表示
            if (m_trapIndex == 0)
            {
                m_prevInstance[m_prevInstance.Length - 1].SetActive(false);
            }
            else
            {
                m_prevInstance[m_trapIndex - 1].SetActive(false);
            }

            m_prevInstance[m_trapIndex].SetActive(true); //現在のトラップのプレビューを表示
        }
        else if (scrollValue.y < -0.1f)
        {
            m_trapIndex--;
            Debug.Log(m_trapIndex);
            m_nextAllowTime = Time.time + m_scrollCoolDown;
            if (m_trapIndex < 0)
            {
                m_trapIndex = m_prevInstance.Length - 1;
            }

            //現在のトラップの一つ前のプレビューを非表示
            if (m_trapIndex == m_prevInstance.Length - 1)
            {
                m_prevInstance[0].SetActive(false);
            }
            else
            {
                m_prevInstance[m_trapIndex + 1].SetActive(false);
            }

            m_prevInstance[m_trapIndex].SetActive(true);//現在のトラップのプレビューを表示

        }
    }

    //カーソル移動でトラップの設置位置を決める
    public void OnMoveTrap(InputAction.CallbackContext context)
    {
        if (m_prevInstance == null || m_prevInstance.Length == 0) {  return; }
        Vector2 pos = Mouse.current.position.ReadValue(); //マウスカーソルの座標を取得
        Vector3 origin = m_playerController.transform.position; //プレイヤーの座標
        Ray ray = Camera.main.ScreenPointToRay(pos); //カメラからray発射
        if (Physics.Raycast(ray, out RaycastHit hit, 500, m_fieldMask)) //地面にrayヒット
        {
            Vector3 hitPos = hit.point; //マウスの座標を3Dに変換

            m_prevInstance[m_trapIndex].transform.position = hitPos; //プレビュー表示

            float distance = Vector3.Distance(origin, hitPos); //マウス位置とプレイヤーの距離計算
            //プレイヤーとの距離が設置可能距離以下
            if (distance <= m_maxPlaceDistance)
            {
                m_canPlace = true; //設置可能
            }
            else
            {
                m_canPlace = false; //設置不可
            }
            TryPlaceTrap(hitPos);
        }

    }


    //Eキーでトラップの設置位置を決定
    public void OnPlaceTrap(InputAction.CallbackContext context)
    {
        //トラップ選択済みかつ設置可能な場所ならトラップ設置
        if (m_decided == true && m_canPlace == true)
        {
            if (context.phase == InputActionPhase.Started)
            {
                SucceedPlaceTrap();
            }
        }
    }

    //プレビューがほかのオブジェクトと衝突していれば設置負荷
    public void TriggerCheckResult(bool collision)
    {
        m_isCollision = collision;
    }

    void TryPlaceTrap(Vector3 hitPos)
    {

        m_lastPreviewPos = m_prevInstance[m_trapIndex].transform;
        m_lastPreviewPos.localRotation = m_prevInstance[m_trapIndex].transform.localRotation;
        if (m_prevMaterial != null)
        {
            m_prevInstance[m_trapIndex].transform.position = hitPos;
            //Colorを設置可能なら緑、不可能なら赤に変更
            m_prevMaterial.color = m_canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
        }
        else
        {
            Debug.LogWarning("prevMaterial is null");
        }
    }

    //トラップを設置する
    void SucceedPlaceTrap()
    {
        GameObject trap = m_poolManager.GetTrap(m_trapName[m_trapIndex]); //トラップをプールから取り出す
        m_prevInstance[m_trapIndex].SetActive(false); //プレビュー非表示
        trap.SetActive(true);
        trap.transform.rotation = m_lastPreviewPos.localRotation;
        trap.transform.position = m_lastPreviewPos.position; //トラップをプレビューの位置に設置
        m_decided = false;
    }

    //トラップ設置モードを切り替え
    public void ModeChange(bool canChange)
    {
        Debug.Log($"length={m_prevInstance?.Length}");
        
        if (canChange == true)
        {
            m_buildMode = !m_buildMode;

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
        Debug.Log(m_buildMode);
    }

    public void ReceivePlayer(PlayerController player)
    {if (player == null)
        {
            Debug.Log("null!!!");
        }
        else
        {
            Debug.Log("not null!!!");
        }
        m_playerController = player;
        Debug.Log("setPlayer");
    }

    private void Start()
    {
        m_prevInstance = new GameObject[m_previewTrapPrefab.Length];
        m_prevCollider = new Collider[m_previewTrapPrefab.Length];
        for (int i = 0; i < m_previewTrapPrefab.Length; i++)
        {
            GameObject prev=Instantiate(m_previewTrapPrefab[i]);
            m_prevInstance[i] = prev;
            m_prevCollider[i] = m_prevInstance[i].GetComponent<Collider>();
            m_prevInstance[i].SetActive(false);
        }
        
    }

    private void Update()
    {Debug.Log($"trapIndex{m_trapIndex}");
        Debug.Log(m_gameManager.IsWaveFinished());
        
        if (m_isRotating == true)
        {
            RotateTrap();
        }

        if (m_isCollision == true)
        {
            m_canPlace = false;
        }
    }
}
