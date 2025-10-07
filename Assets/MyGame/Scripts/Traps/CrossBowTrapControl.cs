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
    private float shotArrowInterval; //��̔��ˊԊu
    private float arrowDamage; //��̃_���[�W;
    private float shotRange; //�˒�����
    private int numOfArrow; //��̐�
    private float spreadAngle; //��̊g�U����p�x
    private bool isShot=true;

    private void Awake()
    {
        allCrossBowTraps.Add(this);
    }

    private void OnDestroy()
    {
        allCrossBowTraps.Remove(this);
    }

    //TrapStatus�Őݒ肳�ꂽ�l����
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
            float distance = (col.transform.position - origin).sqrMagnitude; //�˒��������̓G�Ƃ̋������v�Z
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = col; //�����Ƃ��������߂��G����
            }
        }

        if (colliders.Length > 0)
        {
            Vector3 targetPosition = new Vector3(closestTarget.transform.position.x,
                      gameObject.transform.position.y,
                       closestTarget.transform.position.z);

            gameObject.transform.LookAt(targetPosition); //�{�̂�G�̕����Ɍ�����

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

    //��𐶐����G�Ɍ����Ďˏo
    private IEnumerator shotArrow(Transform targetPos)
    {
        isShot = false;
        Debug.Log("shot");
        if (numOfArrow < 2)
        {
            Debug.Log("��͈�{");
            Vector3 direction = (targetPos.position - shotPos.position).normalized; //���ˊp�x�v�Z

            //��𐶐�
            GameObject arrow = Instantiate(arrowPrefab, shotPos.position, Quaternion.LookRotation(direction));
            ArrowMove arrowMove = arrow.GetComponent<ArrowMove>();
            arrowMove.SetDirection(direction, arrowDamage); //��ɑ��x�ƃ_���[�W��n��
            yield return new WaitForSeconds(shotArrowInterval); //shotArrowInterval�b�ҋ@
        }
        else
        {
            Debug.Log("��͕����{");
            Vector3 targetDir = (targetPos.position - shotPos.position).normalized; //�G�Ƃ̊p�x���v�Z
            float startAngle = spreadAngle / 2; //��̔��ˈʒu�̐^��
            float angleStep = spreadAngle / (numOfArrow - 1); //�^�񒆂��炸�炷�p�x

            for (int i = 0; i < numOfArrow; i++)
            {
                float angle = startAngle + i * angleStep; //�^�񒆂̔��ˈʒu����p�x�����炷
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

                //��]�����؂�ւ�
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
                Vector3 direction = rotation * targetDir; //�i�ޕ������v�Z

                //�����ˏ�ɐ���
                GameObject arrow = Instantiate(arrowPrefab, shotPos.position, Quaternion.LookRotation(direction));
                ArrowMove arrowMove = arrow.GetComponent<ArrowMove>();
                arrowMove.SetDirection(direction, arrowDamage); //��ɑ��x�ƃ_���[�W��n��
            }
            yield return new WaitForSeconds(shotArrowInterval); //shotArrowInterval�b�ҋ@
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
