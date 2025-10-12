using UnityEngine;

//enemy��attackCollider�ɃZ�b�g.
public class DamagePlayer : MonoBehaviour
{
    [SerializeField] EnemyControl control;
    private PlayerController player;
    private float m_power;

    private void Start()
    {
        player = control.GetPlayer;
        m_power = control.AP; //�U���͂�ǂݎ��.
    }

    private void OnTriggerEnter(Collider other)
    {    
        if (other.CompareTag("Player"))
        {
           // player.TakeDamage(m_power);
        }

        if (other.CompareTag("Trap"))
        {
            Debug.Log("attack to trap");
        }

        if (other.CompareTag("Ruin"))
        {
            Debug.Log("attack to ruin");
        }
    }
}
