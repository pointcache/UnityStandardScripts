using UnityEngine;
using System.Collections.Generic;

namespace USS.Pooling
{
    public class ObjectPoolID : MonoBehaviour
    {
        public Transform MyParentTransform;
        public ObjectPool.Pool pool { get; set; }
        public bool Free { get { return isFree; } }
        bool isFree { get; set; }
        public int thisID { get { return GetInstanceID(); } }
        public int prefabID { get { if (MyParentTransform == null) return 0; return MyParentTransform.gameObject.GetInstanceID(); } }

        public void SetFree(bool state)
        {
            isFree = state;
        }

        public bool GetFree()
        {
            return isFree;
        }
    }
}