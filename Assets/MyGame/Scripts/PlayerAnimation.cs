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

    [Header("不自然なアニメーションを調整するためのboneのTransform")]
    public Transform armRBone;
    public Transform sholderLBone;
    public Transform bodyBone;

    [Header("不自然なアニメーションを修正するためのパラメータ")]
    [Header("右腕の回転する量")]
    public float lotateRArmAngleX;
    public float lotateRArmAngleY;
    public float lotateRArmAngleZ;

    [Header("胴体の回転する量")]
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

    //移動アニメーションを再生
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

    //ジャンプアニメーション
    public void JumpAnimation()
    {
        animator.Play("Jump", 0, 0);
    }

    //着地アニメーション
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

    //コンボ攻撃のアニメーションを再生
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

    //ガードアニメーションを再生
    public void GuardAnimation( bool isGuard)
    {
        animator.SetBool("Guard", isGuard);
    }

    //ダメージを受けるアニメーションを再生
    public void TakeDamageAnimation()
    {
        animator.SetTrigger("Damaged");
    }

    //プレイヤーの死亡アニメーションを再生
    public void PlayerDied()
    {
        animator.SetBool("PlayerDied", true);
        animator.SetTrigger("Died");
    }

    void LateUpdate()
    {
        //不自然なアニメーションを調整
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        AnimatorStateInfo leftArmLayer = animator.GetCurrentAnimatorStateInfo(2);

        bool isAttackState = stateInfo.IsName("Attack");
        bool isGuardState = leftArmLayer.IsName("Guard");

        if (isAttackState == true && isMoving == true)
        {
            armRBone.localRotation = Quaternion.Euler(lotateRArmAngleX, lotateRArmAngleY, lotateRArmAngleZ);
            //Debug.Log("回転");
        }

        if (V2MoveX == 0 && V2MoveY == -1)
        {
            bodyBone.localRotation = Quaternion.Euler(lotateBodyAngleX, lotateBodyAngleY, lotateBodyAngleZ);
        }
    }

    private void FixedUpdate()
    {   
        //移動速度計算
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
