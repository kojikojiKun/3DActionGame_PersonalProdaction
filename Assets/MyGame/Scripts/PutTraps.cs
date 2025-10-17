using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class PutTraps : MonoBehaviour
{
    //InputSystems
    [SerializeField] InputActionReference trapAction;

    private GameSceneManager m_gameSceneManager;
    private PoolManager m_poolManager;
    private PlayerController m_playerController;
    [SerializeField] GameObject[] m_previewTrapPrefab; //�g���b�v�̐ݒu�v���r���[�p�I�u�W�F�N�g.
    [SerializeField] Material m_prevMaterial; //�v���r���[�̐F��ύX���邽�߂̃}�e���A��.
    private GameObject[] m_prevInstance; //���������v���r���[�I�u�W�F�N�g���i�[���邽�߂̕ϐ�.
    private Transform m_lastPreviewPos; //�v���r���[�̍ŏI���W.

    private string[] m_trapName = new string[4] { "Fire", "CrossBow", "Blade", "Spike" }; //�v�[��������o���g���b�v�̖��O.
    [SerializeField] private float m_rotateSpeed; //�v���r���[����]�����鑬�x.
    [SerializeField] private float m_maxPlaceDistance; //�g���b�v��ݒu�\�ȃv���C���[�Ƃ̋���.
    [SerializeField] LayerMask m_fieldMask; //�g���b�v�ݒu�\�ȃ��C���[.
    public bool m_buildMode = true; //�g���b�v�ݒu���[�h�t���O.
    private bool m_decided; //�g���b�v�̎�ޑI�������t���O.
    private bool m_inRange; //�ݒu�\�����t���O.
    private bool m_collision; //�v���r���[���ق��̃I�u�W�F�N�g�ɐG��Ă��邩�̃t���O.
    private bool m_canPlace; //�g���b�v�ݒu�\�t���O.
    private bool m_isRotating; //��]������t���O.
    private Vector2 m_rotateDir; //��]����.
    private int m_trapIndex = 0; //�I�𒆂̃g���b�v.
    private float m_nextAllowTime = 0f;
    private float m_scrollCoolDown = 0.1f; //�}�E�X�~�h���{�^���̃X�N���[���N�[���_�E��.

    private void Start()
    {
        //�K�v�ȗv�f���Q��.
        m_gameSceneManager = GameSceneManager.instance;
        m_poolManager = m_gameSceneManager.GetPoolManager;
        m_playerController = m_gameSceneManager.GetPlayer;
        m_prevInstance = new GameObject[m_previewTrapPrefab.Length];
        for (int i = 0; i < m_previewTrapPrefab.Length; i++)
        {
            GameObject prev = Instantiate(m_previewTrapPrefab[i]);
            m_prevInstance[i] = prev;
            m_prevInstance[i].SetActive(false);
        }

    }

    private void Update()
    {
        if (m_isRotating == true)
        {
            RotateTrap();
        }

        //�v���r���[���ݒu�\���������ق��̃I�u�W�F�N�g�ɐG��Ă��Ȃ�.
        if (m_inRange == true && m_collision == false)
        {
            m_canPlace = true; //�ݒu�\.
        }
        else if (m_inRange == false || m_collision == true)
        {
            m_canPlace = false; //�ݒu�s��.
        }
    }

    private void OnEnable()
    {
        //InputActionMap�L����.
        trapAction.action.actionMap.Enable();
    }
    private void OnDisable()
    {
        //InputActionMap������.
        trapAction.action.actionMap.Disable();
    }

    //�}�E�X�~�h���{�^���X�N���[���Ńg���b�v�̎�ޑI��.
    public void OnSelectTrap(InputAction.CallbackContext context)
    {
        //�X�N���[�����Ԃ��N�[���_�E���ȓ��܂��̓g���b�v�̎�ޖ��I��.
        if (Time.time < m_nextAllowTime || m_decided == true) return;

        Vector2 scrollValue = context.ReadValue<Vector2>();
        SelectTrap(scrollValue);
    }

    //�X�N���[���̒l����g���b�v�̎�ޑI��.
    void SelectTrap(Vector2 scrollValue)
    {
        //��ɃX�N���[��.
        if (scrollValue.y > 0.1f)
        {
            m_trapIndex++;
            m_nextAllowTime = Time.time + m_scrollCoolDown;

            //m_trapIndex�͈̔͐���.
            if (m_trapIndex > m_prevInstance.Length - 1)
            {
                m_trapIndex = 0;
            }

            //���݂̃g���b�v�̈�O�̃v���r���[���\��.
            if (m_trapIndex == 0)
            {
                m_prevInstance[m_prevInstance.Length - 1].SetActive(false);
            }
            else
            {
                m_prevInstance[m_trapIndex - 1].SetActive(false);
            }

            m_prevInstance[m_trapIndex].SetActive(true); //���݂̃g���b�v�̃v���r���[��\��.
        }

        //���ɃX�N���[��.
        else if (scrollValue.y < -0.1f)
        {
            m_trapIndex--;
            m_nextAllowTime = Time.time + m_scrollCoolDown;

            //m_trapIndex�͈̔͐���.
            if (m_trapIndex < 0)
            {
                m_trapIndex = m_prevInstance.Length - 1;
            }

            //���݂̃g���b�v�̈�O�̃v���r���[���\��.
            if (m_trapIndex == m_prevInstance.Length - 1)
            {
                m_prevInstance[0].SetActive(false);
            }
            else
            {
                m_prevInstance[m_trapIndex + 1].SetActive(false);
            }

            m_prevInstance[m_trapIndex].SetActive(true);//���݂̃g���b�v�̃v���r���[��\��.

        }
    }

    //�~�h���{�^���������݂Ńg���b�v�̎�ތ���.
    public void OnDecideTrap(InputAction.CallbackContext context)
    {
        if (m_prevInstance == null || m_prevInstance.Length == 0) return;

        //�{�^���������ꂽ�u�Ԃ������s
        if (context.phase == InputActionPhase.Started)
        {
            if (m_decided == false)
            {
                m_decided = true; //�g���b�v�I������.
            }
            else
            {
                m_decided = false; //�g���b�v���I��.
            }

            m_prevInstance[m_trapIndex].SetActive(true); //�v���r���[�\��.
        }
    }

    //�v���r���[����]������
    public void OnRotateTrap(InputAction.CallbackContext context)
    {
        //�{�^�������������Ă���Ԏ��s.
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

    //�J�[�\���ړ��Ńg���b�v�̐ݒu�ʒu�����߂�.
    public void OnMoveTrap(InputAction.CallbackContext context)
    {
        if (m_prevInstance == null || m_prevInstance.Length == 0) { return; }

        Vector2 pos = Mouse.current.position.ReadValue(); //�}�E�X�J�[�\���̍��W���擾.
        Vector3 origin = m_playerController.transform.position; //�v���C���[�̍��W.
        Ray ray = Camera.main.ScreenPointToRay(pos); //�J��������ray����.

        if (Physics.Raycast(ray, out RaycastHit hit, 500, m_fieldMask)) //�n�ʂ�ray�q�b�g.
        {
            Vector3 hitPos = hit.point; //�}�E�X�̍��W��3D�ɕϊ�.
            float distance = Vector3.Distance(origin, hitPos); //�}�E�X�ʒu�ƃv���C���[�̋����v�Z.
            m_prevInstance[m_trapIndex].transform.position = hitPos; //�v���r���[�ړ�.

            //�v���C���[�Ƃ̋������ݒu�\�����ȉ�.
            if (distance <= m_maxPlaceDistance)
            {
                m_inRange = true; //�ݒu�\.
            }
            else
            {
                m_inRange = false; //�ݒu�s��.
            }
            TryPlaceTrap(hitPos);
        }
    }

    //�g���b�v���ݒu�\�����f.
    void TryPlaceTrap(Vector3 hitPos)
    {

        m_lastPreviewPos = m_prevInstance[m_trapIndex].transform; //�v���r���[�̍ŏI���W�ۑ�.
        m_lastPreviewPos.localRotation = m_prevInstance[m_trapIndex].transform.localRotation; //�v���r���[�̉�]�ʕۑ�.

        //�}�e���A����null�`�F�b�N.
        if (m_prevMaterial != null)
        {
            //Color��ݒu�\�Ȃ�΁A�s�\�Ȃ�ԂɕύX.
            m_prevMaterial.color = m_canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
        }
        else
        {
            Debug.LogWarning("prevMaterial is null");
        }
    }

    //E�L�[�Ńg���b�v�̐ݒu�ʒu������.
    public void OnPlaceTrap(InputAction.CallbackContext context)
    {
        //�g���b�v�I���ς݂��ݒu�\�ȏꏊ�Ȃ�g���b�v�ݒu.
        if (m_decided == true && m_canPlace == true)
        {
            if (context.phase == InputActionPhase.Started)
            {
                SucceedPlaceTrap();
            }
        }
    }

    //�g���b�v��ݒu����
    void SucceedPlaceTrap()
    {
        GameObject trap = m_poolManager.GetTrap(m_trapName[m_trapIndex]); //�g���b�v���v�[��������o��.
        m_prevInstance[m_trapIndex].SetActive(false); //�v���r���[��\��.

        trap.transform.rotation = m_lastPreviewPos.localRotation; //�v���r���[�̉�]���g���b�v�ɔ��f.
        trap.transform.position = m_lastPreviewPos.transform.position; //�g���b�v���v���r���[�̈ʒu�ɐݒu.
        Debug.Log($"prevPosition={m_lastPreviewPos.transform.position},trapPosition={trap.transform.position}");
        m_decided = false; //�g���b�v�𖢑I���ɂ���.
    }

    //�v���r���[����]������.
    void RotateTrap()
    {
        //�g���b�v�̎�ނ�����ς�.
        if (m_decided == true)
        {
            GameObject prev = m_prevInstance[m_trapIndex];

            //�E��].
            if (m_rotateDir.x > 0.1f)
            {
                prev.transform.Rotate(0, m_rotateSpeed * Time.deltaTime, 0);
            }
            //����].
            else if (m_rotateDir.x < -0.1f)
            {
                prev.transform.Rotate(0, -m_rotateSpeed * Time.deltaTime, 0);
            }
        }
    }

    //�v���r���[���ق��̃I�u�W�F�N�g�ƏՓ˂��Ă���ΐݒu�s��.
    public void TriggerCheckResult(bool collision)
    {
        m_collision = collision; //�Փ˂��Ă����true.
    }

    //�g���b�v�ݒu���[�h��؂�ւ�.
    public void ModeChange(bool canChange)
    {
        //�؂�ւ��\.
        if (canChange == true)
        {
            m_buildMode = !m_buildMode; //���[�h�ؑ�.

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
    }
}
