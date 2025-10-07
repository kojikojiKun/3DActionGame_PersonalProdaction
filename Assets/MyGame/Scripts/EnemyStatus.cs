using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class EnemyStatus : MonoBehaviour
{
    //参照
    public EnemyType type;
    [Header("ScriptableObject(EnemyData)を設定")]
    public EnemyData enemyData;
    [Header("攻撃範囲")]
    public Collider attackCollider;
    private Animator animator;
    private NavMeshAgent agent;
    public GameObject player;
    private PlayerController playerController;
    private HitAttack hitAttack;

    //経験値のオブジェクト
    [SerializeField] private GameObject[] dropExp;

    //ドロップするアイテムのオブジェクト
    [SerializeField] private GameObject dropItem;
    [SerializeField] private float dropForce; //アイテムを飛ばす力

    public float Dmg; //プレイヤーから受けるダメージ
    private float dmgMultipul; //プレイヤーのダメージ倍率
    public float attackInterval; //攻撃間隔
    
    //敵キャラクターのステータス
    public float hp;
    public float attackPower;
    public float diffencePower;

    public int dropRedExpValue;
    public int dropYellowExpValue;
    public int dropGreenExpValue;

    public int dropItemValue;
    public bool isBoss;

    private bool isDead=false; //死亡フラグ
    private bool isAttacked=false; //攻撃後フラグ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetStatus();
        animator=GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = EnemySpowner.instance.player;
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            hitAttack = player.GetComponent<HitAttack>();
        }
    }

    //ステータスを読み込み代入する
    void SetStatus()
    {
        var data = enemyData;
        if (type == enemyData.enemyType)
        {
            hp = data.hp;
            attackPower = data.attackPower;
            diffencePower = data.diffencePower;
            dropRedExpValue = data.dropRedExpValue;
            dropYellowExpValue = data.dropYellowExpValue;
            dropGreenExpValue = data.dropGreenExpValue;
            dropItemValue = data.dropItemValue;
            isBoss = data.isBoss;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead == true) return;
        //プレイヤーの攻撃判定に入ったとき
        if (other.CompareTag("PlayerAttackRange"))
        {
            //プレイヤーから受けるダメージを計算
            dmgMultipul = hitAttack.dmgMultipul;
            float power = playerController.attackPower;
        
            Dmg = Mathf.RoundToInt((power * dmgMultipul) / diffencePower); //int型に変換

            TakeDamageEnemy(Dmg);

           
        }
    }

    //HPを減らす
    public void TakeDamageEnemy(float dmg)
    {
        if (isDead == true) return;
        
        hp -= dmg;
        Debug.LogWarning($"{dmg}のダメージを受けた");

        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
            EnemyDead();
        }
    }

    //アニメーションイベントで攻撃判定を出す
    public void SetEnemyAttackCollider()
    {
        if (isDead == true) return; //死亡しているなら攻撃しない

        isAttacked = !isAttacked;

        //攻撃している時だけ当たり判定を出す
        if (isAttacked == false)
        {
            attackCollider.enabled = false;
        }
        else
        {
            attackCollider.enabled = true;
        }
    }

    private bool AttackDelay=true; //攻撃待機のためのフラグ

    //プレイヤーに攻撃
    public void AttackToPlayer(bool canAttack)
    {
        if (AttackDelay != true) return;;

        animator.SetTrigger("Attack"); //アニメーションを再生
        StartCoroutine(Interval()); //攻撃待機
    }
    
    //攻撃を待機
    private IEnumerator Interval()
    {
        AttackDelay = false;
       // Debug.Log($"{attackInterval}秒待機");
        yield return new WaitForSeconds(attackInterval);
      //  Debug.Log("攻撃再スタート");
        AttackDelay = true;
    }

    //プレイヤー追跡
    public void ChasePlayer()
    {
        agent.destination = player.transform.position;
    }

    //キャラクターの死亡処理
    public void EnemyDead()
    {
        DropItems(); //アイテムドロップ
        DropExps(); //経験値ドロップ

        animator.SetTrigger("Die"); //アニメーションを再生
        animator.SetBool("Dead",true);

        Destroy(gameObject, 2f); //オブジェクト破壊
    }

    //アイテムをドロップ
    private void DropItems()
    {
        for (int i = 0; i < dropItemValue; i++)
        {
            Vector3 dropItemPosition = transform.position + Vector3.up * 3f;
            GameObject itemPrefab = Instantiate(dropItem, dropItemPosition, Quaternion.identity);
        }
    }

    //経験値をアイテム化してドロップ
    private void DropExps()
    {
        GameObject ExpRed = dropExp[0];
        GameObject ExpYellow = dropExp[1];
        GameObject ExpGreen = dropExp[2];
        Vector3 dropPosition = transform.position + Vector3.up * 3f; //ドロップ位置調整

        //オブジェクト生成
        for (int i = 0; i < dropRedExpValue; i++)
        {
            GameObject eRed = Instantiate(ExpRed, dropPosition, Quaternion.identity);
        }
        for (int i = 0; i < dropYellowExpValue; i++)
        {
            GameObject eYellow = Instantiate(ExpYellow, dropPosition, Quaternion.identity);
        }
        for (int i = 0; i < dropGreenExpValue; i++)
        {
            GameObject eGreen = Instantiate(ExpGreen, dropPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead == true) return;
        ChasePlayer();
        float moveSpeed = agent.velocity.magnitude;
        animator.SetFloat("moveSpeed", moveSpeed);
    }
}
