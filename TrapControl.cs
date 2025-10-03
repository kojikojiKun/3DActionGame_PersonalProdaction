using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum TrapType
{
    FlameTrap,
    SpinBladeTrap,
    CrossBowTrap,
    SpikeTrap
}

public class TrapControl : MonoBehaviour
{
    public TrapType type; //トラップの種類をインスペクターで設定

    [Header("Flame")]
    [SerializeField] ParticleSystem[] fireParticle; //炎のパーティクル(3つ)
    private float fireStartSpeed; //パーティクルの初速度
    private float fireShotInterval; //パーティクルの再生間隔
    private float fireDuration; //パーティクルの持続時間
    private float fireDamageInterval; //炎が敵にダメージを与える間隔
    private float fireDamage; //炎のダメージ

    //TrapStatusで設定された値を代入
    public void SetFlameTrapStasus(float startSpeed, float interval, float duration, float dmgInterval, float damage)
    {
        fireStartSpeed = startSpeed;
        fireShotInterval = interval;
        fireDuration = duration;
        fireDamageInterval = dmgInterval;
        fireDamage = damage;
    }

    //private Vector3 center; //判定の中心
    [SerializeField] private Transform originFlameTrapTransForm; //当たり判定の開始位置
    [SerializeField] private float radius; //円の半径(炎の射程距離)
    [SerializeField] private float coneAngle; //視野の角度
    [SerializeField] private LayerMask targetMask; //対象のレイヤー

    private void FlameControl()
    {
        enemyEnterRange = false;

        Vector3 origin = originFlameTrapTransForm.position; //開始位置
        Vector3 forward = originFlameTrapTransForm.forward; //方向を前方に設定
        Collider[] colliders = Physics.OverlapSphere(origin, radius, targetMask); //円の中のコライダーをすべて取得(targetMask以外は除外する)
        List<GameObject> targetEnemies = new List<GameObject>(); //範囲内の敵を格納するリストを作成

        foreach (Collider collider in colliders)
        {
            Vector3 toTarget = (collider.transform.position - origin).normalized; //ターゲットとの距離を計算し正規化
            float angle = Vector3.Angle(forward, toTarget); //前方向との角度を計算

            //coneAngle内なら判定する
            if (angle < coneAngle / 2f)
            {
                targetEnemies.Add(collider.gameObject); //範囲内の敵を入れる配列を作成
                for (int i = 0; i < colliders.Length; i++)
                {
                    targetEnemies[i] = colliders[i].gameObject; //配列内の敵を取得
                }
            }
        }

        if (targetEnemies.Count > 0) //範囲内の敵が一体以上
        {
            enemyEnterRange = true;

            if (giveDamage == true)
                StartCoroutine(DamageContinue(fireDamage,
                                                   fireDamageInterval,
                                                    targetEnemies.ToArray())); //fireDamageをfireDamageInterval秒ごとに敵に与える
        }

        if (enemyEnterRange == true && isShot == true) //敵が攻撃範囲に入ったら攻撃
            StartCoroutine(ShotFire()); //炎を発射
    }

    //炎を発射
    private IEnumerator ShotFire()
    {
        isShot = false;

        //パーティクル再生
        fireParticle[0].Play();
        fireParticle[1].Play();
        fireParticle[2].Play();
        Debug.Log("Fire!!!");
        yield return new WaitForSeconds(fireDuration); //パーティクルの再生が終わるまで待機
        yield return new WaitForSeconds(fireShotInterval); //fireInterval秒待機
        isShot = true;
    }

    private bool enemyEnterRange = false; //敵の検知フラグ
    private bool isShot = true; //攻撃フラグ
    private bool giveDamage;

    //敵に継続的にダメージを与える(ブレードトラップ、炎トラップ用)
    //ダメージの値とダメージを与える間隔の値,ダメージを与える対象を渡す
    private IEnumerator DamageContinue(float damage, float dmgInterval, GameObject[] targets)
    {
        giveDamage = false;

        float timer = 0;
        while (timer < dmgInterval)
        {
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
        giveDamage = true;
    }

    //シーンビューで当たり判定を表示
    private void OnDrawGizmos()
    {
        Vector3 origin = originFlameTrapTransForm.position;
        Vector3 forward = originFlameTrapTransForm.forward;

        Gizmos.color = new Color(0, 1, 0, 1);
        Gizmos.DrawWireSphere(origin, radius);

        Quaternion leftRot = Quaternion.AngleAxis(-coneAngle / 2f, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(coneAngle / 2f, Vector3.up);

        Vector3 leftDir = leftRot * forward;
        Vector3 rightDir = rightRot * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + leftDir * radius);
        Gizmos.DrawLine(origin, origin + rightDir * radius);
    }

    public void SetCrossBowTrapStatus()
    {

    }
    private void CrossBowControl()
    {

    }

    public void SetBladeTrapStatus()
    {

    }
    private void BladeTrapControl()
    {

    }

    public void SetSpikeWallStasus()
    {

    }
    private void SpikeWallControl()
    {

    }

    private void Start()
    {
        switch (type)
        {
            case TrapType.FlameTrap:
                //ステータスの初期値を設定
                fireStartSpeed = TrapStaus.instance.particleStartSpeed;
                fireShotInterval = TrapStaus.instance.shotFlameInterval;
                fireDuration = TrapStaus.instance.shotFlameDuration;
                fireDamageInterval = TrapStaus.instance.flameDamageInterval;

                //パーティクルモジュールを設定 
                var main_0 = fireParticle[0].main;
                var main_1 = fireParticle[1].main;
                var main_2 = fireParticle[2].main;

                //パーティクルのパラメータを設定
                //初速度
                main_0.startSpeed = fireStartSpeed;
                main_1.startSpeed = fireStartSpeed;
                main_2.startSpeed = fireStartSpeed;
                //持続時間
                main_0.duration = fireDuration;
                main_1.duration = fireDuration;
                main_2.duration = fireDuration;
                break;
            case TrapType.CrossBowTrap:
                break;
            case TrapType.SpinBladeTrap:
                break;
            case TrapType.SpikeTrap:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case TrapType.FlameTrap:
                FlameControl();
                break;
        }
    }
}