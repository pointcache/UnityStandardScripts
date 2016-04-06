using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using USS;
namespace USS
{
    /// <summary>
    /// Substitutes unity update methods by receiving just one call for all components if components inherit from USSBase
    /// The idea is to avoid multiple constly update calls and limit to just one per GameObject.
    /// </summary>
    [DisallowMultipleComponent]
    public class USSUpdater : MonoBehaviour
    {
        List<USSBase> toUpdate = new List<USSBase>();

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

        public void Register(USSBase toAdd)
        {
            toUpdate.Add(toAdd);
        }

        public void Unregister(USSBase toAdd)
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