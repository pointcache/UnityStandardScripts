using UnityEngine;
using System.Collections;
using System;

namespace USS
{
    public enum Axis { x, y, z, all}
    public static class Extensions
    {
        public static Transform FindChildByName
        (this UnityEngine.Transform t, string name)
        {
            return FindRecurs(name, t);
        }

        public static void NullCheckInvoke(this Action del)
        {
            if (del != null)
            {
                del();
            }
        }

        private static Transform FindRecurs
            (string name, Transform current)
        {
            if (current.name == name)
            {
                return current;
            }
            else
            {
                for (int i = 0; i < current.childCount; ++i)
                {
                    Transform found = FindRecurs(name, current.GetChild(i));

                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }
    }

    public static class Utils
    {
        public static void Log(object obj)
        {
            Debug.Log("<color=red>USS: </color>" + obj);
        }

        public static void Log(object obj, GameObject go)
        {
            Debug.Log("<color=red>USS: </color>" + obj, go);
        }
    }

}