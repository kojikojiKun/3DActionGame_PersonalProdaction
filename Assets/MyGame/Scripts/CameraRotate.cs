using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotate : MonoBehaviour
{
    public GameObject player;//player�̃Q�[���I�u�W�F�N�g������ϐ���ݒ�
    public float sensitivility;
    void Update()
    {
        //float my = Mouse.current.delta.ReadValue().y;//�}�E�X�̏c�����̈ړ��ʂ��擾
        float mx = Mouse.current.delta.ReadValue().x;//�J�[�\���̉��̈ړ��ʂ��擾

        /*if (Mathf.Abs(my) > 0.001f)// Y�����Ɉ��ʈړ����Ă���Ώc��]
        {
            transform.RotateAround(player.transform.position, Vector3.right, -my);
        }
        */
        if (Mathf.Abs(mx) > 0.001f) // X�����Ɉ��ʈړ����Ă���Ή���]
        {
            transform.RotateAround(player.transform.position, Vector3.up, mx*sensitivility); 

        }
    }
}
