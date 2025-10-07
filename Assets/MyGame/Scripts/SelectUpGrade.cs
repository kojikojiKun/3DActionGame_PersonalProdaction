using UnityEngine;

public enum UpGradeType
{
    FireStartSpeed, //炎のパーティクルの初速(射程距離)
    FireDuration, //炎の持続時間
    FireDamageInterval, //炎のダメージを与える間隔
    FireInterval, //炎の発射間隔
    FireDamage, //炎のダメージ量
    CrossBowInterval, //矢の発射間隔
    CrossBowDamage, //矢のダメージ
    CrossBowRange, //クロスボウトラップの射程距離
    CrossBowIncreaceArrow, //一度に発射する矢の数
    BladeSize, //ブレードトラップの大きさ
    BladeSpeed, //ブレードトラップの移動速度
    BladeDamageInterval, //ブレードトラップのダメージを与える間隔
    BladeDamage, //ブレードトラップのダメージ量
    SpikeWallDurability, //スパイクトラップの耐久値
    SpikeWallDamage, //スパイクトラップのダメージ量
    None
}
public class SelectUpGrade : MonoBehaviour
{
    public UpGradeType type;

    private GameObject getItemRange;

   /* public void WhichSelect()
    {
        switch (type)
        {
            
        }
    }
   */
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
