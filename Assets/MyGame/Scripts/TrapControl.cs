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
    public TrapType type; //�g���b�v�̎�ނ��C���X�y�N�^�[�Őݒ�   
    [SerializeField] TrapStaus trapStaus;
    [SerializeField] private Transform originTransForm; //�����蔻��̊J�n�ʒu
    
    [SerializeField] private LayerMask targetMask; //�Ώۂ̃��C���[
    private bool isShot = true; //�U���t���O

    [Header("Fire")]
    [SerializeField] ParticleSystem[] fireParticle; //���̃p�[�e�B�N��(3��)
    [SerializeField] private float radius; //�~�̔��a(�˒�����)
    [SerializeField] private float coneAngle; //���g���b�v�̔���p�x
    private float fireStartSpeed; //�p�[�e�B�N���̏����x
    private float fireShotInterval; //�p�[�e�B�N���̍Đ��Ԋu
    private float fireDuration; //�p�[�e�B�N���̎�������
    private float fireDamageInterval; //�����G�Ƀ_���[�W��^����Ԋu
    private float fireDamage; //���̃_���[�W
    
    [Header("CrossBow")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shotPos;
    private float shotArrowInterval; //��̔��ˊԊu
    private float arrowDamage; //��̃_���[�W;
    private float shotRange; //�˒�����
    private int numOfArrow; //��̐�
    private float spreadAngle; //��̊g�U����p�x

    [Header("SpinBlade")]
    [SerializeField] NavMeshAgent bladeAgent;
    private float bladeSize;
    private float bladeSpeed;
    private float bladeDmgInterval;
    private float bladeDmg;

    //TrapStatus�Őݒ肳�ꂽ�l����
    public void SetFireTrapStasus(float startSpeed, float interval, float duration, float dmgInterval, float damage)
    {
        fireStartSpeed = startSpeed;
        fireShotInterval = interval;
        fireDuration = duration;
        fireDamageInterval = dmgInterval;
        fireDamage = damage;
        radius = fireStartSpeed * 0.8f; //�p�[�e�B�N���̏����x�Ŏ˒����������߂�

        for(int i = 0; i < fireParticle.Length; i++)
        {
            var main = fireParticle[i].main;
            main.startSpeed = fireStartSpeed;
            main.duration = fireDuration;
        }
    }

    private void FireControl()
    {

        Vector3 origin = originTransForm.position; //�J�n�ʒu
        Vector3 forward = originTransForm.forward; //������O���ɐݒ�
        Collider[] colliders = Physics.OverlapSphere(origin, radius, targetMask); //�~�̒��̃R���C�_�[�����ׂĎ擾(targetMask�ȊO�͏��O����)
        List<GameObject> targets = new List<GameObject>(); //�͈͓��̓G���i�[���郊�X�g���쐬

        foreach (Collider col in colliders)
        {
            Vector3 toTarget = (col.transform.position - origin).normalized; //�^�[�Q�b�g�Ƃ̋������v�Z�����K��
            float angle = Vector3.Angle(forward, toTarget); //�O�����Ƃ̊p�x���v�Z

            //coneAngle���Ȃ画�肷��
            if (angle < coneAngle / 2f)
            {
                targets.Add(col.gameObject); //�͈͓��̓G������z����쐬
                for (int i = 0; i < colliders.Length; i++)
                {
                    targets[i] = colliders[i].gameObject; //�z����̓G���擾
                }
            }
        }

        if ( isShot == true) //�G���U���͈͂ɓ�������U��
            StartCoroutine(ShotFire(fireDamage,
                fireDamageInterval,
                targets.ToArray())); //���𔭎�
    }

    //���𔭎�
    private IEnumerator ShotFire(float damage, float dmgInterval, GameObject[] targets)
    {
        isShot = false;
        yield return new WaitForSeconds(fireShotInterval); //fireInterval�b�ҋ@
        //�p�[�e�B�N���Đ�
        for (int i = 0; i < fireParticle.Length; i++)
        {
            fireParticle[i].Play();
        }
        Debug.Log("Fire!!!");
        StartCoroutine(DamageContinue(damage, dmgInterval, targets));
        yield return new WaitForSeconds(fireDuration); //�p�[�e�B�N���̍Đ����I���܂őҋ@       

        isShot = true;
    }

    //�G�Ɍp���I�Ƀ_���[�W��^����(�u���[�h�g���b�v�A���g���b�v�p)
    //�_���[�W�̒l�ƃ_���[�W��^����Ԋu�̒l,�_���[�W��^����Ώۂ�n��
    private IEnumerator DamageContinue(float damage, float dmgInterval, GameObject[] targets)
    {
        float timer = 0;
        while (timer < fireDuration)
        {
            foreach (GameObject enemy in targets)
            {
                if (enemy == null) continue; //null�`�F�b�N

                EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
                if (enemyStatus != null) //null�`�F�b�N
                {
                    enemyStatus.TakeDamageEnemy(damage); //�G�Ƀ_���[�W��^����
                }

            }
            yield return new WaitForSeconds(dmgInterval); //dmgInterval�b�ҋ@
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

    private IEnumerator shotArrow(Transform targetPos)
    {
        isShot = false;
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