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

    //選択されたタイプのステータスをセットする
    private void SetStatus(CharacterStatus newStatus)
    {
        status = newStatus;
        SaveStatus();
        Debug.Log($"現在のステータスは{status.id}");
    }

    //データを保存
    private void SaveStatus()
    {
        PlayerPrefs.SetString("StatusID", status.id);
        PlayerPrefs.Save();
        Debug.Log($"{status.id}のステータスを保存");

        PlayerPrefs.SetFloat(status.id + "_HP", status.hp);
        PlayerPrefs.SetFloat(status.id + "_Speed", status.speed);
        PlayerPrefs.SetFloat(status.id + "_AS", status.attackSpeed);
        PlayerPrefs.SetFloat(status.id + "_AP", status.attackPower);
    }

    private void LoadStatus()
    {
        //データが保存されているかチェック
        string savedID = PlayerPrefs.GetString("StatusID", "");
        if (string.IsNullOrEmpty(savedID))
        {
            Debug.Log("ステータスが保存されていません");
            return;
        }
        else
        {
            Debug.Log("ステータスを読み込み可能です");
        }
    
        //すべてのタイプのステータスをロード
        foreach(var charStatus in allStatuses)
        {
            if (charStatus.id == savedID)
            {
                status = charStatus;
                Debug.Log($"ステータス {savedID} を読み込みました");
                return;
            }
        }
    }

    //スタンダードタイプを選んだ時
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

    //パワータイプを選んだ時
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

    //パワータイプを選んだ時
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
        Debug.Log("キャラクタータイプ" + status.type);
        Debug.Log("HP" + status.hp);
        Debug.Log("攻撃速度" + status.attackSpeed);
        Debug.Log("攻撃力" + status.attackPower);
        Debug.Log("コスト" + status.baseCost);
    }
}
