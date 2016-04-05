using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace USTD
{
    public class USTDEditorPrefs : ScriptableObject
    {
        //Sorft of singleton
        private static USTDEditorPrefs _prefs;
        public static USTDEditorPrefs prefs
        {
            get
            {
                if (_prefs == null)
                {
                    string[] GUID = AssetDatabase.FindAssets("t:USTDEditorPrefs");
                    if (GUID.Length > 1)
                    {
                        Debug.LogError("<color=red>MAJOR VIOLATION - YOU CANT HAVE MORE THAN 1 OnEdgeEditorPrefs!</color>");
                        return null;
                    }
                    string path = AssetDatabase.GUIDToAssetPath(GUID[0]);
                    _prefs = (USTDEditorPrefs)AssetDatabase.LoadAssetAtPath(path, typeof(USTDEditorPrefs));
                }
                return _prefs;
            }
        }

        public bool EditorSceneLaunchMode;

        /// <summary>
        /// Scenes that were opened before we launched level from editor
        /// </summary>
        [SerializeField]
        public List<string> PreviousScenes; 

        [SerializeField]
        public List<SceneSetupWrapper> sceneSetup;

        public void StoreSceneSetup(SceneSetup[] setup)
        {
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