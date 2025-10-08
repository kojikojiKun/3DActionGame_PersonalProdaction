
using UnityEngine;
using UnityEngine.InputSystem;

public class PutTraps : MonoBehaviour
{
    //InputSystems
    [SerializeField] InputActionReference clickAction;
    [SerializeField] InputActionReference scrollAction;
    [SerializeField] InputActionReference mousePosition;
    [SerializeField] InputActionReference buttonDown;

    [SerializeField] ObjectPool[] trapPools; // �g���b�v�p�v�[��
    [SerializeField] GameObject[] previewTrapPrefab; //�g���b�v�̐ݒu�v���r���[�p�I�u�W�F�N�g
    private Transform lastPreviewPos; //�v���r���[�̍ŏI���W
    private Renderer[] previewRenderer;
    [SerializeField] private float maxPlaceDistance; //�g���b�v��ݒu�\�ȃv���C���[�Ƃ̋���
    [SerializeField] LayerMask fieldMask; //�g���b�v�ݒu�\�ȃ��C���[
    public bool waveFinished; //�E�F�[�u�I���t���O
    public bool buildMode=false; //�g���b�v�ݒu���[�h�t���O
    private bool decided = false; //�g���b�v�̎�ޑI�������t���O
    private bool canPlace; //�g���b�v�ݒu�\�t���O
    private int trapIndex = 0;
    private float nextAllowTime = 0f;
    private float scrollCoolDown = 0.1f; //�}�E�X�~�h���{�^���̃X�N���[���N�[���_�E��

    private void OnEnable()
    {
        clickAction.action.Enable();
        scrollAction.action.Enable();
        mousePosition.action.Enable();
        buttonDown.action.Enable();

    }
    void OnDisable()
    {
        clickAction.action.Disable();
        scrollAction.action.Disable();
        mousePosition.action.Disable();
        buttonDown.action.Enable();
    }

    //�}�E�X�~�h���{�^���X�N���[���Ńg���b�v�̎�ޑI��
    public void OnScroll(InputAction.CallbackContext context)
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

            if (decided == true)
            {
                previewTrapPrefab[trapIndex].SetActive(true);
               Debug.Log($"{trapIndex}oooooooooooo");
            }
            else
            {
                previewTrapPrefab[trapIndex].SetActive(false);
                Debug.Log($"{trapIndex}iiiiiiiiiiiiiiiii");
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
                trapIndex = trapPools.Length-1;
            }
        }  
    }

    //�J�[�\���ړ��Ńg���b�v�̐ݒu�ʒu�����߂�
    public void MoveTrap(InputAction.CallbackContext context)
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        Vector3 origin = transform.position;
        Ray ray = Camera.main.ScreenPointToRay(pos); //�J��������ray����
        if (Physics.Raycast(ray, out RaycastHit hit,500, fieldMask)) //�n�ʂ�ray�q�b�g
        {
            Vector3 hitPos = hit.point; //�}�E�X�̍��W��3D�ɕϊ�
            previewTrapPrefab[trapIndex].transform.position = hitPos; //�v���r���[�\��

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
    public void PlaceTrap(InputAction.CallbackContext context)
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

    void TryPlaceTrap(Vector3 hitPos)
    {
        GameObject preview = previewTrapPrefab[trapIndex];
        Renderer renderer = preview.GetComponent<Renderer>();
        lastPreviewPos = preview.transform;
        if (renderer == null)
        {
            preview.transform.position = hitPos;
            
            renderer.material.color = canPlace ? Color.green : Color.red; //Color��ݒu�\�Ȃ�΁A�s�\�Ȃ�ԂɕύX
        }
    }

    //�g���b�v��ݒu����
    void SucceedPlaceTrap()
    {
        GameObject trap = trapPools[trapIndex].GetObject(); //�g���b�v���v�[��������o��
        trap.transform.position=lastPreviewPos.position; //�g���b�v���v���r���[�̈ʒu�ɐݒu
        decided = false;
    }

    private void Start()
    {
        previewRenderer = new Renderer[trapPools.Length];
        for (int i = 0; i < trapPools.Length; i++)
        {
            previewRenderer[i] = previewTrapPrefab[i].GetComponent<Renderer>();
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
    }
}
