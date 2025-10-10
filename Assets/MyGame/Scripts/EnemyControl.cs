using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] EnemyManager m_manager;
    [SerializeField] NavMeshAgent m_agent;
    [SerializeField] EnemyAnimation m_enemyAnimation;
    [SerializeField] Collider m_attackCol;
    [SerializeField] EnemyData m_data;
    private GameObject m_originTarget;
    private PlayerController m_player;
    public PlayerController GetPlayer => m_player;
    public GameObject OriginTarget => m_originTarget;

    //�G�̃X�e�[�^�X
    [SerializeField] private float m_coolDown;

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
    void SetStatus()
    {
        m_originTarget = m_manager.OriginTarget;
        m_player = m_manager.GetPlayer;
        m_HP = m_data.hp;
        m_power = m_data.attackPower;
        m_diffence = m_data.diffencePower;
        m_greenExpValue = m_data.dropGreenExpValue;
        m_yellowExpValue = m_data.dropYellowExpValue;
        m_redExpValue = m_data.dropRedExpValue;
        m_itemValue = m_data.dropItemValue;
    }

    //�v���C���[��ǐ�
    void ChasePlayer()
    {
        if (m_target != null)
        {
            m_agent.destination = m_target.transform.position;
        }
    }

    //�ړI�n��ݒ�
    public void SetTarget(GameObject target)
    {
        m_target = target; //�U���͈͓��̃I�u�W�F�N�g��ڕW�ɐݒ�.

        //�͈͓��̃I�u�W�F�N�g��null�̏ꍇ
        if (target == null)
        {
            //�ڕW��origiTarget�ɐݒ�
            m_target = m_originTarget;
        }
    }

    //SetTarget�Őݒ肵���ڕW�Ɍ����čU��
    public void Attack()
    {
        m_enemyAnimation.AttackAnim();
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

    private void Awake()
    {
        SetStatus();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_target = m_originTarget;
    }

    // Update is called once per frame
    void Update()
    {
        ChasePlayer();
    }
}
