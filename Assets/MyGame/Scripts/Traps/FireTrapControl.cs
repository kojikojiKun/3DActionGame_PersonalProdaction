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
    [SerializeField] ParticleSystem[] fireParticle; //���̃p�[�e�B�N��(3��)
    [SerializeField] private float radius; //�~�̔��a(���̎˒�����)
    [SerializeField] private float coneAngle; //���g���b�v�̔���p�x
    private float fireStartSpeed; //�p�[�e�B�N���̏����x
    private float fireShotInterval; //�p�[�e�B�N���̍Đ��Ԋu
    private float fireDuration; //�p�[�e�B�N���̎�������
    private float fireDamageInterval; //�����G�Ƀ_���[�W��^����Ԋu
    private float fireDamage; //���̃_���[�W
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
        radius = fireStartSpeed * 0.8f; //�p�[�e�B�N���̏����x�Ŏ˒����������߂�

        for (int i = 0; i < fireParticle.Length; i++)
        {
            var main = fireParticle[i].main;
            main.startSpeed = fireStartSpeed;
            main.duration = fireDuration;
        }
    }

    private void GetEnemies()
    {

        Vector3 origin = originTransform.position; //�J�n�ʒu
        Vector3 forward = originTransform.forward; //������O���ɐݒ�
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

        if (isShot == true)
        { //�G���U���͈͂ɓ�������U��
            StartCoroutine(ShotFire(fireDamage,
                fireDamageInterval,
                targets.ToArray())); //���𔭎�
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

        // �����E��
        Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * forward;
        Gizmos.DrawRay(origin, leftDir * radius);

        // �E���E��
        Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * forward;
        Gizmos.DrawRay(origin, rightDir * radius);
    }*/

    //���𔭎�
    private IEnumerator ShotFire(float damage, float dmgInterval, GameObject[] targets)
    {
        isShot = false;
        
        //�p�[�e�B�N���Đ�
        for (int i = 0; i < fireParticle.Length; i++)
        {
            fireParticle[i].Play();
        }
        Debug.Log("Fire!!!");
        StartCoroutine(DamageContinue(damage, dmgInterval, targets));
        yield return new WaitForSeconds(fireDuration); //�p�[�e�B�N���̍Đ����I���܂őҋ@       
        yield return new WaitForSeconds(fireShotInterval); //fireInterval�b�ҋ@
        isShot = true;
    }

    //�G�Ɍp���I�Ƀ_���[�W��^����
    //�_���[�W�̒l�ƃ_���[�W��^����Ԋu�̒l,�_���[�W��^����Ώۂ�n��
    private IEnumerator DamageContinue(float damage, float dmgInterval, GameObject[] targets)
    {
        float timer = 0;
        while (timer < fireDuration)
        {
            Debug.Log("111111111111111111111111111111");
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
