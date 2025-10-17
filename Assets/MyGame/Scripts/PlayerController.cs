using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    GameSceneManager m_gameSceneManger;
    PutTraps m_putTraps;
    [SerializeField] InputActionReference m_playerAction;

    [SerializeField] CharacterController m_character;
    [SerializeField] private float m_jumpForce;
    [SerializeField] private float m_gravity;
    [SerializeField] private float m_fallSpeed;
    [SerializeField] private float m_initFalllSpeed;
    private Transform m_transform;
    private float m_verticalVelocity;
    private float m_turnVelocity;
    private bool m_isGroundPrev;

    private float m_HP;
    private float m_AG;
    private float m_AS;
    private float m_ATK;
    Vector2 m_inputMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //�K�v�ȗv�f�Q��
        m_putTraps = GetComponent<PutTraps>();
        m_gameSceneManger = GameSceneManager.instance;
        m_HP = m_gameSceneManger.getHp;
        m_AG = m_gameSceneManger.getAG;
        m_AS = m_gameSceneManger.getAS;
        m_ATK = m_gameSceneManger.getATK;
        m_transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        var isGrounded = m_character.isGrounded;
        Debug.Log(isGrounded);
        if (isGrounded &&!m_isGroundPrev)
        {
            m_verticalVelocity = -m_initFalllSpeed;
        }else if (!isGrounded)
        {
            m_verticalVelocity-=m_gravity*Time.deltaTime;

            if (m_verticalVelocity < -m_fallSpeed)
            {
                m_verticalVelocity=-m_fallSpeed;
            }
        }
        m_isGroundPrev = isGrounded;

        var moveVelocity = new Vector3(m_inputMove.x * m_AG, m_verticalVelocity, m_inputMove.y * m_AG);
        var moveDelta=moveVelocity*Time.deltaTime;

        m_character.Move(moveDelta);

        if (m_inputMove != Vector2.zero)
        {
            // �ړ����͂�����ꍇ�́A�U�����������s��

            // ������͂���y������̖ڕW�p�x[deg]���v�Z
            var targetAngleY = -Mathf.Atan2(m_inputMove.y, m_inputMove.x)
                * Mathf.Rad2Deg + 90;

            // �C�[�W���O���Ȃ��玟�̉�]�p�x[deg]���v�Z
            var angleY = Mathf.SmoothDampAngle(
                m_transform.eulerAngles.y,
                targetAngleY,
            ref m_turnVelocity,
                0.1f
            );

            // �I�u�W�F�N�g�̉�]���X�V
            m_transform.rotation = Quaternion.Euler(0, angleY, 0);
        }
    }

    private void OnEnable()
    {
        //InputActionMap�L����.
        m_playerAction.action.actionMap.Enable();
    }

    private void OnDisable()
    {
        //InputActionMap������.
        m_playerAction.action.actionMap.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        m_inputMove = context.ReadValue<Vector2>();

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && m_character.isGrounded)
        {
            Debug.Log("jumping");
            m_verticalVelocity=m_jumpForce;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("attakicng");
        }
    }

    public void OnChangeMode(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            bool isWaveFinished = m_gameSceneManger.IsWaveFinished();
            m_putTraps.ModeChange(isWaveFinished);
        }
    }
}
