using UnityEngine;

//�v�[���Ő�������I�u�W�F�N�g�ɃA�^�b�`.
public class PooledObject : MonoBehaviour
{
    private ObjectPool pool;

    public void SetPool(ObjectPool p)
    {
        pool = p;
    }

    public void ReturnToPool()
    {
        if (pool != null)
            pool.ReturnObject(this.gameObject);
        else
            Destroy(this.gameObject);
    }
}
