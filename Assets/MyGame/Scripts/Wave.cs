using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Game/Wave")]
public class Wave : ScriptableObject
{
    //public int waveId;

    public ObjectPool[] bossPool;
    public ObjectPool[] enemyPool;

    public int spownEnemyLimit; //�E�F�[�u���ɏo�Ă���G�̐�

    public float spownInterval; //�G�̏o���Ԋu

    public float spownBossTime; //�E�F�[�u���n�܂��ă{�X���o������܂ł̎���

}
