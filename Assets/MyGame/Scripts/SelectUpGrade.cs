using UnityEngine;

public enum UpGradeType
{
    FireStartSpeed, //���̃p�[�e�B�N���̏���(�˒�����)
    FireDuration, //���̎�������
    FireDamageInterval, //���̃_���[�W��^����Ԋu
    FireInterval, //���̔��ˊԊu
    FireDamage, //���̃_���[�W��
    CrossBowInterval, //��̔��ˊԊu
    CrossBowDamage, //��̃_���[�W
    CrossBowRange, //�N���X�{�E�g���b�v�̎˒�����
    CrossBowIncreaceArrow, //��x�ɔ��˂����̐�
    BladeSize, //�u���[�h�g���b�v�̑傫��
    BladeSpeed, //�u���[�h�g���b�v�̈ړ����x
    BladeDamageInterval, //�u���[�h�g���b�v�̃_���[�W��^����Ԋu
    BladeDamage, //�u���[�h�g���b�v�̃_���[�W��
    SpikeWallDurability, //�X�p�C�N�g���b�v�̑ϋv�l
    SpikeWallDamage, //�X�p�C�N�g���b�v�̃_���[�W��
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
