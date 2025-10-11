using UnityEngine;
using System.Collections.Generic;
public class HitAttack : MonoBehaviour
{
    //攻撃アニメーションの当たり判定
    [SerializeField]
    List<Collider> attackColliders = new List<Collider>(); 

    //アニメーションの名前と対応する数を格納する
    [SerializeField]
    SerializableDictionary<string, int> attackColliderDictionary = null;
    public PlayerController playerController;

    //ダメージ倍率
    public float dmgMultipul;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    //攻撃アニメーションを参照して当たり判定を出す
    public void ColliderSet(string animName)
    {
        if (attackColliderDictionary.TryGetValue(animName, out int index))
        {          
            dmgMultipul = playerController.comboDmgMultipul[index];
            Debug.Log($"animName{animName} index{index} dmgMultipul{dmgMultipul}");
        }

        if (attackColliderDictionary.ContainsKey(animName))
        {
            attackColliders[attackColliderDictionary[animName]].enabled = true;
        }
    }
    
    //当たり判定削除
    public void ColliderRemove(string animName)
    {
        //ダメージ倍率を初期化
        dmgMultipul = 0;

        if(attackColliderDictionary.TryGetValue(animName,out int index))
        {
            Debug.Log($"animName{animName} index{index}");
        }

        if (attackColliderDictionary.ContainsKey(animName))
        {
            attackColliders[attackColliderDictionary[animName]].enabled = false;
        }
    }
}
