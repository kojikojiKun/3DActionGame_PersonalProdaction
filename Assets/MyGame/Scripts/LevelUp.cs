using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public static LevelUp instance;
    [HideInInspector] public GameObject player; //�Q�[���J�n���Ƀv���C���[����I�u�W�F�N�g���炤
    private float expValue; //�������Ă���o���l�̗�
    private float remainingExp; //���߂����o���l
    public int haveItemValue; //�������Ă���A�C�e���̗�

    [SerializeField]
    private double defaultMaxExp; //���x���A�b�v�ɕK�v�Ȍo���l
    private int playerLevel; //���݂̃��x��
    

    public void GetExp(float receiveExp)
    {
        //�󂯎�����o���l���������Ă���o���l�ɑ���
        expValue += receiveExp;
        Debug.LogWarning($"{receiveExp}�̌o���l�l���I���x���A�b�v�܂�{defaultMaxExp-expValue}");

        //�o���l���ő�܂ł��܂����烌�x���A�b�v
        if (expValue >= defaultMaxExp)
        {
            playerLevel++;
            remainingExp =  expValue - (float) defaultMaxExp;

            //���x���A�b�v�ɕK�v�Ȍo���l�𑝉�
            defaultMaxExp *= Mathf.Pow(playerLevel, 1.2f);

            expValue = 0;
            expValue += remainingExp; //���߂����o���l�������z��          
            Debug.LogWarning($"���x���A�b�v�I���ɕK�v�Ȍo���l��{defaultMaxExp - expValue}");     
        }       
    }

    public�@void GetItem()
    {
        haveItemValue ++;
        Debug.LogWarning($"�A�C�e���l���I���݂̑���{haveItemValue}��");
    }

    public void UseItem()
    {

    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerLevel = 1; //���x��������
        expValue = 0; //�������Ă���o���l��������
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
