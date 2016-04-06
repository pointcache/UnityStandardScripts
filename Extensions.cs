using UnityEngine;
using System.Collections;

namespace USS
{
    public enum Axis { x, y, z, all}
    public static class Extensions
    {

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