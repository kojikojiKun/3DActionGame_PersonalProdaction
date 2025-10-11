using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Wave m_waves;
    [SerializeField] GameObject m_centerSpown;
    [SerializeField] private float m_radius;
    private ObjectPool[] m_bossPools;
    private ObjectPool[] m_enemyPools;
    private int m_enemyLimit;
    private float m_interval;
    private float m_bossSpownTime;
    private bool m_waveFinised;
    private bool m_isSpowned;

    void SetWaveContent()
    {

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
