using UnityEngine;

public enum EnemyType
{
    Type_1,
    Type_2,
    Type_3,
    Boss_1,
    Boss_2,
    Boss_3
}

[CreateAssetMenu(fileName = "EnemyStatus", menuName = "Game/EnemyStatus")]
public class EnemyData : ScriptableObject
{
    public EnemyType enemyType;
    public bool isBoss;
    public float hp;
    public float attackPower;
    public float diffencePower;
    public int dropRedExpValue;
    public int dropYellowExpValue;
    public int dropGreenExpValue;
    public int dropItemValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
