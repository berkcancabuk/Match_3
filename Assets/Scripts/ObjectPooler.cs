using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject prefabToPool;
    public int poolSize = 50;

    public List<GameObject> pooledObjects = new List<GameObject>();
    [SerializeField] private int head = 0;
    private Vector3 _resetScale;
    void Awake()
    {
        _resetScale = new Vector3(1f, 1f, 1f);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabToPool,new Vector2(0,10),Quaternion.identity);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }



    public GameObject GetPooledObject()
    {
        
        for (int i = 0; i < poolSize; i++)
        {
            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                pooledObjects[i].gameObject.transform.localScale = _resetScale;
                return pooledObjects[i];

            }

        }
        return null;
    }










}