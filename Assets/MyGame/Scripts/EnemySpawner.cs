using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject m_originTarget;
    [SerializeField] GameSceneManager m_gameSceneManager;
    [SerializeField] Wave[] m_waves;
    [SerializeField] GameObject m_centerSpown;
    [SerializeField] private float m_radius; //敵の出現範囲の中心からの距離
    private PoolManager m_poolManager;
    private string[] m_findBossName; //探索するボスの名前
    private string[] m_findEnemyName; //探索するボス以外の敵の名前
    private GameObject[] m_bosses;
    private GameObject[] m_enemies;
    //private ObjectPool[] m_bossPools;
    //private ObjectPool[] m_enemyPools;
    private int m_waveIndex; //開始するウェーブの種類.
    private int m_enemyLimit; //敵の出現上限.
    private int m_enemyCount; //敵の出現回数.
    private int m_aliveEnemyCnt; //生存している敵の数.
    private float m_interval; //敵の出現間隔.
    private float m_bossSpawnTime; //ボスが出現するまでの時間.
    private float timer; //経過時間を記録.
    private bool m_waveFinised; //ウェーブ終了フラグ. 
    private bool m_isSpawned; //敵の出現フラグ.
    private bool m_isSpawnedBoss; //ボスの出現フラグ.

    public bool GetWaveFinished => m_waveFinised;

    public GameObject GetOriginTarget=>m_originTarget;
    public PlayerController GetPlayerController => m_gameSceneManager.GetPlayer;

    public void SetWaveContent()
    {
        m_findBossName = new string[m_waves[m_waveIndex].bossName.Length];
        m_findEnemyName = new string[m_waves[m_waveIndex].enemyName.Length];

        //探索するボスの名前セット.
        for (int i = 0; i < m_waves[m_waveIndex].bossName.Length; i++)
        {
            m_findBossName[i] = m_waves[m_waveIndex].bossName[i];
        }

        //探索するボス以外の敵の名前セット.
        for (int i = 0; i < m_waves[m_waveIndex].enemyName.Length; i++)
        {
            m_findEnemyName[i] = m_waves[m_waveIndex].enemyName[i];
        }

        m_enemyLimit = m_waves[m_waveIndex].spownEnemyLimit;
        m_interval = m_waves[m_waveIndex].spownInterval;
        m_bossSpawnTime = m_waves[m_waveIndex].spownBossTime;

        m_enemyCount = 0;
        m_isSpawned = true;
        m_isSpawnedBoss = false;
        m_waveFinised = false;
        timer = 0;
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

            
            int random = Random.Range(0, m_findEnemyName.Length);//敵の種類をランダムに選択.
            GameObject enemy=m_poolManager.GetEnemy(m_findEnemyName[random]); //プールから取り出す
            enemy.transform.position = spawnPos;
            m_enemyCount++;
            m_aliveEnemyCnt++;
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
        foreach (var bossName in m_findBossName)
        { //m_centerSpawnを中心とした半径m_radiusの円の円周上に敵をスポーン.
            float angle = Random.Range(0f, 360f);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * m_radius;
            Vector3 spawnPos = m_centerSpown.transform.position + offset;

            GameObject boss = m_poolManager.GetEnemy(bossName);
            boss.transform.position = spawnPos;

            m_aliveEnemyCnt++;
        }

        m_isSpawnedBoss = true;
    }
    public void EnemyKilled()
    {
        m_aliveEnemyCnt--;

        if (m_aliveEnemyCnt <= 0)
        {
            m_waveFinised = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_poolManager = m_gameSceneManager.GetPoolManager;
        m_waveFinised=true;
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
                //BossSpawn();
            }
        }
    }
}
