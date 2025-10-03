using UnityEngine;

public class Inspection : MonoBehaviour
{
    [SerializeField]
    private Collider Boxcollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool tf = Boxcollider.enabled;
        //Debug.Log($"{gameObject.name} //{tf}");
    }
}
