using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PutTraps : MonoBehaviour
{
    //InputSystems
    [SerializeField] InputActionReference trapAction;
    //[SerializeField] InputActionReference rotateTrap;

    [SerializeField] GameManager m_gameManager;
    [SerializeField] PoolManager m_poolManager;
    private PlayerController m_playerController;
    private string[] m_trapName = new string[4] { "Fire", "CrossBow", "Blade", "Spike" };
    [SerializeField] GameObject[] m_previewTrapPrefab; //�g���b�v�̐ݒu�v���r���[�p�I�u�W�F�N�g
    [SerializeField] Material m_prevMaterial;
    private GameObject[] m_prevInstance;
    private Collider[] m_prevCollider;
    private Transform m_lastPreviewPos; //�v���r���[�̍ŏI���W
    [SerializeField] private float m_rotateSpeed;
    [SerializeField] private float m_maxPlaceDistance; //�g���b�v��ݒu�\�ȃv���C���[�Ƃ̋���
    [SerializeField] LayerMask m_fieldMask; //�g���b�v�ݒu�\�ȃ��C���[
    public bool m_buildMode = true; //�g���b�v�ݒu���[�h�t���O
    private bool m_decided; //�g���b�v�̎�ޑI�������t���O
    private bool m_canPlace; //�g���b�v�ݒu�\�t���O
    private bool m_isRotating;
    private bool m_isCollision;
    private Vector2 m_rotateDir;
    private int m_trapIndex = 0;
    private float m_nextAllowTime = 0f;
    private float m_scrollCoolDown = 0.1f; //�}�E�X�~�h���{�^���̃X�N���[���N�[���_�E��
    private bool m_isEnabled;
    private bool m_isDisabled;

    private void OnEnable()
    {
        trapAction.action.actionMap.Enable();
    }
    private void OnDisable()
    {
        trapAction.action.actionMap.Disable();
    }

    //�}�E�X�~�h���{�^���X�N���[���Ńg���b�v�̎�ޑI��
    public void OnSelectTrap(InputAction.CallbackContext context)
    {
        if (Time.time < m_nextAllowTime || m_decided == true) return;
        Vector2 scrollValue = context.ReadValue<Vector2>();
        SelectTrap(scrollValue);
    }

    //�~�h���{�^���������݂Ńg���b�v�̎�ތ���
    public void OnDecideTrap(InputAction.CallbackContext context)
    {
        if (m_prevInstance == null || m_prevInstance.Length == 0) { return; }
            
        if (context.phase == InputActionPhase.Started)
        {
            if (m_decided == false)
            {
                m_decided = true; //�g���b�v�I������
            }
            else
            {
                m_decided = false; //�g���b�v���I��
            }

            if (m_prevInstance[m_trapIndex] != null)
            {
                Debug.Log("prev is not null");
            }

            m_prevInstance[m_trapIndex].SetActive(true);
            Debug.Log(m_decided);
        }
    }

    public void OnRotateTrap(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            m_isRotating = true;
            m_rotateDir = context.ReadValue<Vector2>();
        }
        else
        {
            m_isRotating = false;
        }
    }

    void RotateTrap()
    {
        if (m_decided == true)
        {
            GameObject prev = m_prevInstance[m_trapIndex];
            if (m_rotateDir.x > 0.1f)
            {
                prev.transform.Rotate(0, m_rotateSpeed * Time.deltaTime, 0);
                Debug.Log(prev.transform.localRotation);

            }
            else if (m_rotateDir.x < -0.1f)
            {
                prev.transform.Rotate(0, -m_rotateSpeed * Time.deltaTime, 0);
                Debug.Log(prev.transform.localRotation);
            }
        }
    }

    //�X�N���[���̒l����g���b�v�̎�ޑI��
    void SelectTrap(Vector2 scrollValue)
    {
        if (scrollValue.y > 0.1f)
        {
            m_trapIndex++;
            Debug.Log(m_trapIndex);
            m_nextAllowTime = Time.time + m_scrollCoolDown;
            if (m_trapIndex > m_prevInstance.Length-1)
            {
                m_trapIndex = 0;
            }

            //���݂̃g���b�v�̈�O�̃v���r���[���\��
            if (m_trapIndex == 0)
            {
                m_prevInstance[m_prevInstance.Length - 1].SetActive(false);
            }
            else
            {
                m_prevInstance[m_trapIndex - 1].SetActive(false);
            }

            m_prevInstance[m_trapIndex].SetActive(true); //���݂̃g���b�v�̃v���r���[��\��
        }
        else if (scrollValue.y < -0.1f)
        {
            m_trapIndex--;
            Debug.Log(m_trapIndex);
            m_nextAllowTime = Time.time + m_scrollCoolDown;
            if (m_trapIndex < 0)
            {
                m_trapIndex = m_prevInstance.Length - 1;
            }

            //���݂̃g���b�v�̈�O�̃v���r���[���\��
            if (m_trapIndex == m_prevInstance.Length - 1)
            {
                m_prevInstance[0].SetActive(false);
            }
            else
            {
                m_prevInstance[m_trapIndex + 1].SetActive(false);
            }

            m_prevInstance[m_trapIndex].SetActive(true);//���݂̃g���b�v�̃v���r���[��\��

        }
    }

    //�J�[�\���ړ��Ńg���b�v�̐ݒu�ʒu�����߂�
    public void OnMoveTrap(InputAction.CallbackContext context)
    {
        if (m_prevInstance == null || m_prevInstance.Length == 0) {  return; }
        Vector2 pos = Mouse.current.position.ReadValue(); //�}�E�X�J�[�\���̍��W���擾
        Vector3 origin = m_playerController.transform.position; //�v���C���[�̍��W
        Ray ray = Camera.main.ScreenPointToRay(pos); //�J��������ray����
        if (Physics.Raycast(ray, out RaycastHit hit, 500, m_fieldMask)) //�n�ʂ�ray�q�b�g
        {
            Vector3 hitPos = hit.point; //�}�E�X�̍��W��3D�ɕϊ�

            m_prevInstance[m_trapIndex].transform.position = hitPos; //�v���r���[�\��

            float distance = Vector3.Distance(origin, hitPos); //�}�E�X�ʒu�ƃv���C���[�̋����v�Z
            //�v���C���[�Ƃ̋������ݒu�\�����ȉ�
            if (distance <= m_maxPlaceDistance)
            {
                m_canPlace = true; //�ݒu�\
            }
            else
            {
                m_canPlace = false; //�ݒu�s��
            }
            TryPlaceTrap(hitPos);
        }

    }


    //E�L�[�Ńg���b�v�̐ݒu�ʒu������
    public void OnPlaceTrap(InputAction.CallbackContext context)
    {
        //�g���b�v�I���ς݂��ݒu�\�ȏꏊ�Ȃ�g���b�v�ݒu
        if (m_decided == true && m_canPlace == true)
        {
            if (context.phase == InputActionPhase.Started)
            {
                SucceedPlaceTrap();
            }
        }
    }

    //�v���r���[���ق��̃I�u�W�F�N�g�ƏՓ˂��Ă���ΐݒu����
    public void TriggerCheckResult(bool collision)
    {
        m_isCollision = collision;
    }

    void TryPlaceTrap(Vector3 hitPos)
    {

        m_lastPreviewPos = m_prevInstance[m_trapIndex].transform;
        m_lastPreviewPos.localRotation = m_prevInstance[m_trapIndex].transform.localRotation;
        if (m_prevMaterial != null)
        {
            m_prevInstance[m_trapIndex].transform.position = hitPos;
            //Color��ݒu�\�Ȃ�΁A�s�\�Ȃ�ԂɕύX
            m_prevMaterial.color = m_canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
        }
        else
        {
            Debug.LogWarning("prevMaterial is null");
        }
    }

    //�g���b�v��ݒu����
    void SucceedPlaceTrap()
    {
        GameObject trap = m_poolManager.GetTrap(m_trapName[m_trapIndex]); //�g���b�v���v�[��������o��
        m_prevInstance[m_trapIndex].SetActive(false); //�v���r���[��\��
        trap.SetActive(true);
        trap.transform.rotation = m_lastPreviewPos.localRotation;
        trap.transform.position = m_lastPreviewPos.position; //�g���b�v���v���r���[�̈ʒu�ɐݒu
        m_decided = false;
    }

    //�g���b�v�ݒu���[�h��؂�ւ�
    public void ModeChange(bool canChange)
    {
        Debug.Log($"length={m_prevInstance?.Length}");
        
        if (canChange == true)
        {
            m_buildMode = !m_buildMode;

            if (m_buildMode == true)
            {
                m_prevInstance[m_trapIndex].SetActive(true);
            }
            else
            {
                m_prevInstance[m_trapIndex].SetActive(false);
            }
        }
        else
        {
            m_buildMode = false;
            m_prevInstance[m_trapIndex].SetActive(false);
        }
        Debug.Log(m_buildMode);
    }

    public void ReceivePlayer(PlayerController player)
    {if (player == null)
        {
            Debug.Log("null!!!");
        }
        else
        {
            Debug.Log("not null!!!");
        }
        m_playerController = player;
        Debug.Log("setPlayer");
    }

    private void Start()
    {
        m_prevInstance = new GameObject[m_previewTrapPrefab.Length];
        m_prevCollider = new Collider[m_previewTrapPrefab.Length];
        for (int i = 0; i < m_previewTrapPrefab.Length; i++)
        {
            GameObject prev=Instantiate(m_previewTrapPrefab[i]);
            m_prevInstance[i] = prev;
            m_prevCollider[i] = m_prevInstance[i].GetComponent<Collider>();
            m_prevInstance[i].SetActive(false);
        }
        
    }

    private void Update()
    {Debug.Log($"trapIndex{m_trapIndex}");
        Debug.Log(m_gameManager.IsWaveFinished());
        
        if (m_isRotating == true)
        {
            RotateTrap();
        }

        if (m_isCollision == true)
        {
            m_canPlace = false;
        }
    }
}
