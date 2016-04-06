using UnityEngine;
using System.Collections;
namespace USS
{
    /// <summary>
    /// Inherit from this to work with custom USSUpdater 
    /// </summary>
    public class USSBase : MonoBehaviour
    {
        USSUpdater updater;
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
                if (!GetComponent<USSUpdater>())
                {
                    updater = gameObject.AddComponent<USSUpdater>();
                }
                else
                {
                    updater = GetComponent<USSUpdater>();
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