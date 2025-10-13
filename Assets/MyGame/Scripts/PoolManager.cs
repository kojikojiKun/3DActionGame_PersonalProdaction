using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

[System.Serializable]
public class PoolType
{
    [SerializeField] string name;
    [SerializeField] ObjectPool pool;

    public string GetName => name;
    public ObjectPool GetPool=>pool;
}
public class PoolManager : MonoBehaviour
{
    [SerializeField] List<PoolType> enemyPools;
    [SerializeField] List<PoolType> trapPools;

    private Dictionary<string,ObjectPool> enemyDict;
    private Dictionary<string, ObjectPool> trapDict;

    private void Awake()
    {
        enemyDict = new Dictionary<string,ObjectPool>();
        trapDict = new Dictionary<string,ObjectPool>();
        foreach(var ePools in enemyPools)
        {
            enemyDict[ePools.GetName] = ePools.GetPool;
        }

        foreach (var tPools in trapPools)
        {
            enemyDict[tPools.GetName] = tPools.GetPool;
        }
    }

    // “G‚ð–¼‘O‚ÅŽæ“¾
    public GameObject GetEnemy(string name)
    {
        Debug.Log("aaaaaaaaaaaaaaaaaa");
        if(enemyDict.TryGetValue(name, out ObjectPool pool))
        {
            return pool.GetObject();
        }
        Debug.LogWarning($"Enemy '{name}' not found in pool!");
        return null;
    }
    

    // ã©‚ð–¼‘O‚ÅŽæ“¾
    public GameObject GetTrap(string name)
    {
        if (trapDict.TryGetValue(name, out ObjectPool pool))
        {
            return pool.GetObject();
        }
        Debug.LogWarning($"Trap '{name}' not found in pool!");
        return null;
    }
}
