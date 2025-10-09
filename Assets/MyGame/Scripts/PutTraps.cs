using UnityEngine;
using UnityEngine.InputSystem;

public class PutTraps : MonoBehaviour
{
    //InputSystems
    [SerializeField] InputActionReference clickAction;
    [SerializeField] InputActionReference scrollAction;
    [SerializeField] InputActionReference mousePosition;
    [SerializeField] InputActionReference buttonDown;
    [SerializeField] InputActionReference rotateTrap;

    [SerializeField] GameObject player;
    [SerializeField] ObjectPool[] trapPools; // �g���b�v�p�v�[��
    [SerializeField] GameObject[] previewTrapPrefab; //�g���b�v�̐ݒu�v���r���[�p�I�u�W�F�N�g
    [SerializeField] Material prevMaterial;
    private GameObject[] prevInstance;
    private Collider[] prevCollider;
    private Transform lastPreviewPos; //�v���r���[�̍ŏI���W
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float maxPlaceDistance; //�g���b�v��ݒu�\�ȃv���C���[�Ƃ̋���
    [SerializeField] LayerMask fieldMask; //�g���b�v�ݒu�\�ȃ��C���[
    public bool waveFinished = true; //�E�F�[�u�I���t���O
    public bool buildMode = true; //�g���b�v�ݒu���[�h�t���O
    private bool decided; //�g���b�v�̎�ޑI�������t���O
    private bool canPlace; //�g���b�v�ݒu�\�t���O
    private bool isRotating;
    private bool isCollision;
    private Vector2 rotateDir;
    private int trapIndex = 0;
    private float nextAllowTime = 0f;
    private float scrollCoolDown = 0.1f; //�}�E�X�~�h���{�^���̃X�N���[���N�[���_�E��

    private void OnEnable()
    {
        clickAction.action.Enable();
        scrollAction.action.Enable();
        mousePosition.action.Enable();
        buttonDown.action.Enable();
        rotateTrap.action.Enable();
        rotateTrap.action.performed += OnRotateTrap;
        rotateTrap.action.canceled += OnRotateTrap;
    }
    void OnDisable()
    {
        clickAction.action.Disable();
        scrollAction.action.Disable();
        mousePosition.action.Disable();
        buttonDown.action.Disable();
        rotateTrap.action.Disable();
        rotateTrap.action.performed -= OnRotateTrap;
        rotateTrap.action.canceled -= OnRotateTrap;
    }

    //�}�E�X�~�h���{�^���X�N���[���Ńg���b�v�̎�ޑI��
    public void OnSelectTrap(InputAction.CallbackContext context)
    {
        if (Time.time < nextAllowTime || decided == true) return;
        Vector2 scrollValue = context.ReadValue<Vector2>();
        SelectTrap(scrollValue);
    }

