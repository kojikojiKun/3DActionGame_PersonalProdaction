using UnityEngine;

//enemy‚ÌattackCollider‚ÉƒZƒbƒg.
public class DamagePlayer : MonoBehaviour
{
    [SerializeField] EnemyControl control;
    private PlayerController player;
    private float m_power;

    private void Start()
    {
        player = control.GetPlayer;
        m_power = control.AP; //UŒ‚—Í‚ğ“Ç‚İæ‚è.
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
