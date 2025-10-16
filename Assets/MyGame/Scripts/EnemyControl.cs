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

    //敵のステータス
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
        m_enemyColider.enabled=true; //コライダー有効化.
        m_originTarget = m_spawner.GetOriginTarget; //初期ターゲットを設定.
        m_player = m_spawner.GetPlayerController;

        //ステータス初期化
        m_HP = m_data.hp;
        m_power = m_data.attackPower;
        m_diffence = m_data.diffencePower;
        m_greenExpValue = m_data.dropGreenExpValue;
        m_yellowExpValue = m_data.dropYellowExpValue;
        m_redExpValue = m_data.dropRedExpValue;
        m_itemValue = m_data.dropItemValue;
    }

    //目標を追跡.
    void ChasePlayer()
    {
        if (m_target != null)
        {
            m_agent.destination = m_target.transform.position; //目標に向かって移動.
        }
    }

    //目的地を設定
    public void SetTarget(GameObject target)
    {
        m_target = target; //攻撃範囲内のオブジェクトを目標に設定.

        //範囲内のオブジェクトがnullの場合.
        if (target == null)
        {
            //目標をorigiTargetに設定.
            m_target = m_originTarget;
        }
    }

    //SetTargetで設定した目標に向けて攻撃.
    public void Attack()
    {
        transform.LookAt(m_target.transform.position); //目標の方向を向く.
        m_enemyAnimation.AttackAnim(); //攻撃アニメーションを再生.
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

    public void TakeDamage(float damage)
    {
        float dmg = damage * m_diffence;
        m_HP -= dmg;
        m_enemyAnimation.DamageAnim();
        //HPが0になったとき.
        if (m_HP <= 0)
        {
            m_HP = 0; //0以下にならないようにする.
            Dead();
        }
    }

    //敵の死亡処理.
    void Dead()
    {
        m_spawner.EnemyKilled();
        m_enemyColider.enabled=false; //コライダー無効化.
        m_enemyAnimation.DeadAnim(); //死亡アニメーション再生
        //プールに戻す.
        StartCoroutine(WaitReturnPool());
    }

    //死亡したら経験値やアイテムをドロップするようにする.

    //死亡アニメーション完了まで待機.
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
