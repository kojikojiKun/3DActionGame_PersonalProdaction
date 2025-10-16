using UnityEngine;

public class PrevTriggerCheck : MonoBehaviour
{
    private PutTraps m_putTraps;
    [SerializeField] Collider m_prevCol;
    private bool m_collision;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Field"))
        {
            m_collision = true;
            m_putTraps.TriggerCheckResult(m_collision);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Field"))
        {
            m_collision = false;
            m_putTraps.TriggerCheckResult(m_collision);
        }
    }

    private void Start()
    {
        GameObject player=GameSceneManager.instance.GetPlayer.gameObject;
        m_putTraps = player.GetComponent<PutTraps>();
    }
}
