using UnityEngine;

public enum UpGradeType
{
    FireStartSpeed,
    FireDuration,
    FireDamageInterval,
    FireInterval,
    FireDamage,
    CrossBowInterval,
    CrossBowDamage,
    CrossBowRange,
    CrossBowIncreaceArrow,
    BladeSize,
    BladeSpeed,
    BladeDamageInterval,
    BladeDamage,
    SpikeWallDurability,
    SpikeWallDamage,
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
