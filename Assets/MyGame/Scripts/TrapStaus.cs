using UnityEngine;
using System.Collections.Generic;

public class TrapStaus : MonoBehaviour
{
    //選択された強化内容に応じてトラップのステータスを強化
    public void SelectedUpGradeType(UpGradeType upGradeType, float mag)
    {
        //炎トラップのアップグレードが選択された
        bool isFlameUpgrade(UpGradeType type) =>        type == UpGradeType.FireStartSpeed ||
                                                        type == UpGradeType.FireInterval ||
                                                        type == UpGradeType.FireDuration ||
                                                        type == UpGradeType.FireDamageInterval ||
                                                        type == UpGradeType.FireDamage;

        //クロスボウトラップのアップグレードが選択された
        bool isCrossBowUpgrade(UpGradeType type) =>     type == UpGradeType.CrossBowRange ||
                                                        type == UpGradeType.CrossBowInterval ||
                                                        type == UpGradeType.CrossBowIncreaceArrow ||
                                                        type == UpGradeType.CrossBowDamage;

        //ブレードトラップトラップのアップグレードが選択された
        bool isSpinBladeUpgrade(UpGradeType type) =>    type == UpGradeType.BladeSize ||
                                                        type == UpGradeType.BladeSpeed ||
                                                        type == UpGradeType.BladeDamageInterval ||                                                      
                                                        type == UpGradeType.BladeDamage;

        bool isSpikeUpgrade(UpGradeType type) => type == UpGradeType.SpikeWallDamage ||
                                                        type == UpGradeType.SpikeWallDurability;
        //選択された強化内容を設定
        if (isFlameUpgrade(upGradeType)==true) UpgradeFlameTrapStatus(upGradeType, mag);
        if (isCrossBowUpgrade(upGradeType) == true) UpgradeCrossBowTrapStatus(upGradeType, mag);
        if (isSpinBladeUpgrade(upGradeType) == true) UpgradeSpinBladeTrapStatus(upGradeType, mag);
        if (isSpikeUpgrade(upGradeType) == true) UpgradeSpikeWallStatus(upGradeType, mag);
    }

    [Header("FireTrapのステータス")]
    [SerializeField] private float particleStartSpeed = 5f; //パーティクルの初速度      
    [SerializeField] private float shotFireInterval; //パーティクルの発生間隔   
    [SerializeField] private float shotFireDuration; //パーティクルの持続時間    
    [SerializeField] private float fireDamageInterval; //ダメージの発生間隔
    [SerializeField] private float fireDamage;

    //炎トラップの初期ステータス
    private float stParticleStartSpeed;
    private float stFireInterval;
    private float stFireDuration;
    private float stFireDamageInterval;
    private float stFireDamage;

    //FlameTrapのステータスを設定
    private void UpgradeFlameTrapStatus(UpGradeType upGradeType, float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.FireStartSpeed: //パーティクルのstartSpeedを増加(射程距離増加)
                particleStartSpeed = stParticleStartSpeed + (stParticleStartSpeed * mag);
                break;
            case UpGradeType.FireDuration: //炎の持続時間増加
                shotFireDuration = stFireDuration - (stFireDuration * mag);
                break;
            case UpGradeType.FireDamageInterval: //ダメージ間隔を短縮
                fireDamageInterval = stFireDamageInterval - (stFireDamageInterval * mag);
                break;
            case UpGradeType.FireInterval: //炎の発射間隔を短縮
                shotFireInterval = stFireInterval - (stFireInterval * mag);
                break;
            case UpGradeType.FireDamage: //炎のダメージを増加
                fireDamage = stFireDamage + (fireDamage * mag);
                break;
            default:
                break;
        }
    }

    [Header("CrossBowTrapのステータス")]
    [SerializeField] private float shotArrowInterval; //矢の発射間隔
    [SerializeField] private float arrowDamage; //矢のダメージ
    [SerializeField] private float crossBowRange; //射程距離
    [SerializeField] private float spreadAngle;//矢の拡散する角度
    [SerializeField] private int numOfArrow; //発車する矢の本数　

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
    }

    [Header("BladeTrapのステータス")]
    [SerializeField] private float bladeSize; //トラップの大きさ
    [SerializeField] private float bladeSpeed; //トラップの移動速度
    [SerializeField] private float bladeDamageInterval; //ダメージの発生間隔
    [SerializeField] private float bladeDamage; //ダメージ量

    //ブレードトラップの初期ステータス
    private float stBladeSize;
    private float stBladeSpeed;
    private float stBladeDamageInterval;
    private float stBladeDamage;

    //BladeTrapのステータスを設定
    private void UpgradeSpinBladeTrapStatus(UpGradeType upGradeType,float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.BladeSize: //ブレードのサイズを大きくする
                bladeSize = stBladeSize + (stBladeSize * mag);
                break;
            case UpGradeType.BladeSpeed:
                bladeSpeed = stBladeSpeed + (stBladeSpeed * mag);
                break;
            case UpGradeType.BladeDamageInterval: //ブレードのダメージ間隔を短縮
                bladeDamageInterval = stBladeDamageInterval - (stBladeDamageInterval * mag);
                break;
            case UpGradeType.BladeDamage: //ブレードのダメージを増やす
                bladeDamage = stBladeDamage + (stBladeDamage * mag);
                break;
                default:
                break;
        }
    }

    [Header("SpikeWallのステータス")]
    [SerializeField] private float wallDurability; //spikeWallの耐久値
    [SerializeField] private float SpikeDamage; //spikeWallのダメージ

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
    }

    public void GiveStatus()
    {
        foreach (TrapControl trapControl in TrapControl.AllTraps)
        {
            Debug.Log($"{trapControl.type}セット");
            switch (trapControl.type)
            {
                case TrapType.FireTrap:
                    trapControl.SetFireTrapStasus(particleStartSpeed,shotFireInterval,shotFireDuration,shotFireDuration,fireDamage);
                    break;
                case TrapType.CrossBowTrap:
                    trapControl.SetCrossBowTrapStatus(shotArrowInterval, arrowDamage, crossBowRange,spreadAngle, numOfArrow);
                    break;
                case TrapType.SpinBladeTrap:
                    break;
                case TrapType.SpikeTrap:
                    break;
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
        stFireInterval = shotFireInterval;
        stFireDuration = shotFireDuration;
        stFireDamageInterval = fireDamageInterval;
        stFireDamage = fireDamage;
        ///ここまで

        ///ここからCrossBowTrap
        //CrossBowTrapのステータスの初期値を保存
        stCBInterval = shotArrowInterval;
        stCBDamage = arrowDamage;
        stCBRange = crossBowRange;
        //numOfArrow = 1;
        ///

        ///ここからBladeTrap
        //BladeTrapのステータスの初期値を保存
        stBladeSize = bladeSize;
        stBladeSpeed = bladeSpeed;
        stBladeDamageInterval = bladeDamageInterval;
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
