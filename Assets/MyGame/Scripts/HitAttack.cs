using UnityEngine;
using System.Collections.Generic;
public class HitAttack : MonoBehaviour
{
    //�U���A�j���[�V�����̓����蔻��
    [SerializeField]
    List<Collider> attackColliders = new List<Collider>(); 

    //�A�j���[�V�����̖��O�ƑΉ����鐔���i�[����
    [SerializeField]
    SerializableDictionary<string, int> attackColliderDictionary = null;
    public PlayerController playerController;

    //�_���[�W�{��
    public float dmgMultipul;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    //�U���A�j���[�V�������Q�Ƃ��ē����蔻����o��
    public void ColliderSet(string animName)
    {
        if (attackColliderDictionary.TryGetValue(animName, out int index))
        {          
            dmgMultipul = playerController.comboDmgMultipul[index];
            Debug.Log($"animName{animName} index{index} dmgMultipul{dmgMultipul}");
        }

        if (attackColliderDictionary.ContainsKey(animName))
        {
            attackColliders[attackColliderDictionary[animName]].enabled = true;
        }
    }
    
    //�����蔻��폜
    public void ColliderRemove(string animName)
    {
        //�_���[�W�{����������
        dmgMultipul = 0;

        if(attackColliderDictionary.TryGetValue(animName,out int index))
        {
            Debug.Log($"animName{animName} index{index}");
        }

        if (attackColliderDictionary.ContainsKey(animName))
        {
            attackColliders[attackColliderDictionary[animName]].enabled = false;
        }
    }
}
