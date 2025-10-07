using UnityEngine;
using System.Collections.Generic;

public class TrapStaus : MonoBehaviour
{
    //�I�����ꂽ�������e�ɉ����ăg���b�v�̃X�e�[�^�X������
    public void SelectedUpGradeType(UpGradeType upGradeType, float mag)
    {
        //���g���b�v�̃A�b�v�O���[�h���I�����ꂽ
        bool isFlameUpgrade(UpGradeType type) =>        type == UpGradeType.FireStartSpeed ||
                                                        type == UpGradeType.FireInterval ||
                                                        type == UpGradeType.FireDuration ||
                                                        type == UpGradeType.FireDamageInterval ||
                                                        type == UpGradeType.FireDamage;

        //�N���X�{�E�g���b�v�̃A�b�v�O���[�h���I�����ꂽ
        bool isCrossBowUpgrade(UpGradeType type) =>     type == UpGradeType.CrossBowRange ||
                                                        type == UpGradeType.CrossBowInterval ||
                                                        type == UpGradeType.CrossBowIncreaceArrow ||
                                                        type == UpGradeType.CrossBowDamage;

        //�u���[�h�g���b�v�g���b�v�̃A�b�v�O���[�h���I�����ꂽ
        bool isSpinBladeUpgrade(UpGradeType type) =>    type == UpGradeType.BladeSize ||
                                                        type == UpGradeType.BladeSpeed ||
                                                        type == UpGradeType.BladeDamageInterval ||                                                      
                                                        type == UpGradeType.BladeDamage;

        bool isSpikeUpgrade(UpGradeType type) => type == UpGradeType.SpikeWallDamage ||
                                                        type == UpGradeType.SpikeWallDurability;
        //�I�����ꂽ�������e��ݒ�
        if (isFlameUpgrade(upGradeType)==true) UpgradeFlameTrapStatus(upGradeType, mag);
        if (isCrossBowUpgrade(upGradeType) == true) UpgradeCrossBowTrapStatus(upGradeType, mag);
        if (isSpinBladeUpgrade(upGradeType) == true) UpgradeSpinBladeTrapStatus(upGradeType, mag);
        if (isSpikeUpgrade(upGradeType) == true) UpgradeSpikeWallStatus(upGradeType, mag);
    }

    [Header("FireTrap�̃X�e�[�^�X")]
    [SerializeField] private float particleStartSpeed = 5f; //�p�[�e�B�N���̏����x      
    [SerializeField] private float shotFireInterval; //�p�[�e�B�N���̔����Ԋu   
    [SerializeField] private float shotFireDuration; //�p�[�e�B�N���̎�������    
    [SerializeField] private float fireDamageInterval; //�_���[�W�̔����Ԋu
    [SerializeField] private float fireDamage;

    //���g���b�v�̏����X�e�[�^�X
    private float stParticleStartSpeed;
    private float stFireInterval;
    private float stFireDuration;
    private float stFireDamageInterval;
    private float stFireDamage;

