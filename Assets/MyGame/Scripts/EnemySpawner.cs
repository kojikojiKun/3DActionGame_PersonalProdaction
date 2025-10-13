using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject m_originTarget;
    [SerializeField] GameManager m_gameManager;
    [SerializeField] Wave[] m_waves;
    [SerializeField] GameObject m_centerSpown;
    [SerializeField] private float m_radius;
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
    public PlayerController GetPlayerController => m_gameManager.GetPlayer;

    public void SetWaveContent()
    {
        m_bossPools = new ObjectPool[m_waves[m_waveIndex].bossPool.Length];
        m_enemyPools = new ObjectPool[m_waves[m_waveIndex].enemyPool.Length];
        //�{�X�̃v�[�����Z�b�g.
        for (int i = 0; i < m_waves[m_waveIndex].bossPool.Length; i++)
        {
            m_bossPools[i] = m_waves[m_waveIndex].bossPool[i];
        }

        //�{�X�ȊO�̓G�̃v�[�����Z�b�g.
        for (int i = 0; i < m_waves[m_waveIndex].enemyPool.Length; i++)
        {
            m_enemyPools[i] = m_waves[m_waveIndex].enemyPool[i];
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

            
            int random = Random.Range(0, m_enemyPools.Length - 1);//�G�̎�ނ������_���ɑI��.
            GameObject enemy = m_enemyPools[random].GetObject(); //�v�[��������o��.
            enemy.transform.position = spawnPos; //�G�̈ʒu���~����ɔz�u.
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
        foreach (var bossPool in m_bossPools)
        { //m_centerSpawn�𒆐S�Ƃ������am_radius�̉~�̉~����ɓG���X�|�[��.
            float angle = Random.Range(0f, 360f);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * m_radius;
            Vector3 spawnPos = m_centerSpown.transform.position + offset;

            GameObject boss = bossPool.GetObject();
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
