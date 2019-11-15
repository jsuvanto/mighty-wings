using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> pool;
    readonly private GameObject poolableObject;
    readonly private int poolSize;

    public ObjectPool(GameObject poolableObject, int poolSize) {
        this.poolableObject = poolableObject;
        this.poolSize = poolSize;
    }

    // Start is called before the first frame update
    void Start()
    {
        pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(poolableObject);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        return pool.Dequeue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
