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

    //敵のステータス
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

    //プレイヤーを追跡
    void ChasePlayer()
    {
        if (m_target != null)
        {
            m_agent.destination = m_target.transform.position;
        }
    }

    //目的地を設定
    public void SetTarget(GameObject target)
    {
        m_target = target; //攻撃範囲内のオブジェクトを目標に設定.

        //範囲内のオブジェクトがnullの場合
        if (target == null)
        {
            //目標をorigiTargetに設定
            m_target = m_originTarget;
        }
    }

    //SetTargetで設定した目標に向けて攻撃
    public void Attack()
    {
        m_enemyAnimation.AttackAnim();
    }

    //攻撃判定セット(アニメーションイベント).
    public void SetAttackCol()
    {
        m_attackCol.enabled=true;
    }

    //攻撃判定削除(アニメーションイベント).
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
