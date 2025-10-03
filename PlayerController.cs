using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum WhichTypeOfCharacter
{
    StandardType, //標準ステータスのキャラ
    SpeedType, //スピードに特化したキャラ
    PowerType,　//攻撃、耐久力に特化したキャラ
}

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //プレイヤーのステータス
    [HideInInspector]
    private WhichTypeOfCharacter type;
    [HideInInspector]
    public string id;
    [HideInInspector]
    public float hp;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float attackSpeed;
    [HideInInspector]
    public float attackPower;

    [Header("ガード時のダメージ軽減率")]
    public float dmgReduction;

    [Header("コンボ攻撃ごとのダメージ倍率")]
    public float[] comboDmgMultipul;

    public float invincibleTime; //攻撃を受けない無敵時間
    private float defaultSpeed;
    private bool isGuard = false;
    private bool speedDown = false;
    private bool playerTookDmg=false;
    private bool playerDied = false;

    [Header("ジャンプするときの初速"), SerializeField]
    private float jumpForce;
    [Header("重力加速度"), SerializeField]
    private float gravity;
    [Header("落下した時の速さ"), SerializeField]
    private float fallSpeed;
    [Header("落下の初速"), SerializeField]
    private float initFallSpeed;
    [Header("カメラ"), SerializeField]
    private Camera targetCamera;
    [Header("カメラの感度"), SerializeField]
    private float sensitivility;

    private Transform _transform; //プレイヤーのTransform
    private CharacterController characterController;
    private PlayerAnimation playerAnimation;
    private PlayerInput playerInput;

    public Vector2 inputMove;
    private float verticalVelocity;
    private float turnVelosity;
    public bool isGroundedPlayer; //プレイヤーが着地しているか

    public void SetStatus()
    {
        //キャラクタータイプを設定
        switch (id)
        {
            case "StandardType":
                type = WhichTypeOfCharacter.StandardType;
                break;
            case "SpeedType":
                type = WhichTypeOfCharacter.SpeedType;
                break;
            case "PowerType":
                type = WhichTypeOfCharacter.PowerType;
                break;
        }

        //キャラのステータスを読み込んで設定
        hp = PlayerPrefs.GetFloat(id + "_HP", 0);

        speed = PlayerPrefs.GetFloat(id + "_Speed", 0);
        defaultSpeed = speed; //スピードの初期値を保存

        attackSpeed = PlayerPrefs.GetFloat(id + "_AS", 0);
        attackPower = PlayerPrefs.GetFloat(id + "_AP", 0);

        if (hp == 0 || speed == 0 || attackSpeed == 0 || attackPower == 0)
        {
            Debug.Log("ステータスを読み込みできませんでした");
        }
        else
        {
            Debug.Log(type);
            Debug.Log($"HP:{hp}//SP{speed}//AS{attackSpeed}//AP{attackPower}");
        }
    }

    //プレイヤーの移動アクション
    public void OnMove(InputAction.CallbackContext context)
    {
        //入力値を保持
        inputMove = context.ReadValue<Vector2>();
        //Debug.Log(inputMove);
    }

    //プレイヤーを移動させる処理
    void MoveLogic()
    {
        //ガード中は移動スピード低下
        if (isGuard == true && speedDown==false)
        {
            float gSpeed = speed * 0.5f;
            speed = gSpeed;
            speedDown = true;
        }
        
        if(isGuard==false)
        {
            speed = defaultSpeed;
            speedDown = false;
        }

        var isGrounded = characterController.isGrounded;

        if (isGrounded && !isGroundedPlayer)
        {
            // 着地する瞬間に落下の初速を指定しておく
            verticalVelocity = -initFallSpeed;
        }
        else if (!isGrounded)
        {
            // 空中にいるときは、下向きに重力加速度を与えて落下させる
            verticalVelocity -= gravity * Time.deltaTime;

            // 落下する速さ以上にならないように補正
            if (verticalVelocity < -fallSpeed)
                verticalVelocity = -fallSpeed;
        }

        isGroundedPlayer = isGrounded;

        // カメラの向き（角度[deg]）取得
        var cameraAngleY = targetCamera.transform.eulerAngles.y;

        // 操作入力と鉛直方向速度から、現在速度を計算
        var moveVelocity = new Vector3(
            inputMove.x * speed,
            verticalVelocity,
            inputMove.y * speed
        );

        // カメラの角度分だけ移動量を回転
        moveVelocity = Quaternion.Euler(0, cameraAngleY, 0) * moveVelocity;

        // 現在フレームの移動量を移動速度から計算
        var moveDelta = moveVelocity * Time.deltaTime;

        // CharacterControllerに移動量を指定し、オブジェクトを動かす
        characterController.Move(moveDelta);

        if (inputMove != Vector2.zero)
        {
            Vector2 moveInput = inputMove;

            // 入力ベクトルがどの方向を向いているかの角度（正面＝0度、右＝90度、後ろ＝180度、左＝-90度）
            float inputAngle = Vector2.SignedAngle(Vector2.up, moveInput);

            // 正面方向と比べて「後ろ」かどうかを判断
            bool isBackInput = Mathf.Abs(inputAngle) > 135f;

            if (isBackInput)
            {
                moveInput *= -1f;
            }

            if (!isBackInput)
            {
                // 操作入力からy軸周りの目標角度[deg]を計算
                var targetAngleY = -Mathf.Atan2(moveInput.y, moveInput.x)
                  * Mathf.Rad2Deg + 90;
                // カメラの角度分だけ振り向く角度を補正
                targetAngleY += cameraAngleY;

                // イージングしながら次の回転角度[deg]を計算
                var angleY = Mathf.SmoothDampAngle(
                    _transform.eulerAngles.y,
                    targetAngleY,
                    ref turnVelosity,
                    sensitivility
                );

                // オブジェクトの回転を更新
                _transform.rotation = Quaternion.Euler(0, angleY, 0);
            }
        }
    }

    //プレイヤーのジャンプアクション
    public void OnJump(InputAction.CallbackContext context)
    {

        //ボタンが押されているかつプレイヤーが地面に着地しているときだけジャンプ可能
        if (!context.performed || !characterController.isGrounded)
        {
            return;
        }
        else
        {
            verticalVelocity = jumpForce;
            Debug.Log("ジャンプ");
            playerAnimation.JumpAnimation();
        }
    }

    public enum ComboState { Combo1, Combo2, Combo3 }
    public ComboState comboState = ComboState.Combo1;

    private float pressButtonStartTime; //攻撃ボタンが押された時間を記録
    [Header("コンボの受付時間")]
    public float pressButtonInterval;

    //攻撃アニメーションを再生
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return; 
        
        if (playerAnimation.comboFinished == true)
        {
            Debug.Log("Attack!!");
            float currentTime = Time.time;

            //ボタンが押された時間がコンボ受付時間より短ければコンボ攻撃アニメーション再生

            switch (comboState)
            {
                case ComboState.Combo1:
                    pressButtonStartTime = currentTime;
                    playerAnimation.AttackAnimation();
                    comboState = ComboState.Combo2;
                    break;
                case ComboState.Combo2:
                    if (currentTime - pressButtonStartTime <= pressButtonInterval)
                    {
                        pressButtonStartTime = currentTime;
                        playerAnimation.AttackAnimation();
                        comboState = ComboState.Combo3;
                    }
                    else
                    {
                        comboState = ComboState.Combo1;
                        playerAnimation.AttackAnimation();
                    }
                    break;
                case ComboState.Combo3:

                    if (currentTime - pressButtonStartTime <= pressButtonInterval && isGuard == false)
                    {
                        playerAnimation.AttackAnimation();
                        comboState = ComboState.Combo1;
                    }
                    else
                    {
                        comboState = ComboState.Combo1;
                        playerAnimation.AttackAnimation();
                    }
                    break;
            }
        }
    }

    //ガードアクション
    public void OnGuard(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isGuard = true;
            playerAnimation.GuardAnimation(isGuard);
        }

        if (context.canceled)
        {
            isGuard = false;
            playerAnimation.GuardAnimation(isGuard);
        }
    }

    //todoプレイヤーを強化
    public void ReinforcePlayer()
    {
        if (playerDied == true) return;
    }

    //プレイヤーのHPを減らす:HP<=0になるとDead()に遷移
    public void TakeDamage(float dmg)
    {
        if (playerDied == true) return;

        //ダメージアニメーション再生
        playerAnimation.TakeDamageAnimation();

        playerTookDmg = true;

        //ガード中はダメージを軽減
        if (isGuard == true) dmg /= dmgReduction;

        hp = Mathf.RoundToInt((hp -= dmg));

        //HPが０以下になると死亡
        if (hp <= 0)
        {
            hp = 0;
            Dead();
        }

        Debug.Log($"敵から{dmg}のダメージを受けた...のこりHPは{hp}");
        StartCoroutine(Invincible());
    }

    //無敵時間の間ダメージ処理をしない
    private IEnumerator Invincible()
    {
        yield return new WaitForSeconds(invincibleTime);
        
        playerTookDmg = false;
    }

    //todoプレイヤーの死亡処理
    public void Dead()
    {
        playerDied = true;

        //InputActionを停止
        playerInput.enabled = false;

        //死亡アニメーションを再生
        playerAnimation.PlayerDied();

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public static PlayerController instance;
    private void Awake()
    {
        Cursor.lockState=CursorLockMode.Locked;
        _transform = transform;
        characterController = GetComponent<CharacterController>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerInput = GetComponent<PlayerInput>();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //何のキャラが選択されたかを読み込む
        id = PlayerPrefs.GetString("StatusID", "");
        if (string.IsNullOrEmpty(id))
        {
            Debug.Log("キャラのタイプが不明です");
            return;
        }
        Debug.Log($"{id}:が選択された");

        LevelUp.instance.player = gameObject;
        SetStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        //敵キャラクターの攻撃判定に接触するとダメージを受ける
        if (other.CompareTag("EnemyAttackCollider"))
        {
            EnemyStatus enemyStatus = other.GetComponentInParent<EnemyStatus>();
            if (playerTookDmg == false) TakeDamage(enemyStatus.attackPower);          
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerDied == true) return;
        MoveLogic();
    }
}
