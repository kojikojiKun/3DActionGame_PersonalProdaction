using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
public class SpinBladeControl : MonoBehaviour
{
    public static List<SpinBladeControl> allBladeTraps = new List<SpinBladeControl>();
    [SerializeField] TrapStaus trapStaus;
    [SerializeField] TrapType type;

    [Header("SpinBlade")]
    [SerializeField] Transform originTransform;
    [SerializeField] NavMeshAgent bladeAgent;
    [SerializeField] private Transform cetral;
    [SerializeField] private float radius;
    private float bladeSize;
    private float bladeSpeed;
    private float bladeDmgInterval;
    private float bladeDmg;

    private void Awake()
    {
        allBladeTraps.Add(this);
    }

    private void OnDestroy()
    {
        allBladeTraps.Remove(this);
    }

    //TrapStatus�Őݒ肳�ꂽ�l����
    public void SetBladeStatus(float size, float speed, float dmgInterval, float damage)
    {
        bladeSize = size;
        bladeSpeed = speed;
        bladeDmgInterval = dmgInterval;
        bladeDmg = damage;

        originTransform.localScale = new Vector3(bladeSize, originTransform.localScale.y, bladeSize);
        bladeAgent.speed = bladeSpeed;
    }

    private void BladeTrapControl()
    {
        //�ړI�n�ɓ���or�ړI�n�܂ł̋�����2�ȉ�
        if (!bladeAgent.pathPending && bladeAgent.remainingDistance < 2f)
        {
            //���aradius�̉~�̒��̃����_���Ȉʒu���v�Z
            Vector3 randomPos = new Vector3(Random.Range(-radius, radius),
                                            0,
                                            Random.Range(-radius, radius));

            //���aradius�̉~�̒��̃����_���Ȉʒu�ɖړI�n��ݒ�
            bladeAgent.destination = cetral.position + randomPos;
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
        BladeTrapControl();
    }
}
