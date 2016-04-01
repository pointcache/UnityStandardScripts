using UnityEngine;
using System.Collections;

namespace USTD
{
    public enum Axis { x, y, z, all}
    public static class Extensions
    {

    }

    public static class Utils
    {
        public static void Log(object obj)
        {
            Debug.Log("<color=red>USTD: </color>" + obj);
        }

        public static void Log(object obj, GameObject go)
        {
            Debug.Log("<color=red>USTD: </color>" + obj, go);
        }
    }

}