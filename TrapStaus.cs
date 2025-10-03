using UnityEngine;
using System.Collections.Generic;

public class TrapStaus : MonoBehaviour
{
    //選択された強化内容に応じてトラップのステータスを強化
    public void SelectedUpGradeType(UpGradeType upGradeType, float mag)
    {
        //炎トラップのアップグレードが選択された
        bool isFlameUpgrade(UpGradeType type) =>        type == UpGradeType.FlameStartSpeed ||
                                                        type == UpGradeType.FlameInterval ||
                                                        type == UpGradeType.FlameDuration ||
                                                        type == UpGradeType.FlameDamageDuration ||
                                                        type == UpGradeType.FlameDamage;

        //クロスボウトラップのアップグレードが選択された
        bool isCrossBowUpgrade(UpGradeType type) =>     type == UpGradeType.CrossBowRange ||
                                                        type == UpGradeType.CrossBowInterval ||
                                                        type == UpGradeType.CrossBowIncreaceArrow ||
                                                        type == UpGradeType.CrossBowDamage;

        //ブレードトラップトラップのアップグレードが選択された
        bool isSpinBladeUpgrade(UpGradeType type) =>    type == UpGradeType.BladeSize ||
                                                        type == UpGradeType.BladeSize ||
                                                        type == UpGradeType.BladeDamageDuration;

        bool isSpikeUpgrade(UpGradeType type) => type == UpGradeType.SpikeWallDamage ||
                                                        type == UpGradeType.SpikeWallDurability;
        //選択された強化内容を設定
        if (isFlameUpgrade(upGradeType)==true) UpgradeFlameTrapStatus(upGradeType, mag);
        if (isCrossBowUpgrade(upGradeType) == true) UpgradeCrossBowTrapStatus(upGradeType, mag);
        if (isSpinBladeUpgrade(upGradeType) == true) UpgradeSpinBladeTrapStatus(upGradeType, mag);
        if (isSpikeUpgrade(upGradeType) == true) UpgradeSpikeWallStatus(upGradeType, mag);
    }

    [Header("FlameTrapのステータス")]
    public float particleStartSpeed = 5f; //パーティクルの初速度      
    public float shotFlameInterval; //パーティクルの発生間隔   
    public float shotFlameDuration; //パーティクルの持続時間    
    public float flameDamageInterval; //ダメージの発生間隔
    public float flameDamage;

    //炎トラップの初期ステータス
    private float stParticleStartSpeed;
    private float stFlameInterval;
    private float stFlameDuration;
    private float stFlameDamageInterval;
    private float stFlameDamage;

    //FlameTrapのステータスを設定
    private void UpgradeFlameTrapStatus(UpGradeType upGradeType, float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.FlameStartSpeed: //パーティクルのstartSpeedを増加(射程距離増加)
                particleStartSpeed = stParticleStartSpeed + (stParticleStartSpeed * mag);
                break;
            case UpGradeType.FlameDuration: //炎の持続時間増加
                shotFlameDuration = stFlameDuration - (stFlameDuration * mag);
                break;
            case UpGradeType.FlameDamageDuration: //ダメージ間隔を短縮
                flameDamageInterval = stFlameDamageInterval - (stFlameDamageInterval * mag);
                break;
            case UpGradeType.FlameInterval: //炎の発射間隔を短縮
                shotFlameInterval = stFlameInterval - (stFlameInterval * mag);
                break;
            case UpGradeType.FlameDamage: //炎のダメージを増加
                flameDamage = stFlameDamage + (flameDamage * mag);
                break;
            default:
                break;
        }

        TrapControl[] allTraps = FindObjectsByType<TrapControl>(FindObjectsSortMode.None); //シーン内のすべてのトラップオブジェクトを取得
        List<TrapControl> allFlameTraps = new List<TrapControl>(); //炎トラップのリストを作成
        foreach (TrapControl trapControl in allTraps)
        {
            //TrapType.FlameTrapのオブジェクトを探す
            if (trapControl.type == TrapType.FlameTrap)
            {
                allFlameTraps.Add(trapControl); //オブジェクトをリストに追加
            }
        }

