using UnityEngine;
using UnityEditor;
using System.Collections;
using USS.Levels;
using USS;

/// <summary>
/// This class explains how you can override the default level Launch behavior.
/// 1. The override happens in editor static class in folder Editor
/// 2. This class must have [InitializeOnLoad] so that the constructor is being called every time you press play
/// 3. Add your callback in a method using provided methods as guides.
/// </summary>
[InitializeOnLoad]
public static class OverrideCallbackTest  {

    public static void OverrideLevelLoadCallBack()
    {
        USSEditorPrefs.prefs.OverrideEditorLevelLoadEvent(TestCallback);
        EditorUtility.SetDirty(USSEditorPrefs.prefs);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Constructor registers the callback each time the assembly is realoaded, thus its "permanent"
    /// uncomment this line to see how it works.
    /// </summary>
    static OverrideCallbackTest()
    {
       // OverrideLevelLoadCallBack();
    }


    static void TestCallback(Level lev)
    {
        Debug.Log(lev.name);
    }
}
