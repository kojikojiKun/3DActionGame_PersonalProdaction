using UnityEngine;

public class PrevTriggerCheck : MonoBehaviour
{
    [SerializeField] PutTraps putTraps;
    [SerializeField] Collider prevCol;
    private bool collision;
    private void OnTriggerStay(Collider other)
    {
        if (other.layerOverridePriority != LayerMask.NameToLayer("Field"))
        {
            collision = true;
            putTraps.TriggerCheckResult(collision);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.layerOverridePriority != LayerMask.NameToLayer("Field"))
        {
            collision = false;
            putTraps.TriggerCheckResult(collision);
        }
    }
}
