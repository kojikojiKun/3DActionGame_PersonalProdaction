using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolType
{
    public string name;
    public ObjectPool pool;
}
public class PoolManager : MonoBehaviour
{
    public List<PoolType> enemies;
    public List<PoolType> traps;

    // “G‚ð–¼‘O‚ÅŽæ“¾
    public GameObject GetEnemy(string name)
    {
        var type = enemies.Find(e => e.name == name);
        if (type != null)
        {
            return type.pool.GetObject();
        }
        Debug.LogWarning($"Enemy '{name}' not found in pool!");
        return null;
    }

    // ã©‚ð–¼‘O‚ÅŽæ“¾
    public GameObject GetTrap(string name)
    {
        var type = traps.Find(t => t.name == name);
        if (type != null)
        {
            return type.pool.GetObject();
        }
        Debug.LogWarning($"Trap '{name}' not found in pool!");
        return null;
    }
}
