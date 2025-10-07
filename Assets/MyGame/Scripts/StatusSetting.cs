using UnityEngine;

public class StatusSetting : MonoBehaviour
{
    public static StatusSetting Instance;
    [HideInInspector]
    public CharacterStatus status;
    public CharacterStatus[] allStatuses;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        LoadStatus();
    }

    //�I�����ꂽ�^�C�v�̃X�e�[�^�X���Z�b�g����
    private void SetStatus(CharacterStatus newStatus)
    {
        status = newStatus;
        SaveStatus();
        Debug.Log($"���݂̃X�e�[�^�X��{status.id}");
    }

    //�f�[�^��ۑ�
    private void SaveStatus()
    {
        PlayerPrefs.SetString("StatusID", status.id);
        PlayerPrefs.Save();
        Debug.Log($"{status.id}�̃X�e�[�^�X��ۑ�");

        PlayerPrefs.SetFloat(status.id + "_HP", status.hp);
        PlayerPrefs.SetFloat(status.id + "_Speed", status.speed);
        PlayerPrefs.SetFloat(status.id + "_AS", status.attackSpeed);
        PlayerPrefs.SetFloat(status.id + "_AP", status.attackPower);
    }

    private void LoadStatus()
    {
        //�f�[�^���ۑ�����Ă��邩�`�F�b�N
        string savedID = PlayerPrefs.GetString("StatusID", "");
        if (string.IsNullOrEmpty(savedID))
        {
            Debug.Log("�X�e�[�^�X���ۑ�����Ă��܂���");
            return;
        }
        else
        {
            Debug.Log("�X�e�[�^�X��ǂݍ��݉\�ł�");
        }
    
        //���ׂẴ^�C�v�̃X�e�[�^�X�����[�h
        foreach(var charStatus in allStatuses)
        {
            if (charStatus.id == savedID)
            {
                status = charStatus;
                Debug.Log($"�X�e�[�^�X {savedID} ��ǂݍ��݂܂���");
                return;
            }
        }
    }

    //�X�^���_�[�h�^�C�v��I�񂾎�
    public void selectStandardType()
    {
        foreach(var s in allStatuses)
        {
            if (s.id == "StandardType")
            {
                SetStatus(s);
            }
        }
        log();
    }

    //�p���[�^�C�v��I�񂾎�
    public void selectPowerType()
    {
        foreach (var s in allStatuses)
        {
            if (s.id == "PowerType")
            {
                SetStatus(s);
            }
        }
        log();
    }

    //�p���[�^�C�v��I�񂾎�
    public void selectSpeedType()
    {
        foreach (var s in allStatuses)
        {
            if (s.id == "SpeedType")
            {
                SetStatus(s);
            }
        }
        log();
    }

    public void log()
    {
        Debug.Log("�L�����N�^�[�^�C�v" + status.type);
        Debug.Log("HP" + status.hp);
        Debug.Log("�U�����x" + status.attackSpeed);
        Debug.Log("�U����" + status.attackPower);
        Debug.Log("�R�X�g" + status.baseCost);
    }
}
