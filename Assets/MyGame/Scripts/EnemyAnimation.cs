using UnityEngine;
using System.Collections;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] private float attackCoolDown;
    [SerializeField] private int playDmgAnimCnt;
    private float damageCnt = 0;
    private Vector3 lastPos;
    private float moveSpeed;
    private bool isAttacked = true;

    private void FixedUpdate()
    {
        var currentPos = transform.position;
        var deltaPos = currentPos - lastPos;
        moveSpeed = deltaPos.magnitude / Time.fixedDeltaTime;
        lastPos = currentPos;
    }
    private void Update()
    {
        MoveAnim();
    }
    public void MoveAnim()
    {  
        animator.SetFloat("moveSpeed",moveSpeed);
    }

    public void AttackAnim()
    {
        if (isAttacked == true)
        {
            StartCoroutine(WaitAttack()); //コルーチンスタート.
        }
    }

    private IEnumerator WaitAttack()
    {
        isAttacked = false;
        yield return new WaitForSeconds(attackCoolDown); //攻撃待機.
        
        animator.SetTrigger("Attack");
        isAttacked = true;
    }

    public void DamageAnim()
    {
        damageCnt++;
        if (playDmgAnimCnt != 0 && damageCnt % playDmgAnimCnt == 0)
        {
            animator.SetTrigger("Damage");
        }
    }

    public void DeadAnim()
    {
        animator.SetBool("Dead",true);
        animator.SetTrigger("Die");
    }
}