        foreach (TrapControl flameTraps in allFlameTraps)
        {
            flameTraps.SetFlameTrapStasus(
                particleStartSpeed,
                shotFlameInterval,
                shotFlameDuration,
                flameDamageInterval,
                flameDamage);
        }
    }

    [Header("CrossBowTrapのステータス")]
    public float shotArrowInterval; //矢の発射間隔
    public float arrowDamage; //矢のダメージ
    public float crossBowRange; //射程距離
    public int numOfArrow = 1; //発車する矢の本数　

    //クロスボウトラップの初期ステータス
    private float stCBInterval; //発射間隔の初期値
    private float stCBDamage; //矢のダメージの初期値
    private float stCBRange; //射程距離の初期値

    //CrossBowTrapのステータスを設定
    private void UpgradeCrossBowTrapStatus(UpGradeType upGradeType, float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.CrossBowInterval: //矢の発射間隔短縮
                shotArrowInterval = stCBInterval - (stCBInterval * mag);
                break;
            case UpGradeType.CrossBowDamage: //矢のダメージ増加
                stCBDamage = stCBDamage - (stCBDamage * mag);
                break;
            case UpGradeType.CrossBowRange: //クロスボウの射程距離増加
                crossBowRange = stCBRange + (stCBRange * mag);
                break;
            case UpGradeType.CrossBowIncreaceArrow: //発射する矢を増やす
                numOfArrow++;
                break;
            default:
                break;
        }

        TrapControl[] allTraps = FindObjectsByType<TrapControl>(FindObjectsSortMode.None); //シーン内のすべてのトラップオブジェクトを取得
        List<TrapControl> allCrossBowTraps = new List<TrapControl>(); //クロスボウトラップのリストを作成     
        foreach (TrapControl trapControl in allTraps)
        {
            //TrapType.CrossBowTrapのオブジェクトを探す
            if (trapControl.type == TrapType.CrossBowTrap)
            {
                allCrossBowTraps.Add(trapControl); //オブジェクトをリストに追加
            }         
        }

        foreach(TrapControl crossBowTraps in allCrossBowTraps)
        {
            crossBowTraps.SetCrossBowTrapStatus(shotArrowInterval, arrowDamage, crossBowRange, numOfArrow);
        }
    }

    [Header("BladeTrapのステータス")]
    public float bladeSize; //トラップの大きさ
    public float bladeDamageDuration; //ダメージの発生間隔
    public float bladeDamage; //ダメージ量

    //ブレードトラップの初期ステータス
    private float stBladeSize;
    private float stBladeDamageDuration;
    private float stBladeDamage;

    //BladeTrapのステータスを設定
    private void UpgradeSpinBladeTrapStatus(UpGradeType upGradeType,float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.BladeSize: //ブレードのサイズを大きくする
                bladeSize = stBladeSize + (stBladeSize * mag);
                break;
            case UpGradeType.BladeDamageDuration: //ブレードのダメージ間隔を短縮
                bladeDamageDuration = stBladeDamageDuration - (stBladeDamageDuration * mag);
                break;
            case UpGradeType.BladeDamage: //ブレードのダメージを増やす
                bladeDamage = stBladeDamage + (stBladeDamage * mag);
                break;
                default:
                break;
        }

        TrapControl[] allTraps = FindObjectsByType<TrapControl>(FindObjectsSortMode.None); //シーン内のすべてのトラップオブジェクトを取得
        List<TrapControl> spinBladeTraps = new List<TrapControl>(); //ブレードトラップのリストを作成
        foreach (TrapControl trapControl in allTraps)
        {
            //TrapType.SpinBladeTrapのオブジェクトを探す
            if (trapControl.type == TrapType.SpinBladeTrap)
            {
                spinBladeTraps.Add(trapControl); //オブジェクトをリストに追加　
            }
        }

    }

    [Header("SpikeWallのステータス")]
    public float wallDurability; //spikeWallの耐久値
    public float SpikeDamage; //spikeWallのダメージ

    //スパイクトラップの初期ステータス
    private float stWallDurability;
    private float stSpikeDamage;

    //SpikeWallTrapのステータスを設定
    private void UpgradeSpikeWallStatus(UpGradeType upGradeType,float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.SpikeWallDurability: //耐久値増加
                wallDurability = stWallDurability + (stWallDurability * mag);
                break;
            case UpGradeType.SpikeWallDamage: //スパイクのダメージ増加
                wallDurability = stSpikeDamage + (stSpikeDamage * mag);
                break;
        }

        TrapControl[] allTraps = FindObjectsByType<TrapControl>(FindObjectsSortMode.None); //シーン内のすべてのトラップオブジェクトを取得
        List<TrapControl> spikeWalls = new List<TrapControl>(); //スパイクトラップのリストを作成
        foreach (TrapControl trapControl in allTraps)
        {
            //TrapType.SpikeTrapのオブジェクトを探す
            if (trapControl.type == TrapType.SpikeTrap)
            {
                spikeWalls.Add(trapControl); //オブジェクトをリストに追加
            }
        }
    }

    public static TrapStaus instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ///ここからFlameTrap       
        //FlameTrapのステータスの初期値を保存
        stParticleStartSpeed = particleStartSpeed;
        stFlameInterval = shotFlameInterval;
        stFlameDuration = shotFlameDuration;
        stFlameDamageInterval = flameDamageInterval;
        stFlameDamage = flameDamage;
        ///ここまで

        ///ここからCrossBowTrap
        //CrossBowTrapのステータスの初期値を保存
        stCBInterval = shotArrowInterval;
        stCBDamage = arrowDamage;
        stCBRange = crossBowRange;
        numOfArrow = 1;
        ///

        ///ここからBladeTrap
        //BladeTrapのステータスの初期値を保存
        stBladeSize = bladeSize;
        stBladeDamageDuration = bladeDamageDuration;
        stBladeDamage = bladeDamage;
        ///

        ///ここからSpikeWallTrap
        //SpikeTrapのステータスの初期値を保存
        stWallDurability = wallDurability;
        stSpikeDamage = SpikeDamage;
        ///
    }

    // Update is called once per frame
    void Update()
    {

    }
}
