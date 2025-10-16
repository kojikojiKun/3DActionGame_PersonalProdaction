using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] EnemySpawner m_spawner;
    [SerializeField] PooledObject m_pooledObject;
    [SerializeField] NavMeshAgent m_agent;
    [SerializeField] EnemyAnimation m_enemyAnimation;
    [SerializeField] Collider m_enemyColider;
    [SerializeField] Collider m_attackCol;
    [SerializeField] EnemyData m_data;
    private GameObject m_originTarget;
    private PlayerController m_player;
    public PlayerController GetPlayer => m_player;
    public GameObject GetOriginTarget => m_originTarget;

    //�G�̃X�e�[�^�X
    private GameObject m_target;
    private EnemyType m_enemyType;
    private float m_HP;
    private float m_power;
    private float m_diffence;
    private int m_greenExpValue;
    private int m_yellowExpValue;
    private int m_redExpValue;
    private int m_itemValue;
    private bool m_isEnter;
    private bool m_canAttack;

    public float AP => m_power;
    void Initialize()
    {
        m_enemyColider.enabled=true; //�R���C�_�[�L����.
        m_originTarget = m_spawner.GetOriginTarget; //�����^�[�Q�b�g��ݒ�.
        m_player = m_spawner.GetPlayerController;

        //�X�e�[�^�X������
        m_HP = m_data.hp;
        m_power = m_data.attackPower;
        m_diffence = m_data.diffencePower;
        m_greenExpValue = m_data.dropGreenExpValue;
        m_yellowExpValue = m_data.dropYellowExpValue;
        m_redExpValue = m_data.dropRedExpValue;
        m_itemValue = m_data.dropItemValue;
    }

    //�ڕW��ǐ�.
    void ChasePlayer()
    {
        if (m_target != null)
        {
            m_agent.destination = m_target.transform.position; //�ڕW�Ɍ������Ĉړ�.
        }
    }

    //�ړI�n��ݒ�
    public void SetTarget(GameObject target)
    {
        m_target = target; //�U���͈͓��̃I�u�W�F�N�g��ڕW�ɐݒ�.

        //�͈͓��̃I�u�W�F�N�g��null�̏ꍇ.
        if (target == null)
        {
            //�ڕW��origiTarget�ɐݒ�.
            m_target = m_originTarget;
        }
    }

    //SetTarget�Őݒ肵���ڕW�Ɍ����čU��.
    public void Attack()
    {
        transform.LookAt(m_target.transform.position); //�ڕW�̕���������.
        m_enemyAnimation.AttackAnim(); //�U���A�j���[�V�������Đ�.
    }

    //�U������Z�b�g(�A�j���[�V�����C�x���g).
    public void SetAttackCol()
    {
        m_attackCol.enabled=true;
    }

    //�U������폜(�A�j���[�V�����C�x���g).
    public void RemoveAttackCol()
    {
        m_attackCol.enabled = false;
    }

    public void TakeDamage(float damage)
    {
        float dmg = damage * m_diffence;
        m_HP -= dmg;
        m_enemyAnimation.DamageAnim();
        //HP��0�ɂȂ����Ƃ�.
        if (m_HP <= 0)
        {
            m_HP = 0; //0�ȉ��ɂȂ�Ȃ��悤�ɂ���.
            Dead();
        }
    }

    //�G�̎��S����.
    void Dead()
    {
        m_spawner.EnemyKilled();
        m_enemyColider.enabled=false; //�R���C�_�[������.
        m_enemyAnimation.DeadAnim(); //���S�A�j���[�V�����Đ�
        //�v�[���ɖ߂�.
        StartCoroutine(WaitReturnPool());
    }

    //���S������o���l��A�C�e�����h���b�v����悤�ɂ���.

    //���S�A�j���[�V���������܂őҋ@.
    private IEnumerator WaitReturnPool()
    {
        yield return new WaitForSeconds(2f);

        m_pooledObject.ReturnToPool();
        Initialize();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        Initialize();
        m_target = m_originTarget;
    }

    // Update is called once per frame
    void Update()
    {
        ChasePlayer();
    }
}
