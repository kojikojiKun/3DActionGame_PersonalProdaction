using UnityEditor.Recorder;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerController[] m_playerPrefab;
    [SerializeField] PutTraps m_puttraps;
    [SerializeField] EnemySpawner m_enemySpawner;
    private int m_playerIndex;
    [SerializeField] Transform m_playerSpawnPos;
    private string m_statusId;
    private float m_playerHP;
    private float m_playerAG;
    private float m_playerATK;
    private float m_playerAS;
    private bool m_waveFinished;

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPlayerStatus();
        PlayerController player = Instantiate(m_playerPrefab[m_playerIndex], m_playerSpawnPos.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
