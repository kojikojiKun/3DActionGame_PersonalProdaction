using UnityEngine;

public class ArrowMove : MonoBehaviour
{
    private Vector3 targetDir;
   [SerializeField] private float moveSpeed;
    [SerializeField] private float eraseTime;
    private float passedTime;
    private float arrowDamage;

    public void SetDirection(Vector3 dir , float damage) //i‚Ş•ûŒü‚ğİ’è
    {
        targetDir = dir;
        arrowDamage = damage;
        
    }

    private void Start()
    {
        passedTime = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyStatus enemyStatus = other.GetComponent<EnemyStatus>();
            enemyStatus.TakeDamageEnemy(arrowDamage);
            Destroy(gameObject);
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Field"))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        passedTime += Time.deltaTime;
        transform.position += targetDir * moveSpeed * Time.deltaTime;
        if (passedTime >= eraseTime)
        {
            Destroy(gameObject);
        }
    }
}
