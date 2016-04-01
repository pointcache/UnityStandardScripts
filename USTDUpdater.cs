using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using USTD;
namespace USTD
{
    [DisallowMultipleComponent]
    public class USTDUpdater : MonoBehaviour
    {
        List<USTDBase> toUpdate = new List<USTDBase>();

        // Update is called once per frame
        void Update()
        {
            int c = toUpdate.Count;
            for (int i = 0; i < c; i++)
            {
                //in case they fail to unregister for some reason
                if(toUpdate[i] == null)
                {
                    Utils.Log("stdUpdater got null ref, something bad happened", gameObject);
                    continue;
                }
                if (toUpdate[i].isActiveAndEnabled)
                {
                    toUpdate[i].stdUpdate();
                }
            }
        }

        public void Register(USTDBase toAdd)
        {
            toUpdate.Add(toAdd);
        }

        public void Unregister(USTDBase toAdd)
        {
            toUpdate.Remove(toAdd);

            //Cleanup if no more needed, to keep Update methods count to minimum
            if(toUpdate.Count == 0)
            {
                Destroy(this);
            }
        }
    }
}