using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public enum TrapType
{
    FlameTrap,
    SpinBladeTrap,
    CrossBowTrap,
    SpikeTrap
}

public class TrapControl : MonoBehaviour
{
    [Header("commonStatus")]
    public TrapType type; //トラップの種類をインスペクターで設定   
    [SerializeField] private Transform originTransForm; //当たり判定の開始位置
    [SerializeField] private float radius; //円の半径(射程距離)
    [SerializeField] private LayerMask targetMask; //対象のレイヤー
    private bool enemyEnterRange; //敵の検知フラグ
    private bool isShot = true; //攻撃フラグ

    [Header("Flame")]
    [SerializeField] ParticleSystem[] fireParticle; //炎のパーティクル(3つ)
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
    [SerializeField] private float spreadAngle;

    //TrapStatusで設定された値を代入
    public void SetFlameTrapStasus(float startSpeed, float interval, float duration, float dmgInterval, float damage)
    {
        fireStartSpeed = startSpeed;
        fireShotInterval = interval;
        fireDuration = duration;
        fireDamageInterval = dmgInterval;
        fireDamage = damage;
        radius = fireStartSpeed * 0.8f; //パーティクルの初速度で射程距離を決める
    }

    private void FlameControl()
    {
        enemyEnterRange = false;

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

        if (targets.Count > 0) //範囲内の敵が一体以上
        {
            enemyEnterRange = true;
        }

        if (enemyEnterRange == true && isShot == true) //敵が攻撃範囲に入ったら攻撃
            StartCoroutine(ShotFire(fireDamage,
                fireDamageInterval,
                targets.ToArray())); //炎を発射
    }

    //炎を発射
    private IEnumerator ShotFire(float damage, float dmgInterval, GameObject[] targets)
    {
        isShot = false;

        //パーティクル再生
        fireParticle[0].Play();
        fireParticle[1].Play();
        fireParticle[2].Play();
        Debug.Log("Fire!!!");
        StartCoroutine(DamageContinue(damage, dmgInterval, targets));
        yield return new WaitForSeconds(fireDuration); //パーティクルの再生が終わるまで待機       
        yield return new WaitForSeconds(fireShotInterval); //fireInterval秒待機
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

    public void SetCrossBowTrapStatus(float interval, float damage, float range, int amount)
    {
        shotArrowInterval = interval;
        arrowDamage = damage;
        shotRange = range;
        numOfArrow = amount;
    }
    private void CrossBowControl()
    {
        enemyEnterRange = false;
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
            Debug.Log("ssssssssssssssssssss");
            Vector3 targetPosition = new Vector3(closestTarget.transform.position.x,
                       gameObject.transform.position.y,
                       closestTarget.transform.position.z);

            gameObject.transform.LookAt(targetPosition); //本体を敵の方向に向ける
            transform.Rotate(0, 90, 0); //オブジェクトの角度調整

            if (isShotArrow == true)
            {
                Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaa");
                shotArrow(closestTarget.transform);
            }
        }
    }

    private bool isShotArrow = true;
    private IEnumerator shotArrow(Transform targetPos)
    {
        isShotArrow = false;
        Debug.Log("shot");
        if (numOfArrow <2)
        {
            Vector3 direction = (targetPos.position - shotPos.position).normalized; //発射角度計算

            //矢を生成
            GameObject arrow = Instantiate(arrowPrefab, shotPos.position, Quaternion.LookRotation(direction));
            ArrowMove arrowMove = arrow.GetComponent<ArrowMove>();
            arrowMove.SetDirection(direction,arrowDamage); //矢に速度とダメージを渡す
            yield return new WaitForSeconds(shotArrowInterval); //shotArrowInterval秒待機
        }
        else
        {
            Vector3 targetDir = (targetPos.position - shotPos.position).normalized; //敵との角度を計算
            float startAngle = spreadAngle / 2; //矢の発射位置の真ん中
            float angleStep = spreadAngle / (numOfArrow - 1); //真ん中からずらす角度

            for(int i = 0; i < numOfArrow; i++)
            {
                float angle = startAngle + i * angleStep; //真ん中の発射位置から角度をずらす

                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 direction = rotation * targetDir; //進む方向を計算

                //矢を放射状に生成
                GameObject arrow = Instantiate(arrowPrefab, shotPos.position, Quaternion.LookRotation(direction));
                ArrowMove arrowMove = arrow.GetComponent<ArrowMove>();
                arrowMove.SetDirection(direction,arrowDamage); //矢に速度とダメージを渡す
                yield return new WaitForSeconds(shotArrowInterval); //shotArrowInterval秒待機
            }
        }
        isShotArrow = true;
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
                fireDamage = TrapStaus.instance.flameDamage;

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
                shotArrowInterval = TrapStaus.instance.shotArrowInterval;
                arrowDamage = TrapStaus.instance.arrowDamage;
                shotRange = TrapStaus.instance.crossBowRange;
                numOfArrow = 1;
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
            case TrapType.CrossBowTrap:
                CrossBowControl();
                break;
        }
    }
}