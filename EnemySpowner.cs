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
    private int spownEnemyLimit; //敵生成の上限
    private float spownInterval; //敵を生成する間隔
    private bool spowned=true; //敵の生成タイミング調整のためのフラグ
    public bool bossSpowned = false;
    public bool waveStart = false;
    private float spownBossTime; //ウェーブが始まってボスが出現するまでの時間
    private float sTime;

    public Transform bossSpown;
    public Transform centerSpown;
    public float spownDistance;
    
    //ウェーブの情報を読み込む
    public void WaveSetting()
    {
        if (waveStart == true)
        {
            sTime = Time.time; //ウェーブ開始時の時間を記録

            //ウェーブの情報を読み込む
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
        waveStart = true; //ウェーブ開始
        WaveSetting();
    }

    public void FinishWave()
    {
        waveStart = false; //ウェーブ終了
    }

    
    private IEnumerator SpownEnemy()
    {
        if (spownCount >= spownEnemyLimit || waveStart==false) yield break;

        if (spowned == true && bossSpowned == false)
        {
            spowned = false;
          
            float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);//ランダムな角度

            //centerPosition中心の円周上のランダムな位置にスポーン位置を設定
            Vector3 spownPos = new Vector3(Mathf.Cos(angle),
                0,
                Mathf.Sin(angle)) * spownDistance;
           
            spownPos += centerSpown.position;//中心位置に加算
          
            yield return new WaitForSeconds(spownInterval);

            //敵を生成
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
        if (waveStart == false) return; //ウェーブが開始されていないな処理しない 
        if (bossSpowned == true) return;//ボスが出現済みなら処理しない

        float currentTime = Time.time;
        //一定時間経過でボス出現
        if (currentTime - sTime >= spownBossTime)
        {
            Vector3 bossSpownPos = bossSpown.position; //スポーン位置設定
            Instantiate(boss, bossSpownPos, Quaternion.identity); //ボスを生成
            bossSpowned = true;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            player = GameObject.FindGameObjectWithTag("Player");　//敵の目標地点になるプレイヤーを設定
        }
        else
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WaveSetting(); //ウェーブ内容を読み込み
    }

    // Update is called once per frame
    void Update()
    {
        //敵出現
        StartCoroutine(SpownEnemy());
        BossSpown();
    }
}
