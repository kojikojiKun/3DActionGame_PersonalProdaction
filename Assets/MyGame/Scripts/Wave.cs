using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Game/Wave")]
public class Wave : ScriptableObject
{
    //public int waveId;

    public GameObject boss; //ウェーブの最後に出てくるボス

    public GameObject[] enemies; //ウェーブ中に出てくる通常の敵

    public int spownEnemyLimit; //ウェーブ中に出てくる敵の数

    public float spownInterval; //敵の出現間隔

    public float spownBossTime; //ウェーブが始まってボスが出現するまでの時間

}
