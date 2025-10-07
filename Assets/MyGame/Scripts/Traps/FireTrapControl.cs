using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FireTrapControl : MonoBehaviour
{
    public static List<FireTrapControl> allFireTraps = new List<FireTrapControl>();
    [SerializeField] TrapStaus trapStaus;
    [SerializeField] private TrapType type;

    [Header("Status")]
    [SerializeField] Transform originTransform;
    [SerializeField] LayerMask targetMask;
    [SerializeField] ParticleSystem[] fireParticle; //炎のパーティクル(3つ)
    [SerializeField] private float radius; //円の半径(炎の射程距離)
    [SerializeField] private float coneAngle; //炎トラップの判定角度
    private float fireStartSpeed; //パーティクルの初速度
    private float fireShotInterval; //パーティクルの再生間隔
    private float fireDuration; //パーティクルの持続時間
    private float fireDamageInterval; //炎が敵にダメージを与える間隔
    private float fireDamage; //炎のダメージ
    private bool isShot = true;

    private void Awake()
    {
        allFireTraps.Add(this);
    }

    private void OnDestroy()
    {
        allFireTraps.Remove(this);
    }

    public void SetFireStatus(float startSpeed, float interval, float duration, float dmgInterval, float damage)
    {
        fireStartSpeed = startSpeed;
        fireShotInterval = interval;
        fireDuration = duration;
        fireDamageInterval = dmgInterval;
        fireDamage = damage;
        radius = fireStartSpeed * 0.8f; //パーティクルの初速度で射程距離を決める

        for (int i = 0; i < fireParticle.Length; i++)
        {
            var main = fireParticle[i].main;
            main.startSpeed = fireStartSpeed;
            main.duration = fireDuration;
        }
    }

    private void GetEnemies()
    {

        Vector3 origin = originTransform.position; //開始位置
        Vector3 forward = originTransform.forward; //方向を前方に設定
        Collider[] colliders = Physics.OverlapSphere(origin, radius, targetMask); //円の中のコライダーをすべて取得(targetMask以外は除外する)
        List<GameObject> targets = new List<GameObject>(); //範囲内の敵を格納するリストを作成

        foreach (Collider col in colliders)
        {
            Vector3 toTarget = (col.transform.position - origin).normalized; //ターゲットとの距離を計算し正規化
            float angle = Vector3.Angle(forward, toTarget); //前方向との角度を計算

            //coneAngle内なら判定する
            if (angle < coneAngle / 2f)
            {
                targets.Add(col.gameObject); //範囲内の敵を入れる配列を作成
                for (int i = 0; i < colliders.Length; i++)
                {
                    targets[i] = colliders[i].gameObject; //配列内の敵を取得
                }
            }
        }

        if (isShot == true)
        { //敵が攻撃範囲に入ったら攻撃
            StartCoroutine(ShotFire(fireDamage,
                fireDamageInterval,
                targets.ToArray())); //炎を発射
        }
    }

    /*private void OnDrawGizmos()
    {
        Vector3 origin = originTransform.position;
        Vector3 forward = originTransform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin, radius);

        Gizmos.color = Color.yellow;
        float halfAngle = coneAngle / 2f;

        // 左境界線
        Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * forward;
        Gizmos.DrawRay(origin, leftDir * radius);

        // 右境界線
        Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * forward;
        Gizmos.DrawRay(origin, rightDir * radius);
    }*/

    //炎を発射
    private IEnumerator ShotFire(float damage, float dmgInterval, GameObject[] targets)
    {
        isShot = false;
        
        //パーティクル再生
        for (int i = 0; i < fireParticle.Length; i++)
        {
            fireParticle[i].Play();
        }
        Debug.Log("Fire!!!");
        StartCoroutine(DamageContinue(damage, dmgInterval, targets));
        yield return new WaitForSeconds(fireDuration); //パーティクルの再生が終わるまで待機       
        yield return new WaitForSeconds(fireShotInterval); //fireInterval秒待機
        isShot = true;
    }

    //敵に継続的にダメージを与える
    //ダメージの値とダメージを与える間隔の値,ダメージを与える対象を渡す
    private IEnumerator DamageContinue(float damage, float dmgInterval, GameObject[] targets)
    {
        float timer = 0;
        while (timer < fireDuration)
        {
            Debug.Log("111111111111111111111111111111");
            foreach (GameObject enemy in targets)
            {
                if (enemy == null) continue; //nullチェック

                EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
                if (enemyStatus != null) //nullチェック
                {
                    enemyStatus.TakeDamageEnemy(damage); //敵にダメージを与える
                }

            }
            yield return new WaitForSeconds(dmgInterval); //dmgInterval秒待機
            timer += dmgInterval;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trapStaus.GiveStatus();
    }

    // Update is called once per frame
    void Update()
    {
        GetEnemies();
    }
}
