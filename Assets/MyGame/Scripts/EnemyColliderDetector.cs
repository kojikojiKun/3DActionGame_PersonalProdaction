using UnityEngine;

public class EnemyColliderDetector : MonoBehaviour
{
    [SerializeField] EnemyControl m_control;
    [SerializeField] Collider m_attackRange;
    private GameObject enterObj;


    //�U���\�͈͓��̃I�u�W�F�N�g�𔻒�.
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enterObj = other.gameObject; //�U���͈͓��̃I�u�W�F�N�g�Q��.
            m_control.SetTarget(enterObj);
            m_control.Attack();
        }

        if (other.CompareTag("Trap"))
        {
            enterObj = other.gameObject; //�U���͈͓��̃I�u�W�F�N�g�Q��.
            m_control.SetTarget(enterObj);
            m_control.Attack();
        }

         if (other.CompareTag("Ruin"))
        {
            enterObj = other.gameObject; //�U���͈͓��̃I�u�W�F�N�g�Q��.
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
