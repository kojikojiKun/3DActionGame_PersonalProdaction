using UnityEngine;

public class Test : MonoBehaviour
{
    public PoolManager poolManager;
    private float timer;
    private bool spowned = false;
    void Update()
    {
        if (timer > 5 && spowned==false) // „©ÉXÉ|Å[Éì
        {
            GameObject trap = poolManager.GetTrap("Item");
            trap.transform.position = GetTrapPosition();
            spowned = true;
        }

    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
    }

    Vector3 GetTrapPosition()
    {
        return new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
    }
}
