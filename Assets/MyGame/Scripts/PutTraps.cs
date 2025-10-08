
using UnityEngine;
using UnityEngine.InputSystem;

public class PutTraps : MonoBehaviour
{
    //InputSystems
    [SerializeField] InputActionReference clickAction;
    [SerializeField] InputActionReference scrollAction;
    [SerializeField] InputActionReference mousePosition;
    [SerializeField] InputActionReference buttonDown;

    [SerializeField] ObjectPool[] trapPools; // トラップ用プール
    [SerializeField] GameObject[] previewTrapPrefab; //トラップの設置プレビュー用オブジェクト
    private Transform lastPreviewPos; //プレビューの最終座標
    private Renderer[] previewRenderer;
    [SerializeField] private float maxPlaceDistance; //トラップを設置可能なプレイヤーとの距離
    [SerializeField] LayerMask fieldMask; //トラップ設置可能なレイヤー
    public bool waveFinished; //ウェーブ終了フラグ
    public bool buildMode=false; //トラップ設置モードフラグ
    private bool decided = false; //トラップの種類選択完了フラグ
    private bool canPlace; //トラップ設置可能フラグ
    private int trapIndex = 0;
    private float nextAllowTime = 0f;
    private float scrollCoolDown = 0.1f; //マウスミドルボタンのスクロールクールダウン

    private void OnEnable()
    {
        clickAction.action.Enable();
        scrollAction.action.Enable();
        mousePosition.action.Enable();
        buttonDown.action.Enable();

    }
    void OnDisable()
    {
        clickAction.action.Disable();
        scrollAction.action.Disable();
        mousePosition.action.Disable();
        buttonDown.action.Enable();
    }

    //マウスミドルボタンスクロールでトラップの種類選択
    public void OnScroll(InputAction.CallbackContext context)
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

            if (decided == true)
            {
                previewTrapPrefab[trapIndex].SetActive(true);
               Debug.Log($"{trapIndex}oooooooooooo");
            }
            else
            {
                previewTrapPrefab[trapIndex].SetActive(false);
                Debug.Log($"{trapIndex}iiiiiiiiiiiiiiiii");
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
                trapIndex = trapPools.Length-1;
            }
        }  
    }

    //カーソル移動でトラップの設置位置を決める
    public void MoveTrap(InputAction.CallbackContext context)
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        Vector3 origin = transform.position;
        Ray ray = Camera.main.ScreenPointToRay(pos); //カメラからray発射
        if (Physics.Raycast(ray, out RaycastHit hit,500, fieldMask)) //地面にrayヒット
        {
            Vector3 hitPos = hit.point; //マウスの座標を3Dに変換
            previewTrapPrefab[trapIndex].transform.position = hitPos; //プレビュー表示

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
    public void PlaceTrap(InputAction.CallbackContext context)
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

    void TryPlaceTrap(Vector3 hitPos)
    {
        GameObject preview = previewTrapPrefab[trapIndex];
        Renderer renderer = preview.GetComponent<Renderer>();
        lastPreviewPos = preview.transform;
        if (renderer == null)
        {
            preview.transform.position = hitPos;
            
            renderer.material.color = canPlace ? Color.green : Color.red; //Colorを設置可能なら緑、不可能なら赤に変更
        }
    }

    //トラップを設置する
    void SucceedPlaceTrap()
    {
        GameObject trap = trapPools[trapIndex].GetObject(); //トラップをプールから取り出す
        trap.transform.position=lastPreviewPos.position; //トラップをプレビューの位置に設置
        decided = false;
    }

    private void Start()
    {
        previewRenderer = new Renderer[trapPools.Length];
        for (int i = 0; i < trapPools.Length; i++)
        {
            previewRenderer[i] = previewTrapPrefab[i].GetComponent<Renderer>();
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
    }
}
