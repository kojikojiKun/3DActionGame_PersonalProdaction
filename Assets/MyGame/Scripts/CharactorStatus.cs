using UnityEngine;

public enum CharacterType
{
    StandardType, //標準ステータスのキャラ
    SpeedType, //スピードに特化したキャラ
    PowerType,　//攻撃、耐久力に特化したキャラ
}

[CreateAssetMenu(fileName = "CharacterStatus", menuName = "Game/CharacterStatus")]
public class CharacterStatus : ScriptableObject
{
    public CharacterType type; //キャラクターのタイプを決める
    public string id;

    [Header("基準のキャラクターステータス")]
    public float hp; //ヒットポイント
    public float speed; //移動スピード
    public float attackSpeed; //攻撃速度
    public float attackPower; //攻撃力

    [Header("ステータスアップグレードのときの倍率")]
    public float hpMlp;
    public float SpMlp;
    public float AsMlp;
    public float ApMlp;

    public float baseCost; //アップグレードにかかるコスト
    public float costIncreasePerLevel; //レベルごとに上がるコスト
}
