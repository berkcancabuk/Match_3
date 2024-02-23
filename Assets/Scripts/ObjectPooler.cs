using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject prefabToPool;
    public int poolSize = 50;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();
    [SerializeField] private int head = 0;
    private Vector3 _resetScale;

    void Awake()
    {
        _resetScale = new Vector3(1f, 1f, 1f);
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabToPool,new Vector2(0,10),Quaternion.identity);
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public GameObject GetObjectFromQueue()
    {
        if (poolQueue.Count > 0)
        {
            var obj = poolQueue.Dequeue();
            obj.transform.localScale = _resetScale;
            return obj;
        }
        return null;
    }

    public void ReturnObjectToQueue(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }




}