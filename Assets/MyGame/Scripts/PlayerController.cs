using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameManager m_gameManger;
    [SerializeField] InputActionReference m_playerAction;
    private float HP;
    private float AG;
    private float AS;
    private float ATK;

    private void OnEnable()
    {
        m_playerAction.action.actionMap.Enable();
    }

    private void OnDisable()
    {
        m_playerAction.action.actionMap.Disable();
    }

    public void OnMove()
    {
        
    }

    public void OnJump()
    {

    }

    public void OnAttack()
    {

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HP = m_gameManger.getHp;
        AG = m_gameManger.getAG;
        AS = m_gameManger.getAS;
        ATK= m_gameManger.getATK;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
