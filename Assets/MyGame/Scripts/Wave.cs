using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Game/Wave")]
public class Wave : ScriptableObject
{
    //public int waveId;

    [Header("PoolManager�̃C���X�y�N�^�Őݒ肵���G�̃v�[���̖��O�����")]
    public string[] bossName;
    public string[] enemyName;

    [Header("�E�F�[�u���e")]
    public int spownEnemyLimit; //�E�F�[�u���ɏo�Ă���G�̐�
    public float spownInterval; //�G�̏o���Ԋu
    public float spownBossTime; //�E�F�[�u���n�܂��ă{�X���o������܂ł̎���

}
