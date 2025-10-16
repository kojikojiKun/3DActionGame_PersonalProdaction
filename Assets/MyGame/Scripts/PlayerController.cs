using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    GameSceneManager m_gameSceneManger;
    PutTraps m_putTraps;
    [SerializeField] InputActionReference m_playerAction;
    private float m_HP;
    private float m_AG;
    private float m_AS;
    private float m_ATK;
    private bool m_isMove;
    Vector2 m_moveInput;

    private void OnEnable()
    {
        m_playerAction.action.actionMap.Enable();
    }

    private void OnDisable()
    {
        m_playerAction.action.actionMap.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            m_isMove = true;
            m_moveInput=context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            m_isMove = false;
        }
    }

    void Moving()
    {
        Debug.Log(m_moveInput.sqrMagnitude);
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("jumping");
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_putTraps=GetComponent<PutTraps>();
        m_gameSceneManger=GameSceneManager.instance;
        m_HP = m_gameSceneManger.getHp;
        m_AG = m_gameSceneManger.getAG;
        m_AS = m_gameSceneManger.getAS;
        m_ATK= m_gameSceneManger.getATK;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isMove)
        {
            Moving();
        }
    }
}
