using UnityEngine;

public class BreakBuildObject : MonoBehaviour
{
    [SerializeField]
    private int hp=3;
    private void OnTriggerEnter(Collider other)
    {
        //�G�L�����N�^�[�̍U������ɐڐG����ƃ_���[�W���󂯂�
        if (other.CompareTag("EnemyAttackCollider"))
        {
            hp -= 1;
        }

        if (hp <= 0)
        {
            //��������ꂽ���̏���
            Destroy(gameObject);
            //aaaaaaaaaaaaaaaaaaaaaaaaaaaa
        }
    }
}
