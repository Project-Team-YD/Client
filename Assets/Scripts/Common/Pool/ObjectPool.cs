using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class ObjectPool<T> where T : IPoolable
{
    private Queue<T> unusedObjectQueue = null;

    private GameObject prefab = null;

    public async void Initialize(string _prefabPath, int _initializeCreateCount = 8)
    {
        string prefabPath = Path.Combine("", _prefabPath);
        //.. TODO :: Addressable / 비동기 적용
        prefab = Resources.Load(prefabPath, typeof(GameObject)) as GameObject;

        for(int i = 0; i < _initializeCreateCount; i++)
        {
            EnqueueObject(CreateObject());
        }
    }

    public ObjectPool()
    {
        unusedObjectQueue = new Queue<T>();
        unusedObjectQueue.Clear();
    }

    ~ObjectPool()
    {
        OnRelease();
    }

    public void EnqueueObject(T _object)
    {
        //.. FIXME? :: 여기서 호출 하지 말까?
        _object.OnDeactivate();
        unusedObjectQueue.Enqueue(_object);
    }

    public T GetObject()
    {
        T getObject;
        if(unusedObjectQueue.Count <= 0)
        {
            getObject = CreateObject();
        }
        else
        {
            getObject = unusedObjectQueue.Dequeue();
        }

        return getObject;
    }

    private T CreateObject()
    {
        GameObject newObject = GameObject.Instantiate(prefab);
        var componenet = newObject.GetComponent<T>();

        return componenet;
    }

    public void OnRelease()
    {
        unusedObjectQueue.Clear();
        unusedObjectQueue = null;
    }
}
