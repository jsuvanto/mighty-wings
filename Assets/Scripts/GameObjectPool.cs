using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool
{
    private List<GameObject> pool;
    public GameObject GameObject;
    public int PoolSize;

    
    public GameObjectPool(GameObject gameObject, int poolSize)
    {
        GameObject = gameObject;
        PoolSize = poolSize;

        pool = new List<GameObject>(PoolSize);
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject obj = Object.Instantiate(GameObject);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }
        return null;
    }
}
