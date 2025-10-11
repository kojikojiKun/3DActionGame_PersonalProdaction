using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            EnemyControl enemy=other.GetComponent<EnemyControl>();
            enemy.TakeDamage(1);
        }
    }
}
