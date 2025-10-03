using UnityEngine;

public class EnemyAttackDetector : MonoBehaviour
{
    private Collider attackRange;
    private EnemyStatus enemyStatus;

    private bool canAttack = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackRange = GetComponent<Collider>();
        enemyStatus = GetComponentInParent<EnemyStatus>();
    }

    //UŒ‚”ÍˆÍ“à‚ÉƒvƒŒƒCƒ„[‚ª‚¢‚ê‚ÎUŒ‚ŠJn
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = true; //UŒ‚‚Å‚«‚é           
            enemyStatus.AttackToPlayer(canAttack);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = false; //UŒ‚‚Å‚«‚È‚¢
            enemyStatus.AttackToPlayer(canAttack);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
