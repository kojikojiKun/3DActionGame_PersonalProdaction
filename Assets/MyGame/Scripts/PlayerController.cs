using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    GameSceneManager m_gameSceneManger;
    PutTraps m_putTraps;
    [SerializeField] Camera m_camera;
    private CharacterController m_character;
    private Animator m_animator;
    private Vector3 m_prevPos;
    [SerializeField] private float m_jumpForce;
    [SerializeField] private float m_gravity;
    [SerializeField] private float m_fallSpeed;
    [SerializeField] private float m_initFallSpeed;
    private float m_verticalVelocity;
    private bool m_isGroundPrev;

    private float m_HP;
    private float m_AG;
    private float m_AS;
    private float m_ATK;
    Vector2 m_inputMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //必要な要素参照
        m_putTraps = GetComponent<PutTraps>();
        m_character = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_gameSceneManger = GameSceneManager.instance;
        m_HP = m_gameSceneManger.getHp;
        m_AG = m_gameSceneManger.getAG;
        m_AS = m_gameSceneManger.getAS;
        m_ATK = m_gameSceneManger.getATK;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }



    public void OnMove(InputAction.CallbackContext context)
    {

        m_inputMove = context.ReadValue<Vector2>();
        // Debug.Log(m_inputMove);
    }

    bool IsGroundedRay()
    {
        float radius = 0.3f;
        float distance = 0.1f;
        return Physics.SphereCast(transform.position, radius, Vector3.down, out _, distance);
    }

    //プレイヤーを移動させる.
    void MovePlayer()
    {
        bool isGrounded = m_character.isGrounded || IsGroundedRay();
        Debug.Log($"character={m_character.isGrounded},Ray={IsGroundedRay()}");

        if (isGrounded)
        {
            if (!m_isGroundPrev)
                m_verticalVelocity = 0f; // 着地時リセット

            if (m_verticalVelocity < 0)
                m_verticalVelocity = -1f; // 安定化
        }
        else
        {
            m_verticalVelocity -= m_gravity * Time.deltaTime;
            if (m_verticalVelocity < -m_fallSpeed)
                m_verticalVelocity = -m_fallSpeed;
        }

        m_isGroundPrev = isGrounded;

        Vector3 camForward = m_camera.transform.forward;
        Vector3 camRight = m_camera.transform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = (camForward.normalized * m_inputMove.y + camRight.normalized * m_inputMove.x).normalized;
        Vector3 moveVelocity = moveDir * m_AG;

        Vector3 moveDelta = (moveVelocity + Vector3.up * m_verticalVelocity) * Time.deltaTime;


        m_character.Move(moveDelta);

        //移動スピードを計算 .
        Vector3 delta = m_character.transform.position - m_prevPos;
        float moveSpeed = delta.magnitude / Time.deltaTime;
        m_prevPos = transform.position;

        m_animator.SetFloat("moveSpeed", moveSpeed);
        m_animator.SetFloat("moveX", m_inputMove.x);
        m_animator.SetFloat("moveY", m_inputMove.y);
    }

    //ジャンプボタン入力を検知.
    public void OnJump(InputAction.CallbackContext context)
    {
        //入力した瞬間かつプレイヤーが接地している.
        if (context.phase == InputActionPhase.Started)
        {
            if (m_character.isGrounded || IsGroundedRay())
                Debug.Log("jumping");
            m_verticalVelocity = m_jumpForce;
        }
    }

    //攻撃ボタン入力を検知.
    public void OnAttack(InputAction.CallbackContext context)
    {
        //入力した瞬間だけ.
        if (context.phase == InputActionPhase.Started)
        {
            m_animator.SetTrigger("attack");
        }
    }

    //トラップを置く状態と置かない状態を切り替え.
    public void OnChangeMode(InputAction.CallbackContext context)
    {
        //入力した瞬間だけ.
        if (context.phase == InputActionPhase.Started)
        {
            bool isWaveFinished = m_gameSceneManger.IsWaveFinished();
            m_putTraps.ModeChange(isWaveFinished);
        }
    }
}
