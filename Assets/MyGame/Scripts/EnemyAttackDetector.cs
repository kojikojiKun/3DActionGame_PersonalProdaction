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

    //�U���͈͓��Ƀv���C���[������΍U���J�n
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = true; //�U���ł���           
            enemyStatus.AttackToPlayer(canAttack);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canAttack = false; //�U���ł��Ȃ�
            enemyStatus.AttackToPlayer(canAttack);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
