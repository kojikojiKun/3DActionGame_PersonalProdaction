using UnityEngine;
using System.Collections;
using System;

public class EnemySpowner : MonoBehaviour
{
    public static EnemySpowner instance;
    public GameObject player;

    [SerializeField]
    private Wave[] wave;
    private int waveId = -1;
    private GameObject boss;
    private GameObject[] enemies;

    private int spownCount=0;
    private int spownEnemyLimit; //�G�����̏��
    private float spownInterval; //�G�𐶐�����Ԋu
    private bool spowned=true; //�G�̐����^�C�~���O�����̂��߂̃t���O
    public bool bossSpowned = false;
    public bool waveStart = false;
    private float spownBossTime; //�E�F�[�u���n�܂��ă{�X���o������܂ł̎���
    private float sTime;

    public Transform bossSpown;
    public Transform centerSpown;
    public float spownDistance;
    
    //�E�F�[�u�̏���ǂݍ���
    public void WaveSetting()
    {
        if (waveStart == true)
        {
            sTime = Time.time; //�E�F�[�u�J�n���̎��Ԃ��L�^

            //�E�F�[�u�̏���ǂݍ���
            if (waveId < wave.Length)
            {
                spownCount = 0;
                bossSpowned = false;
                waveId++;
                boss = wave[waveId].boss;
                enemies = wave[waveId].enemies;
                spownEnemyLimit = wave[waveId].spownEnemyLimit;
                spownInterval = wave[waveId].spownInterval;
                spownBossTime = wave[waveId].spownBossTime;
            }
        }
    }

    public void StartWave()
    {
        waveStart = true; //�E�F�[�u�J�n
        WaveSetting();
    }

    public void FinishWave()
    {
        waveStart = false; //�E�F�[�u�I��
    }

    
    private IEnumerator SpownEnemy()
    {
        if (spownCount >= spownEnemyLimit || waveStart==false) yield break;

        if (spowned == true && bossSpowned == false)
        {
            spowned = false;
          
            float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);//�����_���Ȋp�x

            //centerPosition���S�̉~����̃����_���Ȉʒu�ɃX�|�[���ʒu��ݒ�
            Vector3 spownPos = new Vector3(Mathf.Cos(angle),
                0,
                Mathf.Sin(angle)) * spownDistance;
           
            spownPos += centerSpown.position;//���S�ʒu�ɉ��Z
          
            yield return new WaitForSeconds(spownInterval);

            //�G�𐶐�
            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    Instantiate(enemy, spownPos, Quaternion.identity);
                    spownCount++;
                }
            }
            spowned = true;
        }
    }

    void BossSpown()
    {
        if (waveStart == false) return; //�E�F�[�u���J�n����Ă��Ȃ��ȏ������Ȃ� 
        if (bossSpowned == true) return;//�{�X���o���ς݂Ȃ珈�����Ȃ�

        float currentTime = Time.time;
        //��莞�Ԍo�߂Ń{�X�o��
        if (currentTime - sTime >= spownBossTime)
        {
            Vector3 bossSpownPos = bossSpown.position; //�X�|�[���ʒu�ݒ�
            Instantiate(boss, bossSpownPos, Quaternion.identity); //�{�X�𐶐�
            bossSpowned = true;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");�@//�G�̖ڕW�n�_�ɂȂ�v���C���[��ݒ�
        }
        else
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaveSetting(); //�E�F�[�u���e��ǂݍ���
    }

    // Update is called once per frame
    void Update()
    {
        //�G�o��
        StartCoroutine(SpownEnemy());
        BossSpown();
    }
}
