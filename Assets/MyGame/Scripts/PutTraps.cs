using UnityEngine;

public class PutTraps : MonoBehaviour
{
    public ObjectPool blockPool;           // ブロック用プール
    public GameObject previewBlockPrefab;
    public LayerMask placeLayer;
    public LayerMask blockLayer;
    public float maxPlaceDistance = 5f;

    private GameObject previewInstance;

    void Start()
    {
        previewInstance = Instantiate(previewBlockPrefab);
        previewInstance.SetActive(false);
    }

    private float timer = 0;
    private bool put = false;
    void Update()
    {
        //UpdatePreview();
        timer += Time.deltaTime;
        if (timer > 5 && put == false)
        {
            TryPlaceBlock();
            put = true;
        }
      //  else if (Input.GetMouseButtonDown(1))
       // {
       //     TryDestroyBlock();
       // }
    }

    /*void UpdatePreview()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placeLayer))
        {
            Vector3 placePos = GetGridPosition(hit.point);
            float distance = Vector3.Distance(transform.position, placePos);

            if (distance <= maxPlaceDistance)
            {
                previewInstance.SetActive(true);
                previewInstance.transform.position = placePos;
            }
            else
            {
                previewInstance.SetActive(false);
            }
        }
        else
        {
            previewInstance.SetActive(false);
        }
    }*/

    void TryPlaceBlock()
    {
        if (!previewInstance.activeSelf) return;

        Vector3 placePos = previewInstance.transform.position;
        GameObject block = blockPool.GetObject();
        block.transform.position = placePos;
        block.transform.rotation = Quaternion.identity;

        // ブロックに自分のプールを伝えておく
        var pooled = block.GetComponent<PooledObject>();
        if (pooled != null) pooled.SetPool(blockPool);
    }

    void TryDestroyBlock()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, blockLayer))
        {
            var pooled = hit.collider.GetComponent<PooledObject>();
            if (pooled != null)
            {
                pooled.ReturnToPool();
            }
            else
            {
                Destroy(hit.collider.gameObject); // 念のため
            }
        }
    }

    Vector3 GetGridPosition(Vector3 rawPos)
    {
        return new Vector3(
            Mathf.Round(rawPos.x),
            Mathf.Round(rawPos.y),
            Mathf.Round(rawPos.z)
        );
    }
}
