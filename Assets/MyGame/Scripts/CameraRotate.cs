using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    public GameObject player;//playerのゲームオブジェクトを入れる変数を設定
    public float sensitivility;
    void Update()
    {
        //float my = Mouse.current.delta.ReadValue().y;//マウスの縦方向の移動量を取得
        float mx = Mouse.current.delta.ReadValue().x;//カーソルの横の移動量を取得

        /*if (Mathf.Abs(my) > 0.001f)// Y方向に一定量移動していれば縦回転
        {
            transform.RotateAround(player.transform.position, Vector3.right, -my);
        }
        */
        if (Mathf.Abs(mx) > 0.001f) // X方向に一定量移動していれば横回転
        {
            transform.RotateAround(player.transform.position, Vector3.up, mx*sensitivility); 

        }
    }
}
