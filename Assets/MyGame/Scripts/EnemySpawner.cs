using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject m_originTarget;
    [SerializeField] GameSceneManager m_gameSceneManager;
    [SerializeField] Wave[] m_waves;
    [SerializeField] GameObject m_centerSpown;
    [SerializeField] private float m_radius; //�G�̏o���͈͂̒��S����̋���
    private PoolManager m_poolManager;
    private string[] m_findBossName; //�T������{�X�̖��O
    private string[] m_findEnemyName; //�T������{�X�ȊO�̓G�̖��O
    private GameObject[] m_bosses;
    private GameObject[] m_enemies;
    //private ObjectPool[] m_bossPools;
    //private ObjectPool[] m_enemyPools;
    private int m_waveIndex; //�J�n����E�F�[�u�̎��.
    private int m_enemyLimit; //�G�̏o�����.
    private int m_enemyCount; //�G�̏o����.
    private int m_aliveEnemyCnt; //�������Ă���G�̐�.
    private float m_interval; //�G�̏o���Ԋu.
    private float m_bossSpawnTime; //�{�X���o������܂ł̎���.
    private float timer; //�o�ߎ��Ԃ��L�^.
    private bool m_waveFinised; //�E�F�[�u�I���t���O. 
    private bool m_isSpawned; //�G�̏o���t���O.
    private bool m_isSpawnedBoss; //�{�X�̏o���t���O.

    public bool GetWaveFinished => m_waveFinised;

    public GameObject GetOriginTarget=>m_originTarget;
    public PlayerController GetPlayerController => m_gameSceneManager.GetPlayer;

    public void SetWaveContent()
    {
        m_findBossName = new string[m_waves[m_waveIndex].bossName.Length];
        m_findEnemyName = new string[m_waves[m_waveIndex].enemyName.Length];

        //�T������{�X�̖��O�Z�b�g.
        for (int i = 0; i < m_waves[m_waveIndex].bossName.Length; i++)
        {
            m_findBossName[i] = m_waves[m_waveIndex].bossName[i];
        }

        //�T������{�X�ȊO�̓G�̖��O�Z�b�g.
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
        //�G�̃X�|�[������m_enemylimit�ȉ�
        if (m_enemyCount < m_enemyLimit)
        { 
            m_isSpawned = false;

            //m_centerSpawn�𒆐S�Ƃ������am_radius�̉~�̉~����ɓG���X�|�[��.
            float angle = Random.Range(0f, 360f);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * m_radius;
            Vector3 spawnPos = m_centerSpown.transform.position + offset;

            
            int random = Random.Range(0, m_findEnemyName.Length);//�G�̎�ނ������_���ɑI��.
            GameObject enemy=m_poolManager.GetEnemy(m_findEnemyName[random]); //�v�[��������o��
            enemy.transform.position = spawnPos;
            m_enemyCount++;
            m_aliveEnemyCnt++;
            yield return new WaitForSeconds(m_interval); //m_interval�b�ҋ@

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
        { //m_centerSpawn�𒆐S�Ƃ������am_radius�̉~�̉~����ɓG���X�|�[��.
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
