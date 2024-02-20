using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject prefabToPool;
    public int poolSize = 50;

    public List<GameObject> pooledObjects = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabToPool,new Vector2(0,10),Quaternion.identity);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
}