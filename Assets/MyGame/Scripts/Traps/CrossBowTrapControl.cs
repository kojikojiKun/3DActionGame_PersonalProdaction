using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossBowTrapControl : MonoBehaviour
{
    public static List<CrossBowTrapControl> allCrossBowTraps = new List<CrossBowTrapControl>();
    [SerializeField] TrapType type;
    [SerializeField] TrapStaus trapStaus;
    [Header("status")]
    [SerializeField] Transform originTransform;
    [SerializeField] LayerMask targetMask;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shotPos;
    private float shotArrowInterval; //矢の発射間隔
    private float arrowDamage; //矢のダメージ;
    private float shotRange; //射程距離
    private int numOfArrow; //矢の数
    private float spreadAngle; //矢の拡散する角度
    private bool isShot=true;

    private void Awake()
    {
        allCrossBowTraps.Add(this);
    }

    private void OnDestroy()
    {
        allCrossBowTraps.Remove(this);
    }

    //TrapStatusで設定された値を代入
    public void SetCrossBowStatus(float interval, float damage, float range, float angle, int amount)
    {
        shotArrowInterval = interval;
        arrowDamage = damage;
        shotRange = range;
        spreadAngle = angle;
        numOfArrow = amount;
    }
    private void GetEnemies()
    {
        Vector3 origin = originTransform.position;

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

    private void OnDrawGizmos()
    {
        Vector3 origin = originTransform.transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(origin, shotRange);
    }

    //矢を生成し敵に向けて射出
    private IEnumerator shotArrow(Transform targetPos)
    {
        isShot = false;
        Debug.Log("shot");
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
