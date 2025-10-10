using UnityEngine;
using System.Collections;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] private float attackCoolDown;
    private bool isAttacked = true;
    public void AttackAnim()
    {
        Debug.Log("can attack");
        if (isAttacked == true)
        {
            StartCoroutine(WaitAttack());
        }
    }

    private IEnumerator WaitAttack()
    {
        isAttacked = false;
        Debug.Log("enemy is attacking");
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(attackCoolDown);
        isAttacked = true;
    }
}
