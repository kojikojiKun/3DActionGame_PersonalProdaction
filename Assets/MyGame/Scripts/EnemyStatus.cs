using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;

public class EnemyStatus : MonoBehaviour
{
    //�Q��
    public EnemyType type;
    [Header("ScriptableObject(EnemyData)��ݒ�")]
    public EnemyData enemyData;
    [Header("�U���͈�")]
    public Collider attackCollider;
    private Animator animator;
    private NavMeshAgent agent;
    public GameObject player;
    private PlayerController playerController;
    private HitAttack hitAttack;

    //�o���l�̃I�u�W�F�N�g
    [SerializeField] private GameObject[] dropExp;

    //�h���b�v����A�C�e���̃I�u�W�F�N�g
    [SerializeField] private GameObject dropItem;
    [SerializeField] private float dropForce; //�A�C�e�����΂���

    public float Dmg; //�v���C���[����󂯂�_���[�W
    private float dmgMultipul; //�v���C���[�̃_���[�W�{��
    public float attackInterval; //�U���Ԋu
    
    //�G�L�����N�^�[�̃X�e�[�^�X
    public float hp;
    public float attackPower;
    public float diffencePower;

    public int dropRedExpValue;
    public int dropYellowExpValue;
    public int dropGreenExpValue;

    public int dropItemValue;
    public bool isBoss;

    private bool isDead=false; //���S�t���O
    private bool isAttacked=false; //�U����t���O

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

    //�X�e�[�^�X��ǂݍ��ݑ������
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
        //�v���C���[�̍U������ɓ������Ƃ�
        if (other.CompareTag("PlayerAttackRange"))
        {
            //�v���C���[����󂯂�_���[�W���v�Z
            dmgMultipul = hitAttack.dmgMultipul;
            float power = playerController.attackPower;
        
            Dmg = Mathf.RoundToInt((power * dmgMultipul) / diffencePower); //int�^�ɕϊ�

            TakeDamageEnemy(Dmg);

           
        }
    }

    //HP�����炷
    public void TakeDamageEnemy(float dmg)
    {
        if (isDead == true) return;
        
        hp -= dmg;
        Debug.LogWarning($"{dmg}�̃_���[�W���󂯂�");

        if (hp <= 0)
        {
            hp = 0;
            isDead = true;
            EnemyDead();
        }
    }

    //�A�j���[�V�����C�x���g�ōU��������o��
    public void SetEnemyAttackCollider()
    {
        if (isDead == true) return; //���S���Ă���Ȃ�U�����Ȃ�

        isAttacked = !isAttacked;

        //�U�����Ă��鎞���������蔻����o��
        if (isAttacked == false)
        {
            attackCollider.enabled = false;
        }
        else
        {
            attackCollider.enabled = true;
        }
    }

    private bool AttackDelay=true; //�U���ҋ@�̂��߂̃t���O

    //�v���C���[�ɍU��
    public void AttackToPlayer(bool canAttack)
    {
        if (AttackDelay != true) return;;

        animator.SetTrigger("Attack"); //�A�j���[�V�������Đ�
        StartCoroutine(Interval()); //�U���ҋ@
    }
    
    //�U����ҋ@
    private IEnumerator Interval()
    {
        AttackDelay = false;
       // Debug.Log($"{attackInterval}�b�ҋ@");
        yield return new WaitForSeconds(attackInterval);
      //  Debug.Log("�U���ăX�^�[�g");
        AttackDelay = true;
    }

    //�v���C���[�ǐ�
    public void ChasePlayer()
    {
        agent.destination = player.transform.position;
    }

    //�L�����N�^�[�̎��S����
    public void EnemyDead()
    {
        DropItems(); //�A�C�e���h���b�v
        DropExps(); //�o���l�h���b�v

        animator.SetTrigger("Die"); //�A�j���[�V�������Đ�
        animator.SetBool("Dead",true);

        Destroy(gameObject, 2f); //�I�u�W�F�N�g�j��
    }

    //�A�C�e�����h���b�v
    private void DropItems()
    {
        for (int i = 0; i < dropItemValue; i++)
        {
            Vector3 dropItemPosition = transform.position + Vector3.up * 3f;
            GameObject itemPrefab = Instantiate(dropItem, dropItemPosition, Quaternion.identity);
        }
    }

    //�o���l���A�C�e�������ăh���b�v
    private void DropExps()
    {
        GameObject ExpRed = dropExp[0];
        GameObject ExpYellow = dropExp[1];
        GameObject ExpGreen = dropExp[2];
        Vector3 dropPosition = transform.position + Vector3.up * 3f; //�h���b�v�ʒu����

        //�I�u�W�F�N�g����
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
