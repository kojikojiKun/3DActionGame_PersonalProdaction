using Unity.VisualScripting;
using UnityEditor.Recorder;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PoolManager m_poolManager;
    [SerializeField] PlayerController[] m_playerPrefab;
    [SerializeField] EnemySpawner m_enemySpawner;
    [SerializeField] PutTraps m_putTraps;
    private int m_playerIndex;
    [SerializeField] Transform m_playerSpawnPos;
    private PlayerController m_playerController;
    private string m_statusId;
    private float m_playerHP;
    private float m_playerAG;
    private float m_playerATK;
    private float m_playerAS;

    public PlayerController GetPlayer=>m_playerController;
    public PoolManager GetPoolManager => m_poolManager;
    public float getHp => m_playerHP;
    public float getAG => m_playerAG;
    public float getATK => m_playerATK;
    public float getAS => m_playerAS;

    void SetPlayerStatus()
    {
        m_statusId = PlayerPrefs.GetString("StatusID");
        m_playerHP = PlayerPrefs.GetFloat(m_statusId + "_HP");
        m_playerAG = PlayerPrefs.GetFloat(m_statusId + "_Speed");
        m_playerAS = PlayerPrefs.GetFloat(m_statusId + "_AS");
        m_playerATK = PlayerPrefs.GetFloat(m_statusId + "_AP");

        switch (m_statusId)
        {
            case "StandardType":
                m_playerIndex = 0;
                break;
            case "SpeedType":
                m_playerIndex = 1;
                break;
            case "PowerType":
                m_playerIndex = 2;
                break;
        }

        PlayerController player = Instantiate(m_playerPrefab[m_playerIndex], m_playerSpawnPos.position, Quaternion.identity);
        m_playerController = player;
    }

    //ウェーブ開始
    public void StartWave()
    {
        m_putTraps.ModeChange(false); //トラップ設置を不可能にする.
        m_enemySpawner.SetWaveContent();
    }

    public bool IsWaveFinished()
    {
        return m_enemySpawner.GetWaveFinished;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPlayerStatus();
        m_putTraps.ReceivePlayer(GetPlayer);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
