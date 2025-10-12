using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Wave[] m_waves;
    [SerializeField] GameObject m_centerSpown;
    [SerializeField] private float m_radius;
    private ObjectPool[] m_bossPools;
    private ObjectPool[] m_enemyPools;
    private int m_waveIndex; //開始するウェーブの種類.
    private int m_enemyLimit; //敵の出現上限.
    private int m_enemyCount; //敵の出現回数
    private float m_interval; //敵の出現間隔.
    private float m_bossSpawnTime; //ボスが出現するまでの時間.
    private float timer; //経過時間を記録.
    private bool m_waveFinised; //ウェーブ終了フラグ.
    private bool m_isSpawned; //敵の出現フラグ.
    private bool m_isSpawnedBoss; //ボスの出現フラグ.

    void SetWaveContent()
    {
        m_bossPools=new ObjectPool[m_waves[m_waveIndex].bossPool.Length];
        m_enemyPools=new ObjectPool[m_waves[m_waveIndex].enemyPool.Length];
        //ボスのプールをセット.
        for (int i = 0; i < m_waves[m_waveIndex].bossPool.Length; i++)
        {
            m_bossPools[i] = m_waves[m_waveIndex].bossPool[i];
        }

        //ボス以外の敵のプールをセット.
        for (int i = 0; i < m_waves[m_waveIndex].enemyPool.Length; i++)
        {
            m_enemyPools[i] = m_waves[m_waveIndex].enemyPool[i];
        }

        m_enemyCount = 0;
        m_isSpawned = true;
        m_isSpawnedBoss = false;
        timer = 0;

        m_enemyLimit = m_waves[m_waveIndex].spownEnemyLimit;
        m_interval = m_waves[m_waveIndex].spownInterval;
        m_bossSpawnTime = m_waves[m_waveIndex].spownBossTime;
    }

    private IEnumerator Spawn()
    {
        //敵のスポーン数がm_enemylimit以下
        if (m_enemyCount < m_enemyLimit)
        { 
            m_isSpawned = false;

            //m_centerSpawnを中心とした半径m_radiusの円の円周上に敵をスポーン.
            float angle = Random.Range(0f, 360f);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * m_radius;
            Vector3 spawnPos = m_centerSpown.transform.position + offset;

            
            int random = Random.Range(0, m_enemyPools.Length - 1);//敵の種類をランダムに選択.
            GameObject enemy = m_enemyPools[random].GetObject(); //プールから取り出す.
            enemy.transform.position = spawnPos; //敵の位置を円周上に配置.
            m_enemyCount++;
            yield return new WaitForSeconds(m_interval); //m_interval秒待機

            m_isSpawned = true;
        }
        else
        {
            yield return null;
        }
    }

    public void BossSpawn()
    {
        foreach (var bossPool in m_bossPools)
        { //m_centerSpawnを中心とした半径m_radiusの円の円周上に敵をスポーン.
            float angle = Random.Range(0f, 360f);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * m_radius;
            Vector3 spawnPos = m_centerSpown.transform.position + offset;

            GameObject boss = bossPool.GetObject();
            boss.transform.position = spawnPos;
        }

        m_isSpawnedBoss = true;
    }

    public void StartWave()
    {
        m_waveFinised = false;
        SetWaveContent();
    }

    public void FinishWave()
    {
        m_waveFinised = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartWave();
    }

    // Update is called once per frame
    void Update()
    {
        if ((m_waveFinised == false))
        {
            if (m_isSpawned == true)
            {
                StartCoroutine(Spawn());
            }

            if (m_isSpawnedBoss == false)
            {
                timer += Time.deltaTime;
            }

            if (timer > m_bossSpawnTime && m_isSpawnedBoss == false)
            {
                BossSpawn();
            }
        }
    }
}
