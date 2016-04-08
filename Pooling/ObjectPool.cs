using UnityEngine;
using System.Collections.Generic;
using System;
using USS.Pooling;

public static class ObjectPoolExtensions
{
    public static GameObject InstantiateFromPool(this GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return ObjectPool.Instantiate(prefab, position, rotation);
    }
    public static void Release(this GameObject objthis)
    {
        ObjectPool.Release(objthis);
    }
}

namespace USS.Pooling
{
    public class ObjectPool : MonoBehaviour
    {
        private static ObjectPool _instance;
        public static ObjectPool instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ObjectPool>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ObjectPool");
                        _instance = go.AddComponent<ObjectPool>();
                    }
                }
                return _instance;
            }
        }
        public List<Pool> customPools = new List<Pool>();

        public List<Pool> runtimePools = new List<Pool>();//Read only, just displays pools created on runtime without prior setup."

        Dictionary<GameObject, Pool> pool = new Dictionary<GameObject, Pool>();

        public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return instance._Instantiate(prefab, position, rotation);
        }

        GameObject _Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (pool.ContainsKey(prefab))
            {
                Pool current = pool[prefab];

                return current.Request(position, rotation);
            }
            else
            {
                Pool newpool = new Pool();
                runtimePools.Add(newpool);
                pool.Add(prefab, newpool);
                newpool.MaxObjectsWarning = 100;
                newpool.prefab = prefab;
                return newpool.Request(position, rotation);
            }

        }

        void Awake()
        {
            for (int i = 0; i < customPools.Count; i++)
            {
                if (customPools[i].prefab == null)
                {
                    Debug.LogError("Exists custom object pool without object, clean it up.");
                    continue;
                }

                if (!pool.ContainsKey(customPools[i].prefab))
                {
                    pool.Add(customPools[i].prefab, customPools[i]);
                }
                else
                {
                    Debug.LogError("Trying to add object that is already registered by object pooler.");
                }
            }
        }

        public static void Release(GameObject obj)
        {
            instance._release(obj);
        }

        void _release(GameObject obj)
        {
            ObjectPoolID id = obj.GetComponent<ObjectPoolID>();
            if (id.Free)
            {
                return;
            }
            id.pool.Release(id);
        }
        [Serializable]
        public class Pool
        {
            public override string ToString()
            {
                if (prefab == null)
                    return "Empty Pool";
                return prefab.name;
            }
            public int MaxObjectsWarning = 10;
            public string Keyword;
            public GameObject prefab;

            public int CountFree;
            public int CountInUse;
            public int CountTotal { get { return CountFree + CountInUse; } }
            List<ObjectPoolID> free = new List<ObjectPoolID>();
            List<ObjectPoolID> inUse = new List<ObjectPoolID>();

            GameObject temp;

            public GameObject Request(Vector3 position, Quaternion rotation)
            {

                if (CountFree <= 0)
                {
                    temp = (GameObject)GameObject.Instantiate(prefab, position, rotation);
                    ObjectPoolID obj = temp.AddComponent<ObjectPoolID>();
                    inUse.Add(obj);
                    CountInUse = inUse.Count;

                    obj.pool = this;
                    obj.MyParentTransform = prefab.transform.parent;
                    obj.transform.SetParent(obj.MyParentTransform);
                    if (CountTotal > MaxObjectsWarning)
                    {
                        Debug.LogError("ObjectPool: More than max objects spawned. --- " + prefab.name + " Max obj set to: " + MaxObjectsWarning + " and the pool already has: " + CountTotal);
                    }
                    obj.SetFree(false);
                }
                else
                {
                    ObjectPoolID obj = free[0];


                    free.RemoveAt(0);
                    inUse.Add(obj);
                    obj.transform.SetParent(obj.MyParentTransform);

                    obj.gameObject.transform.position = position;
                    obj.gameObject.transform.rotation = rotation;
                    obj.gameObject.SetActive(true);

                    CountFree = free.Count;
                    CountInUse = inUse.Count;
                    temp = obj.gameObject;
                    obj.SetFree(false);
                }

                return temp;
            }

            public void Release(ObjectPoolID obj)
            {
                inUse.Remove(obj);
                CountInUse = inUse.Count;
                free.Add(obj);

                CountFree = free.Count;
                obj.transform.SetParent(instance.transform);
                obj.gameObject.SetActive(false);
                obj.SetFree(true);
            }


        }
    }
}