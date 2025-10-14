using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Game/Wave")]
public class Wave : ScriptableObject
{
    //public int waveId;

    [Header("PoolManagerのインスペクタで設定した敵のプールの名前を入力")]
    public string[] bossName;
    public string[] enemyName;

    [Header("ウェーブ内容")]
    public int spownEnemyLimit; //ウェーブ中に出てくる敵の数
    public float spownInterval; //敵の出現間隔
    public float spownBossTime; //ウェーブが始まってボスが出現するまでの時間

}