    //�~�h���{�^���������݂Ńg���b�v�̎�ތ���
    public void OnDecideTrap(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (decided == false)
            {
                decided = true; //�g���b�v�I������
            }
            else
            {
                decided = false; //�g���b�v���I��
            }
            if (prevInstance[trapIndex] != null)
            {
                Debug.Log("prev is not null");
            }
            Debug.Log(trapIndex);
            if (decided == true)
            {
                prevInstance[trapIndex].SetActive(true);
                Debug.Log($"{prevInstance[trapIndex]}active");
            }
            else
            {
                prevInstance[trapIndex].SetActive(false);
                Debug.Log($"{prevInstance[trapIndex]}invilid");
            }
        }
    }

    public void OnRotateTrap(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            isRotating = true;
            rotateDir = context.ReadValue<Vector2>();
        }
        else
        {
            isRotating = false;
        }
    }

    void RotateTrap()
    {
        if (decided == true)
        {
            GameObject prev = prevInstance[trapIndex];
            if (rotateDir.x > 0.1f)
            {
                prev.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
                Debug.Log(prev.transform.localRotation);

            }
            else if (rotateDir.x < -0.1f)
            {
                prev.transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
                Debug.Log(prev.transform.localRotation);
            }
        }
    }

    //�X�N���[���̒l����g���b�v�̎�ޑI��
    void SelectTrap(Vector2 scrollValue)
    {
        if (scrollValue.y > 0.1f)
        {
            trapIndex++;
            nextAllowTime = Time.time + scrollCoolDown;
            if (trapIndex >= trapPools.Length)
            {
                trapIndex = 0;
            }
        }
        else if (scrollValue.y < -0.1f)
        {
            trapIndex--;
            nextAllowTime = Time.time + scrollCoolDown;
            if (trapIndex < 0)
            {
                trapIndex = trapPools.Length - 1;
            }
        }
    }

    //�J�[�\���ړ��Ńg���b�v�̐ݒu�ʒu�����߂�
    public void OnMoveTrap(InputAction.CallbackContext context)
    {
        Vector2 pos = Mouse.current.position.ReadValue(); //�}�E�X�J�[�\���̍��W���擾
        Vector3 origin = player.transform.position; //�v���C���[�̍��W
        Ray ray = Camera.main.ScreenPointToRay(pos); //�J��������ray����
        if (Physics.Raycast(ray, out RaycastHit hit, 500, fieldMask)) //�n�ʂ�ray�q�b�g
        {
            Vector3 hitPos = hit.point; //�}�E�X�̍��W��3D�ɕϊ�
            prevInstance[trapIndex].transform.position = hitPos; //�v���r���[�\��

            float distance = Vector3.Distance(origin, hitPos); //�}�E�X�ʒu�ƃv���C���[�̋����v�Z
            //�v���C���[�Ƃ̋������ݒu�\�����ȉ�
            if (distance <= maxPlaceDistance)
            {
                canPlace = true; //�ݒu�\
            }
            else
            {
                canPlace = false; //�ݒu�s��
            }
            TryPlaceTrap(hitPos);
        }

    }


    //E�L�[�Ńg���b�v�̐ݒu�ʒu������
    public void OnPlaceTrap(InputAction.CallbackContext context)
    {
        //�g���b�v�I���ς݂��ݒu�\�ȏꏊ�Ȃ�g���b�v�ݒu
        if (decided == true && canPlace == true)
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
        isCollision = collision;
    }

    void TryPlaceTrap(Vector3 hitPos)
    {

        lastPreviewPos = prevInstance[trapIndex].transform;
        lastPreviewPos.localRotation = prevInstance[trapIndex].transform.localRotation;
        if (prevMaterial != null)
        {
            prevInstance[trapIndex].transform.position = hitPos;
            //Color��ݒu�\�Ȃ�΁A�s�\�Ȃ�ԂɕύX
            prevMaterial.color = canPlace ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
        }
        else
        {
            Debug.LogWarning("prevMaterial is null");
        }
    }

    //�g���b�v��ݒu����
    void SucceedPlaceTrap()
    {
        GameObject trap = trapPools[trapIndex].GetObject(); //�g���b�v���v�[��������o��
        prevInstance[trapIndex].SetActive(false); //�v���r���[��\��
        trap.SetActive(true);
        trap.transform.rotation = lastPreviewPos.localRotation;
        trap.transform.position = lastPreviewPos.position; //�g���b�v���v���r���[�̈ʒu�ɐݒu
        decided = false;
    }

    private void Start()
    {
        prevInstance = new GameObject[trapPools.Length];
        prevCollider = new Collider[trapPools.Length];
        for (int i = 0; i < trapPools.Length; i++)
        {
            prevInstance[i] = Instantiate(previewTrapPrefab[i]);
            prevCollider[i] = prevInstance[i].GetComponent<Collider>();
            prevInstance[i].SetActive(false);
        }
    }

    private void Update()
    {
        //�E�F�[�u�������g���b�v�ݒu���[�h��
        if (waveFinished == true && buildMode == true)
        {
            OnEnable(); //InputSystem�L��
        }
        else
        {
            OnDisable();
        }

        if (isRotating == true)
        {
            RotateTrap();
        }

        if (isCollision == true)
        {
            canPlace = false;
        }
    }
}
