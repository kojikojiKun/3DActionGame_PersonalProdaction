using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Game/Wave")]
public class Wave : ScriptableObject
{
    //public int waveId;

    public ObjectPool[] bossPool;
    public ObjectPool[] enemyPool;

    public int spownEnemyLimit; //ウェーブ中に出てくる敵の数

    public float spownInterval; //敵の出現間隔

    public float spownBossTime; //ウェーブが始まってボスが出現するまでの時間

}
