using UnityEngine;
using System.Collections.Generic;

public class SpikeWallControl : MonoBehaviour
{
    public static List<SpikeWallControl> allSpikeTraps = new List<SpikeWallControl>();
    [SerializeField] TrapStaus trapStaus;
    [SerializeField] TrapType type;
    [Header("status")]
    private float spikeDurability; //スパイクトラップの耐久値
    private float spikeDamage; //スパイクトラップのダメージ量

    private void Awake()
    {
        allSpikeTraps.Add(this);
    }

    private void OnDestroy()
    {
        allSpikeTraps.Remove(this);
    }

    public void SetSpikeWallStasus(float durability, float damage)
    {
        spikeDurability = durability;
        spikeDamage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyStatus enemyStatus = other.GetComponent<EnemyStatus>();
            enemyStatus.TakeDamageEnemy(spikeDamage);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
