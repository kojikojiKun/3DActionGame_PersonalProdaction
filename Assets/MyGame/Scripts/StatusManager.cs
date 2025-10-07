using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [Header("�X�^���_�[�h�^�C�v�A�X�s�[�h�^�C�v�A�p���[�^�C�v�̏�")]
    [SerializeField]   
    private CharacterStatus[] characterStatuses; //�L�����N�^�[���
    private UpGradeStatus[] upGradeStatuses; //���s���̃L�����X�e�[�^�X

    public float havePoint; //�������Ă���|�C���g

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ScriptableObject ����f�[�^�쐬
        upGradeStatuses = new UpGradeStatus[characterStatuses.Length];
        for(int i=0 ; i<characterStatuses.Length; i++)
        {
            upGradeStatuses[i] = new UpGradeStatus(characterStatuses[i]);
            LoadCharacterUpgrade(upGradeStatuses[i]);
        }
    }

    //�X�e�[�^�X��ۑ�
    public void SaveCharacterUpgrade(UpGradeStatus status)
    {
        string id = status.baseData.id;

        PlayerPrefs.SetInt(id + "_Level", status.level);
        PlayerPrefs.SetFloat(id + "_HP", status.hp);
        PlayerPrefs.SetFloat(id + "_Speed", status.speed);
        PlayerPrefs.SetFloat(id + "_AS", status.attackSpeed);
        PlayerPrefs.SetFloat(id + "_AP", status.attackPower);

        PlayerPrefs.Save();
        Debug.Log($"�L�����u{id}�v�̋�����Ԃ�ۑ����܂���");
    }

    //�X�e�[�^�X��ǂݍ���
    public void LoadCharacterUpgrade(UpGradeStatus status)
    {
        string id = status.baseData.id;

        if (PlayerPrefs.HasKey(id + "_Level"))
        {
            status.level = PlayerPrefs.GetInt(id + "_Level");
            status.hp = PlayerPrefs.GetFloat(id + "_HP");
            status.speed = PlayerPrefs.GetFloat(id + "_Speed");
            status.attackSpeed = PlayerPrefs.GetFloat(id + "_AS");
            status.attackPower = PlayerPrefs.GetFloat(id + "_AP");

            Debug.Log($"�L�����u{id}�v�̋�����Ԃ�ǂݍ��݂܂����i���x��{status.level}�j");
        }
        else
        {
            Debug.Log($"�L�����u{id}�v�̕ۑ��f�[�^������܂���B�����l���g�p���܂��B");
        }
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }

    //�X�^���_�[�h�^�C�v�̋����{�^���ɐݒ�
    public void UpHP_Standard() => UpCharStatus(0, "HP");
    public void UpSpeed_Standard() => UpCharStatus(0, "Speed");
    public void UpAS_Standard() => UpCharStatus(0, "As");
    public void UpAP_Standard() => UpCharStatus(0, "Ap");

    //�X�s�[�h�^�C�v�̋����{�^���ɐݒ�
    public void UpHP_Speed() => UpCharStatus(1, "HP");
    public void UpSpeed_Speed() => UpCharStatus(1, "Speed");
    public void UpAS_Speed() => UpCharStatus(1, "As");
    public void UpAP_Speed() => UpCharStatus(1, "Ap");

    //�p���[�^�C�v�̋����{�^���ɐݒ�
    public void UpHP_Power() => UpCharStatus(2, "HP");
    public void UpSpeed_Power() => UpCharStatus(2, "Speed");
    public void UpAS_Power() => UpCharStatus(2, "AS");
    public void UpAP_Power() => UpCharStatus(2, "AP");

    public void UpCharStatus(int charactorIndex,string statusType)
    {
        var character = upGradeStatuses[charactorIndex];
        float cost = character.GetupGradeCost();

        //�|�C���g������Ă��Ȃ�
        if (havePoint < cost)
        {
            return;
        }

        Debug.Log($"{character.baseData.id}������");
        //�^�C�v���ƂɃX�e�[�^�X�㏸
        switch (statusType)
        {
            case "HP":
                character.UpHp();
                break;
            case "Speed":
                character.UpSpeed();
                break;
            case "AS":
                character.UpAttackSpeed();
                break;
            case "AP":
                character.UpAttackPower();
                break;
        }

        havePoint -= cost;
        SaveCharacterUpgrade(character);
    }
}
