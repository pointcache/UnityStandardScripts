using UnityEngine;
using System.Collections;
namespace USTD
{
    /// <summary>
    /// Inherit from this to work with custom USTDUpdater 
    /// </summary>
    public class USTDBase : MonoBehaviour
    {
        USTDUpdater updater;
        public virtual void OnEnable()
        {
            CheckUpdater();
            Register();
        }

        public virtual void OnDisable()
        {
            Unregister();
        }

        void CheckUpdater()
        {
            if (updater == null)
            {
                if (!GetComponent<USTDUpdater>())
                {
                    updater = gameObject.AddComponent<USTDUpdater>();
                }
                else
                {
                    updater = GetComponent<USTDUpdater>();
                }
            }
        }

        void Register()
        {
            updater.Register(this);
        }

        void Unregister()
        {
            updater.Unregister(this);
        }

        public virtual void stdUpdate()
        {

        }
    }
}