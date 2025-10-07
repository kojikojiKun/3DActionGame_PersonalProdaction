using UnityEngine;

public class UpGradeStatus
{
    public CharacterStatus baseData;
    public int level;

    public float hp;
    public float speed;
    public float attackSpeed;
    public float attackPower;

    public UpGradeStatus(CharacterStatus data)
    {
        baseData = data;
        hp = data.hp;
        speed = data.speed;
        attackSpeed = data.attackSpeed;
        attackPower = data.attackPower;
    }

    //�A�b�v�O���[�h�ɂ�����R�X�g���擾
    public float GetupGradeCost()
    {
        return baseData.baseCost + baseData.costIncreasePerLevel * level;
    }

    //HP������
    public void UpHp()
    {
        hp *= baseData.hpMlp;
        level++;
        Debug.Log($"HP:{hp}...���x��:{level}");
    }

    //�ړ��X�s�[�h������
    public void UpSpeed()
    {
        speed *= baseData.SpMlp;
        level++;
        Debug.Log($"SPEED:{speed}...���x��:{level}");
    }

    //�U�����x������
    public void UpAttackSpeed()
    {
        attackSpeed *= baseData.AsMlp;
        level++;
        Debug.Log($"AS:{attackSpeed}...���x��:{level}");
    }

    //�U���͂�����
    public void UpAttackPower()
    {
        attackPower *= baseData.ApMlp;
        level++;
        Debug.Log($"AP:{attackPower}...���x��:{level}");
    }
}
