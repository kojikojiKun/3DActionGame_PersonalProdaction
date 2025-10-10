using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] GameObject ruin;
    public GameObject OriginTarget => ruin;
    public PlayerController GetPlayer => player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
