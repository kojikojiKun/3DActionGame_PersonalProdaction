using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    private Vector3 targetDir;
   [SerializeField] private float moveSpeed;
    private float arrowDamage;

    public void SetDirection(Vector3 dir , float damage) //i‚Ş•ûŒü‚ğİ’è
    {
        targetDir = dir.normalized;
        arrowDamage = damage;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += targetDir * moveSpeed * Time.deltaTime;
    }
}
