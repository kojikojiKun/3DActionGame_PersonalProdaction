using UnityEngine;

public class BreakBuildObject : MonoBehaviour
{
    [SerializeField]
    private int hp=3;
    private void OnTriggerEnter(Collider other)
    {
        //敵キャラクターの攻撃判定に接触するとダメージを受ける
        if (other.CompareTag("EnemyAttackCollider"))
        {
            hp -= 1;
        }

        if (hp <= 0)
        {
            //建物が壊れた時の処理
            Destroy(gameObject);
            //aaaaaaaaaaaaaaaaaaaaaaaaaaaa
        }
    }
}