    //FlameTrap�̃X�e�[�^�X��ݒ�
    private void UpgradeFlameTrapStatus(UpGradeType upGradeType, float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.FireStartSpeed: //�p�[�e�B�N����startSpeed�𑝉�(�˒���������)
                particleStartSpeed = stParticleStartSpeed + (stParticleStartSpeed * mag);
                break;
            case UpGradeType.FireDuration: //���̎������ԑ���
                shotFireDuration = stFireDuration - (stFireDuration * mag);
                break;
            case UpGradeType.FireDamageInterval: //�_���[�W�Ԋu��Z�k
                fireDamageInterval = stFireDamageInterval - (stFireDamageInterval * mag);
                break;
            case UpGradeType.FireInterval: //���̔��ˊԊu��Z�k
                shotFireInterval = stFireInterval - (stFireInterval * mag);
                break;
            case UpGradeType.FireDamage: //���̃_���[�W�𑝉�
                fireDamage = stFireDamage + (fireDamage * mag);
                break;
            default:
                break;
        }
    }

    [Header("CrossBowTrap�̃X�e�[�^�X")]
    [SerializeField] private float shotArrowInterval; //��̔��ˊԊu
    [SerializeField] private float arrowDamage; //��̃_���[�W
    [SerializeField] private float crossBowRange; //�˒�����
    [SerializeField] private float spreadAngle;//��̊g�U����p�x
    [SerializeField] private int numOfArrow; //���Ԃ����̖{���@

    //�N���X�{�E�g���b�v�̏����X�e�[�^�X
    private float stCBInterval; //���ˊԊu�̏����l
    private float stCBDamage; //��̃_���[�W�̏����l
    private float stCBRange; //�˒������̏����l

    //CrossBowTrap�̃X�e�[�^�X��ݒ�
    private void UpgradeCrossBowTrapStatus(UpGradeType upGradeType, float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.CrossBowInterval: //��̔��ˊԊu�Z�k
                shotArrowInterval = stCBInterval - (stCBInterval * mag);
                break;
            case UpGradeType.CrossBowDamage: //��̃_���[�W����
                stCBDamage = stCBDamage - (stCBDamage * mag);
                break;
            case UpGradeType.CrossBowRange: //�N���X�{�E�̎˒���������
                crossBowRange = stCBRange + (stCBRange * mag);
                break;
            case UpGradeType.CrossBowIncreaceArrow: //���˂����𑝂₷
                numOfArrow++;
                break;
            default:
                break;
        }
    }

    [Header("BladeTrap�̃X�e�[�^�X")]
    [SerializeField] private float bladeSize; //�g���b�v�̑傫��
    [SerializeField] private float bladeSpeed; //�g���b�v�̈ړ����x
    [SerializeField] private float bladeDamageInterval; //�_���[�W�̔����Ԋu
    [SerializeField] private float bladeDamage; //�_���[�W��

    //�u���[�h�g���b�v�̏����X�e�[�^�X
    private float stBladeSize;
    private float stBladeSpeed;
    private float stBladeDamageInterval;
    private float stBladeDamage;

    //BladeTrap�̃X�e�[�^�X��ݒ�
    private void UpgradeSpinBladeTrapStatus(UpGradeType upGradeType,float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.BladeSize: //�u���[�h�̃T�C�Y��傫������
                bladeSize = stBladeSize + (stBladeSize * mag);
                break;
            case UpGradeType.BladeSpeed:
                bladeSpeed = stBladeSpeed + (stBladeSpeed * mag);
                break;
            case UpGradeType.BladeDamageInterval: //�u���[�h�̃_���[�W�Ԋu��Z�k
                bladeDamageInterval = stBladeDamageInterval - (stBladeDamageInterval * mag);
                break;
            case UpGradeType.BladeDamage: //�u���[�h�̃_���[�W�𑝂₷
                bladeDamage = stBladeDamage + (stBladeDamage * mag);
                break;
                default:
                break;
        }
    }

    [Header("SpikeWall�̃X�e�[�^�X")]
    [SerializeField] private float wallDurability; //spikeWall�̑ϋv�l
    [SerializeField] private float SpikeDamage; //spikeWall�̃_���[�W

    //�X�p�C�N�g���b�v�̏����X�e�[�^�X
    private float stWallDurability;
    private float stSpikeDamage;

    //SpikeWallTrap�̃X�e�[�^�X��ݒ�
    private void UpgradeSpikeWallStatus(UpGradeType upGradeType,float mag)
    {
        switch (upGradeType)
        {
            case UpGradeType.SpikeWallDurability: //�ϋv�l����
                wallDurability = stWallDurability + (stWallDurability * mag);
                break;
            case UpGradeType.SpikeWallDamage: //�X�p�C�N�̃_���[�W����
                wallDurability = stSpikeDamage + (stSpikeDamage * mag);
                break;
        }
    }

    public void GiveStatus()
    {
        foreach (TrapControl trapControl in TrapControl.AllTraps)
        {
            Debug.Log($"{trapControl.type}�Z�b�g");
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
        ///��������FlameTrap       
        //FlameTrap�̃X�e�[�^�X�̏����l��ۑ�
        stParticleStartSpeed = particleStartSpeed;
        stFireInterval = shotFireInterval;
        stFireDuration = shotFireDuration;
        stFireDamageInterval = fireDamageInterval;
        stFireDamage = fireDamage;
        ///�����܂�

        ///��������CrossBowTrap
        //CrossBowTrap�̃X�e�[�^�X�̏����l��ۑ�
        stCBInterval = shotArrowInterval;
        stCBDamage = arrowDamage;
        stCBRange = crossBowRange;
        //numOfArrow = 1;
        ///

        ///��������BladeTrap
        //BladeTrap�̃X�e�[�^�X�̏����l��ۑ�
        stBladeSize = bladeSize;
        stBladeSpeed = bladeSpeed;
        stBladeDamageInterval = bladeDamageInterval;
        stBladeDamage = bladeDamage;
        ///

        ///��������SpikeWallTrap
        //SpikeTrap�̃X�e�[�^�X�̏����l��ۑ�
        stWallDurability = wallDurability;
        stSpikeDamage = SpikeDamage;
        ///
    }

    // Update is called once per frame
    void Update()
    {

    }
}
