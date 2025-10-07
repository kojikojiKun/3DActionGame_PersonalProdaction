using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
public enum TrapType
{
    FireTrap,
    SpinBladeTrap,
    CrossBowTrap,
    SpikeTrap
}

public class TrapControl : MonoBehaviour
{
    [Header("commonStatus")]
    public TrapType type; //トラップの種類をインスペクターで設定   
    [SerializeField] TrapStaus trapStaus;
    [SerializeField] private Transform originTransForm; //当たり判定の開始位置
    
    [SerializeField] private LayerMask targetMask; //対象のレイヤー
    private bool isShot = true; //攻撃フラグ

    [Header("Fire")]
    [SerializeField] ParticleSystem[] fireParticle; //炎のパーティクル(3つ)
    [SerializeField] private float radius; //円の半径(射程距離)
    [SerializeField] private float coneAngle; //炎トラップの判定角度
    private float fireStartSpeed; //パーティクルの初速度
    private float fireShotInterval; //パーティクルの再生間隔
    private float fireDuration; //パーティクルの持続時間
    private float fireDamageInterval; //炎が敵にダメージを与える間隔
    private float fireDamage; //炎のダメージ
    
    [Header("CrossBow")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shotPos;
    private float shotArrowInterval; //矢の発射間隔
    private float arrowDamage; //矢のダメージ;
    private float shotRange; //射程距離
    private int numOfArrow; //矢の数
    private float spreadAngle; //矢の拡散する角度

    [Header("SpinBlade")]
    [SerializeField] NavMeshAgent bladeAgent;
    private float bladeSize;
    private float bladeSpeed;
    private float bladeDmgInterval;
    private float bladeDmg;

    //TrapStatusで設定された値を代入
    public void SetFireTrapStasus(float startSpeed, float interval, float duration, float dmgInterval, float damage)
    {
        fireStartSpeed = startSpeed;
        fireShotInterval = interval;
        fireDuration = duration;
        fireDamageInterval = dmgInterval;
        fireDamage = damage;
        radius = fireStartSpeed * 0.8f; //パーティクルの初速度で射程距離を決める

        for(int i = 0; i < fireParticle.Length; i++)
        {
            var main = fireParticle[i].main;
            main.startSpeed = fireStartSpeed;
            main.duration = fireDuration;
        }
    }

    private void FireControl()
    {

        Vector3 origin = originTransForm.position; //開始位置
        Vector3 forward = originTransForm.forward; //方向を前方に設定
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

        if ( isShot == true) //敵が攻撃範囲に入ったら攻撃
            StartCoroutine(ShotFire(fireDamage,
                fireDamageInterval,
                targets.ToArray())); //炎を発射
    }

    //炎を発射
    private IEnumerator ShotFire(float damage, float dmgInterval, GameObject[] targets)
    {
        isShot = false;
        yield return new WaitForSeconds(fireShotInterval); //fireInterval秒待機
        //パーティクル再生
        for (int i = 0; i < fireParticle.Length; i++)
        {
            fireParticle[i].Play();
        }
        Debug.Log("Fire!!!");
        StartCoroutine(DamageContinue(damage, dmgInterval, targets));
        yield return new WaitForSeconds(fireDuration); //パーティクルの再生が終わるまで待機       

        isShot = true;
    }

    //敵に継続的にダメージを与える(ブレードトラップ、炎トラップ用)
    //ダメージの値とダメージを与える間隔の値,ダメージを与える対象を渡す
    private IEnumerator DamageContinue(float damage, float dmgInterval, GameObject[] targets)
    {
        float timer = 0;
        while (timer < fireDuration)
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
    }

    public void SetCrossBowTrapStatus(float interval, float damage, float range, float angle, int amount)
    {
        shotArrowInterval = interval;
        arrowDamage = damage;
        shotRange = range;
        spreadAngle = angle;
        numOfArrow = amount;
    }
    private void CrossBowControl()
    {
        Vector3 origin = originTransForm.position;

        Collider[] colliders = Physics.OverlapSphere(origin, shotRange, targetMask);
        Collider closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            float distance = (col.transform.position - origin).sqrMagnitude; //射程距離内の敵との距離を計算
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = col; //もっとも距離が近い敵を代入
            }
        }

        if (colliders.Length > 0)
        {
            Vector3 targetPosition = new Vector3(closestTarget.transform.position.x,
                      gameObject.transform.position.y,
                       closestTarget.transform.position.z);

            gameObject.transform.LookAt(targetPosition); //本体を敵の方向に向ける

            if (isShot == true)
            {
                StartCoroutine(shotArrow(closestTarget.transform));
            }
        }
    }

    private IEnumerator shotArrow(Transform targetPos)
    {
        isShot = false;
        if (numOfArrow < 2)
        {
            Debug.Log("矢は一本");
            Vector3 direction = (targetPos.position - shotPos.position).normalized; //発射角度計算

            //矢を生成
            GameObject arrow = Instantiate(arrowPrefab, shotPos.position, Quaternion.LookRotation(direction));
            ArrowMove arrowMove = arrow.GetComponent<ArrowMove>();
            arrowMove.SetDirection(direction, arrowDamage); //矢に速度とダメージを渡す
            yield return new WaitForSeconds(shotArrowInterval); //shotArrowInterval秒待機
        }
        else
        {
            Debug.Log("矢は複数本");
            Vector3 targetDir = (targetPos.position - shotPos.position).normalized; //敵との角度を計算
            float startAngle = spreadAngle / 2; //矢の発射位置の真ん中
            float angleStep = spreadAngle / (numOfArrow - 1); //真ん中からずらす角度

            for (int i = 0; i < numOfArrow; i++)
            {
                float angle = startAngle + i * angleStep; //真ん中の発射位置から角度をずらす
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

                //回転方向切り替え
                if (i == 0)
                {
                    rotation = Quaternion.AngleAxis(0, Vector3.zero);
                }
                else if (i % 2 == 0)
                {
                    rotation = Quaternion.AngleAxis(angle, Vector3.up);
                }
                else
                {
                    rotation = Quaternion.AngleAxis(angle, -Vector3.up);
                }
                Vector3 direction = rotation * targetDir; //進む方向を計算

                //矢を放射状に生成
                GameObject arrow = Instantiate(arrowPrefab, shotPos.position, Quaternion.LookRotation(direction));
                ArrowMove arrowMove = arrow.GetComponent<ArrowMove>();
                arrowMove.SetDirection(direction, arrowDamage); //矢に速度とダメージを渡す
            }
            yield return new WaitForSeconds(shotArrowInterval); //shotArrowInterval秒待機
        }
        isShot = true;
    }



    public void SetBladeTrapStatus(float size, float speed, float dmgInterval,float damage)
    {
        bladeSize = size;
        bladeSpeed = speed;
        bladeDmgInterval = dmgInterval;
        bladeDmg = damage;
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

    public static List<TrapControl> AllTraps = new List<TrapControl>();
    private void Awake()
    {
        AllTraps.Add(this);
    }

    private void OnDestroy()
    {
        AllTraps.Remove(this);
    }

    private void Start()
    {
        trapStaus.GiveStatus();
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case TrapType.FireTrap:
                FireControl();
                break;
            case TrapType.CrossBowTrap:
                CrossBowControl();
                break;
        }
    }
}