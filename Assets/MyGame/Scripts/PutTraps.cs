using UnityEngine;
using UnityEngine.InputSystem;

public class PutTraps : MonoBehaviour
{
    //InputSystems
    [SerializeField] InputActionReference clickAction;
    [SerializeField] InputActionReference scrollAction;
    [SerializeField] InputActionReference mousePosition;
    [SerializeField] InputActionReference buttonDown;
    [SerializeField] InputActionReference rotateTrap;

    [SerializeField] GameObject player;
    [SerializeField] ObjectPool[] trapPools; // トラップ用プール
    [SerializeField] GameObject[] previewTrapPrefab; //トラップの設置プレビュー用オブジェクト
    [SerializeField] Material prevMaterial;
    private GameObject[] prevInstance;
    private Collider[] prevCollider;
    private Transform lastPreviewPos; //プレビューの最終座標
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float maxPlaceDistance; //トラップを設置可能なプレイヤーとの距離
    [SerializeField] LayerMask fieldMask; //トラップ設置可能なレイヤー
    public bool waveFinished = true; //ウェーブ終了フラグ
    public bool buildMode = true; //トラップ設置モードフラグ
    private bool decided; //トラップの種類選択完了フラグ
    private bool canPlace; //トラップ設置可能フラグ
    private bool isRotating;
    private bool isCollision;
    private Vector2 rotateDir;
    private int trapIndex = 0;
    private float nextAllowTime = 0f;
    private float scrollCoolDown = 0.1f; //マウスミドルボタンのスクロールクールダウン

    private void OnEnable()
    {
        clickAction.action.Enable();
        scrollAction.action.Enable();
        mousePosition.action.Enable();
        buttonDown.action.Enable();
        rotateTrap.action.Enable();
        rotateTrap.action.performed += OnRotateTrap;
        rotateTrap.action.canceled += OnRotateTrap;
    }
    void OnDisable()
    {
        clickAction.action.Disable();
        scrollAction.action.Disable();
        mousePosition.action.Disable();
        buttonDown.action.Disable();
        rotateTrap.action.Disable();
        rotateTrap.action.performed -= OnRotateTrap;
        rotateTrap.action.canceled -= OnRotateTrap;
    }

    //マウスミドルボタンスクロールでトラップの種類選択
    public void OnSelectTrap(InputAction.CallbackContext context)
    {
        if (Time.time < nextAllowTime || decided == true) return;
        Vector2 scrollValue = context.ReadValue<Vector2>();
        SelectTrap(scrollValue);
    }

    //ミドルボタン押し込みでトラップの種類決定
    public void OnDecideTrap(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (decided == false)
            {
                decided = true; //トラップ選択完了
            }
            else
            {
                decided = false; //トラップ未選択
            }
            if (prevInstance[trapIndex] != null)
            {
                Debug.Log("prev is not null");
            }
            Debug.Log(trapIndex);
            if (decided == true)
            {
                prevInstance[trapIndex].SetActive(true);
                Debug.Log($"{prevInstance[trapIndex]}active");
            }
            else
            {
                prevInstance[trapIndex].SetActive(false);
                Debug.Log($"{prevInstance[trapIndex]}invilid");
            }
        }
    }

    public void OnRotateTrap(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            isRotating = true;
            rotateDir = context.ReadValue<Vector2>();
        }
        else
        {
            isRotating = false;
        }
    }

    void RotateTrap()
    {
        if (decided == true)
        {
            GameObject prev = prevInstance[trapIndex];
            if (rotateDir.x > 0.1f)
            {
                prev.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
                Debug.Log(prev.transform.localRotation);

            }
            else if (rotateDir.x < -0.1f)
            {
                prev.transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
                Debug.Log(prev.transform.localRotation);
            }
        }
    }

    //スクロールの値からトラップの種類選択
    void SelectTrap(Vector2 scrollValue)
    {
        if (scrollValue.y > 0.1f)
        {
            trapIndex++;
            nextAllowTime = Time.time + scrollCoolDown;
            if (trapIndex >= trapPools.Length)
            {
                trapIndex = 0;
            }
        }
        else if (scrollValue.y < -0.1f)
        {
            trapIndex--;
            nextAllowTime = Time.time + scrollCoolDown;
            if (trapIndex < 0)
            {
                trapIndex = trapPools.Length - 1;
            }
        }
    }

    //カーソル移動でトラップの設置位置を決める
    public void OnMoveTrap(InputAction.CallbackContext context)
    {
        Vector2 pos = Mouse.current.position.ReadValue(); //マウスカーソルの座標を取得
        Vector3 origin = player.transform.position; //プレイヤーの座標
        Ray ray = Camera.main.ScreenPointToRay(pos); //カメラからray発射
        if (Physics.Raycast(ray, out RaycastHit hit, 500, fieldMask)) //地面にrayヒット
        {
            Vector3 hitPos = hit.point; //マウスの座標を3Dに変換
            prevInstance[trapIndex].transform.position = hitPos; //プレビュー表示

            float distance = Vector3.Distance(origin, hitPos); //マウス位置とプレイヤーの距離計算
            //プレイヤーとの距離が設置可能距離以下
            if (distance <= maxPlaceDistance)
            {
                canPlace = true; //設置可能
            }
            else
            {
                canPlace = false; //設置不可
            }
            TryPlaceTrap(hitPos);
        }

    }


    //Eキーでトラップの設置位置を決定
    public void OnPlaceTrap(InputAction.CallbackContext context)
    {
        //トラップ選択済みかつ設置可能な場所ならトラップ設置
        if (decided == true && canPlace == true)
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
        isCollision = collision;
    }

    void TryPlaceTrap(Vector3 hitPos)
    {

        lastPreviewPos = prevInstance[trapIndex].transform;
        lastPreviewPos.localRotation = prevInstance[trapIndex].transform.localRotation;
        if (prevMaterial != null)
        {
            prevInstance[trapIndex].transform.position = hitPos;
            //Colorを設置可能なら緑、不可能なら赤に変更
            prevMaterial.color = canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
        }
        else
        {
            Debug.LogWarning("prevMaterial is null");
        }
    }

    //トラップを設置する
    void SucceedPlaceTrap()
    {
        GameObject trap = trapPools[trapIndex].GetObject(); //トラップをプールから取り出す
        prevInstance[trapIndex].SetActive(false); //プレビュー非表示
        trap.SetActive(true);
        trap.transform.rotation = lastPreviewPos.localRotation;
        trap.transform.position = lastPreviewPos.position; //トラップをプレビューの位置に設置
        decided = false;
    }

    private void Start()
    {
        prevInstance = new GameObject[trapPools.Length];
        prevCollider = new Collider[trapPools.Length];
        for (int i = 0; i < trapPools.Length; i++)
        {
            prevInstance[i] = Instantiate(previewTrapPrefab[i]);
            prevCollider[i] = prevInstance[i].GetComponent<Collider>();
            prevInstance[i].SetActive(false);
        }
    }

    private void Update()
    {
        //ウェーブ完了かつトラップ設置モード中
        if (waveFinished == true && buildMode == true)
        {
            OnEnable(); //InputSystem有効
        }
        else
        {
            OnDisable();
        }

        if (isRotating == true)
        {
            RotateTrap();
        }

        if (isCollision == true)
        {
            canPlace = false;
        }
    }
}
