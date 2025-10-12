using UnityEngine;
using System.Collections;

namespace Benjathemaker
{
    public class SimpleGemsAnim : MonoBehaviour
    {
        private bool isRotating = false;
        public bool rotateX = false;
        public bool rotateY = false;
        public bool rotateZ = false;
        public float rotationSpeed = 90f; // Degrees per second

        private bool isFloating = false;
        public bool useEasingForFloating = false; // Separate toggle for floating ease
        public float floatHeight = 1f; // Max height displacement
        public float floatSpeed = 1f;
        private Vector3 initialPosition;
        private float floatTimer;

        private enum ObjectType
        {
            Item,
            EXP_Small,
            EXP_Midium,
            EXP_Large
        }
        [SerializeField] private ObjectType type;
        
        [SerializeField] private float giveExpValue;
        private Rigidbody rb;
        private GameObject target;

        [SerializeField] private float dropForce; //�A�C�e������o����Ƃ��̗�
        private float deleteTime = 30f; //�A�C�e����������܂ł̎���
        private float spowmTime; //�A�C�e�����o����������
        private float defaultFloatSpeed;
        private bool getPlayer = false;


        void Start()
        {
            rb = GetComponent<Rigidbody>();
            defaultFloatSpeed = floatSpeed;
           // target = PlayerController.instance.gameObject;
            Injection();
        }

        private void Injection()
        {
            if (rb != null)
            {
                // �����_���ȕ����ɗ͂�������
                Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
                randomDirection.y = Mathf.Abs(randomDirection.y); // ����������������i�����h�~�j

                rb.AddForce(randomDirection * dropForce, ForceMode.Impulse);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Field") && rb.isKinematic == false)
            {
                spowmTime = Time.time;

                //����������~
                rb.isKinematic = true;
                
                //�I�u�W�F�N�g���ړ������邽�߂̏����ʒu��ݒ�
                initialPosition = transform.position+Vector3.up*0.5f;

                //�I�u�W�F�N�g�ړ����J�n
                isFloating = true;
                isRotating = true;
            }

            if (other.CompareTag("GetItemRange"))
            {
                getPlayer = true;
            }

            
            if (other.CompareTag("Player"))
            {
                //�o���l��n��
                if (type != ObjectType.Item)
                {
                    LevelUp.instance.GetExp(giveExpValue);
                }
                else
                {
                    //�������Ă���A�C�e���̑��ʂ𑝂₷
                    LevelUp.instance.GetItem();
                }
                Destroy(this.gameObject);
            }
        }

        void Update()
        { 
            //�I�u�W�F�N�g����]������
            if (isRotating)
            {
                Vector3 rotationVector = new Vector3(
                    rotateX ? 1 : 0,
                    rotateY ? 1 : 0,
                    rotateZ ? 1 : 0
                );
                transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
            }

            //�I�u�W�F�N�g���㉺������
            if (isFloating)
            {
                floatTimer += Time.deltaTime * floatSpeed;
                float t = (Mathf.Sin(floatTimer) + 1f) / 2f;

                transform.position = initialPosition + new Vector3(0, t * floatHeight, 0);
            }

            //�v���C���[�̃A�C�e���擾�͈͂ɓ���ƃv���C���[�܂ňړ�����
            if (getPlayer == true && rb.isKinematic == true)
            {
                if (target != null)
                {
                    isFloating = false;

                   // Debug.LogWarning("target finded");
                    transform.position = Vector3.MoveTowards(
                        transform.position,
                        new Vector3(target.transform.position.x,
                        target.transform.position.y,
                        target.transform.position.z),
                        6f * Time.deltaTime);
                }
                else
                {
                    Debug.LogWarning("error");
                }
            }

            //������܂ł̎��Ԃ��߂Â�����㉺����X�s�[�h�𑁂߂�
            if (Time.time - spowmTime >= deleteTime*0.8f)
            {
                if (floatSpeed == defaultFloatSpeed)
                {
                    floatSpeed *= 3f;
                }
            }

            //��莞�Ԍo�߂ŃI�u�W�F�N�g�폜
            if (Time.time - spowmTime >= deleteTime)
            {
                Destroy(gameObject);
            }
        }
    }
}

