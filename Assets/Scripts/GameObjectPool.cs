using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool: MonoBehaviour
{
    public static GameObjectPool SharedInstance;


    private List<GameObject> pool;
    public GameObject GameObject;
    public int PoolSize;
    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        pool = new List<GameObject>(PoolSize);
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject obj = Instantiate(GameObject);
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
