using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System;
using USS.Levels;
using UnityEngine.Events;

namespace USS
{
    public class USSEditorPrefs : ScriptableObject
    {
        //Sorft of singleton
        private static USSEditorPrefs _prefs;
        public static USSEditorPrefs prefs
        {
            get
            {
                if (_prefs == null)
                {
                    string[] GUID = AssetDatabase.FindAssets("t:USSEditorPrefs");
                    if (GUID.Length > 1)
                    {
                        Debug.LogError("<color=red>MAJOR VIOLATION - YOU CANT HAVE MORE THAN 1 OnEdgeEditorPrefs!</color>");
                        return null;
                    }
                    string path = AssetDatabase.GUIDToAssetPath(GUID[0]);
                    _prefs = (USSEditorPrefs)AssetDatabase.LoadAssetAtPath(path, typeof(USSEditorPrefs));


                    
                }
                return _prefs;
            }
        }

        private static USSEditorVars _vars;
        public static USSEditorVars vars
        {
            get
            {
                if (_vars == null)
                {
                    string[] GUID = AssetDatabase.FindAssets("t:USSEditorVars");
                    if (GUID.Length > 1)
                    {
                        Debug.LogError("<color=red>MAJOR VIOLATION - YOU CANT HAVE MORE THAN 1 OnEdgeEditorVars!</color>");
                        return null;
                    }
                    string path = AssetDatabase.GUIDToAssetPath(GUID[0]);
                    _vars = (USSEditorVars)AssetDatabase.LoadAssetAtPath(path, typeof(USSEditorVars));
                }
                return _vars;
            }
        }
        
        [SerializeField]
        private UnityAction<Level> LevelLaunchOverrideCallback;
        /// <summary>
        /// Scenes that were opened before we launched level from editor
        /// </summary>
        public List<string> PreviousScenes;
        [SerializeField]
        public List<SceneSetupWrapper> sceneSetup;
        public bool Override;
        public UnityAction<Level> GetLevelLaunchOverrideCallback()
        {
            return LevelLaunchOverrideCallback;
        }

        /// <summary>
        /// This acts as an interface to change the behavior of level launcher.
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void OverrideEditorLevelLoadEvent(UnityAction<Level> callback)
        {
            LevelLaunchOverrideCallback = callback;
        }

        public void RestoreEditorLevelLoadEvent()
        {
            LevelLaunchOverrideCallback = null;
        }

        public void StoreSceneSetup(SceneSetup[] setup)
        {
            if (sceneSetup == null)
                sceneSetup = new List<SceneSetupWrapper>();
            sceneSetup.Clear();

            for (int i = 0; i < setup.Length; i++)
            {
                SceneSetup s = setup[i];
                sceneSetup.Add(new SceneSetupWrapper(s));
            }
        }

        public SceneSetup[] RestoreSceneSetup()
        {
            SceneSetup[] ss = new SceneSetup[sceneSetup.Count];
            for (int i = 0; i < ss.Length; i++)
            {
                ss[i] = sceneSetup[i].GetSetup();
            }
            return ss;
        }

        [System.Serializable]
        public class SceneSetupWrapper
        {
            public bool isActive, isLoaded;
            public string path;

            public SceneSetupWrapper(SceneSetup s)
            {
                isActive = s.isActive;
                isLoaded = s.isLoaded;
                path = s.path;
            }

            public SceneSetup GetSetup()
            {
                SceneSetup s = new SceneSetup();
                s.path = path;
                s.isActive = isActive;
                s.isLoaded = isLoaded;

                return s;
            }
        }
    }
}