using UnityEngine;
using System.Collections.Generic;

public class CrossBowControl : MonoBehaviour
{
    [SerializeField] TrapStaus trapStaus;
    public static List<CrossBowControl> crossBows = new List<CrossBowControl>();
    private float shotInterval;
    private float damage;
    private float shotRange;
    private float spreadAngle;
    private int arrowCnt;

    public void SetStatus(float interval, float dmg, float range, float angle, int cnt)
    {
        shotInterval = interval;
        damage = dmg;
        shotRange = range;
        spreadAngle = angle;
        arrowCnt = cnt;
    }

    private void Awake()
    {
        crossBows.Add(this);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
