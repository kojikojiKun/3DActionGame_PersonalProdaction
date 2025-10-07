using UnityEngine;

public enum CharacterType
{
    StandardType, //�W���X�e�[�^�X�̃L����
    SpeedType, //�X�s�[�h�ɓ��������L����
    PowerType,�@//�U���A�ϋv�͂ɓ��������L����
}

[CreateAssetMenu(fileName = "CharacterStatus", menuName = "Game/CharacterStatus")]
public class CharacterStatus : ScriptableObject
{
    public CharacterType type; //�L�����N�^�[�̃^�C�v�����߂�
    public string id;

    [Header("��̃L�����N�^�[�X�e�[�^�X")]
    public float hp; //�q�b�g�|�C���g
    public float speed; //�ړ��X�s�[�h
    public float attackSpeed; //�U�����x
    public float attackPower; //�U����

    [Header("�X�e�[�^�X�A�b�v�O���[�h�̂Ƃ��̔{��")]
    public float hpMlp;
    public float SpMlp;
    public float AsMlp;
    public float ApMlp;

    public float baseCost; //�A�b�v�O���[�h�ɂ�����R�X�g
    public float costIncreasePerLevel; //���x�����Ƃɏオ��R�X�g
}
