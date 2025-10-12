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

        [SerializeField] private float dropForce; //アイテムを放出するときの力
        private float deleteTime = 30f; //アイテムが消えるまでの時間
        private float spowmTime; //アイテムが出現した時間
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
                // ランダムな方向に力を加える
                Vector3 randomDirection = UnityEngine.Random.onUnitSphere;
                randomDirection.y = Mathf.Abs(randomDirection.y); // 上向き成分を強調（落下防止）

                rb.AddForce(randomDirection * dropForce, ForceMode.Impulse);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Field") && rb.isKinematic == false)
            {
                spowmTime = Time.time;

                //物理挙動停止
                rb.isKinematic = true;
                
                //オブジェクトを移動させるための初期位置を設定
                initialPosition = transform.position+Vector3.up*0.5f;

                //オブジェクト移動を開始
                isFloating = true;
                isRotating = true;
            }

            if (other.CompareTag("GetItemRange"))
            {
                getPlayer = true;
            }

            
            if (other.CompareTag("Player"))
            {
                //経験値を渡す
                if (type != ObjectType.Item)
                {
                    LevelUp.instance.GetExp(giveExpValue);
                }
                else
                {
                    //所持しているアイテムの総量を増やす
                    LevelUp.instance.GetItem();
                }
                Destroy(this.gameObject);
            }
        }

        void Update()
        { 
            //オブジェクトを回転させる
            if (isRotating)
            {
                Vector3 rotationVector = new Vector3(
                    rotateX ? 1 : 0,
                    rotateY ? 1 : 0,
                    rotateZ ? 1 : 0
                );
                transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
            }

            //オブジェクトを上下させる
            if (isFloating)
            {
                floatTimer += Time.deltaTime * floatSpeed;
                float t = (Mathf.Sin(floatTimer) + 1f) / 2f;

                transform.position = initialPosition + new Vector3(0, t * floatHeight, 0);
            }

            //プレイヤーのアイテム取得範囲に入るとプレイヤーまで移動する
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

            //消えるまでの時間が近づいたら上下するスピードを早める
            if (Time.time - spowmTime >= deleteTime*0.8f)
            {
                if (floatSpeed == defaultFloatSpeed)
                {
                    floatSpeed *= 3f;
                }
            }

            //一定時間経過でオブジェクト削除
            if (Time.time - spowmTime >= deleteTime)
            {
                Destroy(gameObject);
            }
        }
    }
}

