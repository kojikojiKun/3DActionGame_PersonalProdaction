using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [Header("スタンダードタイプ、スピードタイプ、パワータイプの順")]
    [SerializeField]   
    private CharacterStatus[] characterStatuses; //キャラクター情報
    private UpGradeStatus[] upGradeStatuses; //実行中のキャラステータス

    public float havePoint; //所持しているポイント

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ScriptableObject からデータ作成
        upGradeStatuses = new UpGradeStatus[characterStatuses.Length];
        for(int i=0 ; i<characterStatuses.Length; i++)
        {
            upGradeStatuses[i] = new UpGradeStatus(characterStatuses[i]);
            LoadCharacterUpgrade(upGradeStatuses[i]);
        }
    }

    //ステータスを保存
    public void SaveCharacterUpgrade(UpGradeStatus status)
    {
        string id = status.baseData.id;

        PlayerPrefs.SetInt(id + "_Level", status.level);
        PlayerPrefs.SetFloat(id + "_HP", status.hp);
        PlayerPrefs.SetFloat(id + "_Speed", status.speed);
        PlayerPrefs.SetFloat(id + "_AS", status.attackSpeed);
        PlayerPrefs.SetFloat(id + "_AP", status.attackPower);

        PlayerPrefs.Save();
        Debug.Log($"キャラ「{id}」の強化状態を保存しました");
    }

    //ステータスを読み込み
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

            Debug.Log($"キャラ「{id}」の強化状態を読み込みました（レベル{status.level}）");
        }
        else
        {
            Debug.Log($"キャラ「{id}」の保存データがありません。初期値を使用します。");
        }
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }

    //スタンダードタイプの強化ボタンに設定
    public void UpHP_Standard() => UpCharStatus(0, "HP");
    public void UpSpeed_Standard() => UpCharStatus(0, "Speed");
    public void UpAS_Standard() => UpCharStatus(0, "As");
    public void UpAP_Standard() => UpCharStatus(0, "Ap");

    //スピードタイプの強化ボタンに設定
    public void UpHP_Speed() => UpCharStatus(1, "HP");
    public void UpSpeed_Speed() => UpCharStatus(1, "Speed");
    public void UpAS_Speed() => UpCharStatus(1, "As");
    public void UpAP_Speed() => UpCharStatus(1, "Ap");

    //パワータイプの強化ボタンに設定
    public void UpHP_Power() => UpCharStatus(2, "HP");
    public void UpSpeed_Power() => UpCharStatus(2, "Speed");
    public void UpAS_Power() => UpCharStatus(2, "AS");
    public void UpAP_Power() => UpCharStatus(2, "AP");

    public void UpCharStatus(int charactorIndex,string statusType)
    {
        var character = upGradeStatuses[charactorIndex];
        float cost = character.GetupGradeCost();

        //ポイントが足りていない
        if (havePoint < cost)
        {
            return;
        }

        Debug.Log($"{character.baseData.id}を強化");
        //タイプごとにステータス上昇
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
