using UnityEngine;

public class EnemyColliderDetector : MonoBehaviour
{
    [SerializeField] EnemyControl m_control;
    [SerializeField] Collider m_attackRange;
    private GameObject enterObj;


    //攻撃可能範囲内のオブジェクトを判定.
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enterObj = other.gameObject; //攻撃範囲内のオブジェクト参照.
            m_control.SetTarget(enterObj);
            m_control.Attack();
        }

        if (other.CompareTag("Trap"))
        {
            enterObj = other.gameObject; //攻撃範囲内のオブジェクト参照.
            m_control.SetTarget(enterObj);
            m_control.Attack();
        }

         if (other.CompareTag("Ruin"))
        {
            enterObj = other.gameObject; //攻撃範囲内のオブジェクト参照.
            m_control.SetTarget(enterObj);
            m_control.Attack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enterObj = null;
            m_control.SetTarget(enterObj);
        }
        if (other.CompareTag("Trap"))
        {
            enterObj = null;
            m_control.SetTarget(enterObj);
        }
        if (other.CompareTag("Ruin"))
        {
            enterObj = null;
            m_control.SetTarget(enterObj);
        }
    }
}
