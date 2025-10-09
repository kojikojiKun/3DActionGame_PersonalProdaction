using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] private float initialSize;
    private Queue<GameObject> pool = new Queue<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);

        // ƒv[ƒ‹î•ñ‚ð“o˜^i–ß‚é‚½‚ßj
        var pooled = obj.GetComponent<PooledObject>();
        if (pooled != null)
        {
            pooled.SetPool(this);
        }

        pool.Enqueue(obj);
        Debug.Log("create");
        return obj;
    }

    public GameObject GetObject()
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : CreateNewObject();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
