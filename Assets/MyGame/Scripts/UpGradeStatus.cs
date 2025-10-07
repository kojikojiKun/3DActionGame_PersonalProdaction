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

    //アップグレードにかかるコストを取得
    public float GetupGradeCost()
    {
        return baseData.baseCost + baseData.costIncreasePerLevel * level;
    }

    //HPを強化
    public void UpHp()
    {
        hp *= baseData.hpMlp;
        level++;
        Debug.Log($"HP:{hp}...レベル:{level}");
    }

    //移動スピードを強化
    public void UpSpeed()
    {
        speed *= baseData.SpMlp;
        level++;
        Debug.Log($"SPEED:{speed}...レベル:{level}");
    }

    //攻撃速度を強化
    public void UpAttackSpeed()
    {
        attackSpeed *= baseData.AsMlp;
        level++;
        Debug.Log($"AS:{attackSpeed}...レベル:{level}");
    }

    //攻撃力を強化
    public void UpAttackPower()
    {
        attackPower *= baseData.ApMlp;
        level++;
        Debug.Log($"AP:{attackPower}...レベル:{level}");
    }
}
