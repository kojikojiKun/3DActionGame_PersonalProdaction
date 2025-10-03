using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public static LevelUp instance;
    [HideInInspector] public GameObject player; //ゲーム開始時にプレイヤーからオブジェクトもらう
    private float expValue; //所持している経験値の量
    private float remainingExp; //超過した経験値
    public int haveItemValue; //所持しているアイテムの量

    [SerializeField]
    private double defaultMaxExp; //レベルアップに必要な経験値
    private int playerLevel; //現在のレベル
    

    public void GetExp(float receiveExp)
    {
        //受け取った経験値を所持している経験値に足す
        expValue += receiveExp;
        Debug.LogWarning($"{receiveExp}の経験値獲得！レベルアップまで{defaultMaxExp-expValue}");

        //経験値が最大までたまったらレベルアップ
        if (expValue >= defaultMaxExp)
        {
            playerLevel++;
            remainingExp =  expValue - (float) defaultMaxExp;

            //レベルアップに必要な経験値を増加
            defaultMaxExp *= Mathf.Pow(playerLevel, 1.2f);

            expValue = 0;
            expValue += remainingExp; //超過した経験値を持ち越す          
            Debug.LogWarning($"レベルアップ！次に必要な経験値は{defaultMaxExp - expValue}");     
        }       
    }

    public　void GetItem()
    {
        haveItemValue ++;
        Debug.LogWarning($"アイテム獲得！現在の総量{haveItemValue}個");
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
        playerLevel = 1; //レベル初期化
        expValue = 0; //所持している経験値を初期化
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
