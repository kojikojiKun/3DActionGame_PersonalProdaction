using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private PlayerController controller;
    private CharacterController character;

    [Header("�s���R�ȃA�j���[�V�����𒲐����邽�߂�bone��Transform")]
    public Transform armRBone;
    public Transform sholderLBone;
    public Transform bodyBone;

    [Header("�s���R�ȃA�j���[�V�������C�����邽�߂̃p�����[�^")]
    [Header("�E�r�̉�]�����")]
    public float lotateRArmAngleX;
    public float lotateRArmAngleY;
    public float lotateRArmAngleZ;

    [Header("���̂̉�]�����")]
    public float lotateBodyAngleX;
    public float lotateBodyAngleY;
    public float lotateBodyAngleZ;

    public bool isMoving;
    private bool isLanded = true;

    private float moveSpeed;
    private float V2MoveX;
    private float V2MoveY;
    private Vector3 lastPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        character = GetComponent<CharacterController>();
    }

    //�ړ��A�j���[�V�������Đ�
    void moveAnimation()
    {
        Vector2 input = new Vector2(V2MoveX, V2MoveY).normalized;

        isMoving = moveSpeed > 0.5f;
        animator.SetBool("IsJumping",!controller.isGroundedPlayer);
        animator.SetBool("IsMove", isMoving);
        animator.SetFloat("moveSpeed", moveSpeed);
        animator.SetFloat("moveX", input.x);
        animator.SetFloat("moveY", input.y);
    }

    //�W�����v�A�j���[�V����
    public void JumpAnimation()
    {
        animator.Play("Jump", 0, 0);
    }

    //���n�A�j���[�V����
    private void LandedAnimation()
    {
        if (controller.isGroundedPlayer == false && isLanded==true)
        {
            isLanded = false;
        }

        if(isLanded==false && controller.isGroundedPlayer == true)
        {
            animator.Play("Land", 0, 0);
            isLanded = true;
        }
    }

    //�R���{�U���̃A�j���[�V�������Đ�
    public void AttackAnimation()
    {
            switch (controller.comboState)
            {
                case PlayerController.ComboState.Combo1:
                    animator.SetTrigger("Attack");
                    comboFinished = false;
                    break;
                case PlayerController.ComboState.Combo2:
                    animator.SetTrigger("Attack1");
                    comboFinished = false;
                    break;
                case PlayerController.ComboState.Combo3:
                    animator.SetTrigger("Attack2");
                    comboFinished = false;
                    break;
            }
    }

    public bool comboFinished=true;
    public void AttackAnimationFinished()
    {
        comboFinished = true;
    }

    //�K�[�h�A�j���[�V�������Đ�
    public void GuardAnimation( bool isGuard)
    {
        animator.SetBool("Guard", isGuard);
    }

    //�_���[�W���󂯂�A�j���[�V�������Đ�
    public void TakeDamageAnimation()
    {
        animator.SetTrigger("Damaged");
    }

    //�v���C���[�̎��S�A�j���[�V�������Đ�
    public void PlayerDied()
    {
        animator.SetBool("PlayerDied", true);
        animator.SetTrigger("Died");
    }

    void LateUpdate()
    {
        //�s���R�ȃA�j���[�V�����𒲐�
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        AnimatorStateInfo leftArmLayer = animator.GetCurrentAnimatorStateInfo(2);

        bool isAttackState = stateInfo.IsName("Attack");
        bool isGuardState = leftArmLayer.IsName("Guard");

        if (isAttackState == true && isMoving == true)
        {
            armRBone.localRotation = Quaternion.Euler(lotateRArmAngleX, lotateRArmAngleY, lotateRArmAngleZ);
            //Debug.Log("��]");
        }

        if (V2MoveX == 0 && V2MoveY == -1)
        {
            bodyBone.localRotation = Quaternion.Euler(lotateBodyAngleX, lotateBodyAngleY, lotateBodyAngleZ);
        }
    }

    private void FixedUpdate()
    {   
        //�ړ����x�v�Z
        var currentPosition = transform.position;
        var deltaPosition = currentPosition - lastPosition;
        moveSpeed = deltaPosition.magnitude / Time.fixedDeltaTime;
        lastPosition = currentPosition;

        V2MoveX = controller.inputMove.x;
        V2MoveY = controller.inputMove.y;
    }

    // Update is called once per frame
    void Update()
    {
        moveAnimation();
        LandedAnimation();
    }
}
