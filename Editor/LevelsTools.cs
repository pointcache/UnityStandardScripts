using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace USTD.Levels
{
    [InitializeOnLoad]
    public static class LevelsTools
    {

        const string LevelSeachQuery = "t:USTD.Levels.Level";
        static List<Level> levels;
        static Level selectedLevel;

        public static void InitializeLevels()
        {
            //get levels list
            levels = new List<Level>();
            var currentBuildScenes = EditorBuildSettings.scenes.ToList();

            string[] GUID = AssetDatabase.FindAssets(LevelSeachQuery);

            for (int i = 0; i < GUID.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(GUID[i]);
                Level LEV = (Level)AssetDatabase.LoadAssetAtPath(path, typeof(Level));
                levels.Add(LEV);
            }

            for (int i = 0; i < levels.Count; i++)
            {
                Level L = levels[i];
                List<string> currentPaths = currentBuildScenes.Select(x => x.path).ToList();
                List<string> scenes = L.Cache();

                for (int t = 0; t < L.scenesInFolderPaths.Count; t++)
                {

                    if (!currentPaths.Contains("Assets/" + L.scenesInFolderPaths[i]))
                    {
                        currentBuildScenes.Add(new EditorBuildSettingsScene("Assets/" + L.scenesInFolderPaths[t], true));
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorBuildSettings.scenes = currentBuildScenes.ToArray();
        }

        static LevelsTools()
        {
            EditorApplication.playmodeStateChanged += InitializeLevels;
            EditorApplication.playmodeStateChanged += CollectLevelsInDatabase;
            EditorApplication.playmodeStateChanged += RestoreScenes;
        }



        [MenuItem("Assets/LaunchLevel")]
        [MenuItem("USTD/LaunchLevel(select level asset)")]
        static void TestOpenedLevel()
        {

            var selected = Selection.activeObject;
            if (selected == null)
            {
                Debug.Log("Select level first.");
                return;

            }
            Type t = selected.GetType();

            if (t != typeof(Level))
            {
                Debug.LogError("Select Level!");
                return;
            }

            selectedLevel = (Level)selected;
            EditorSceneManager.SaveOpenScenes();
            USTDEditorPrefs.prefs.StoreSceneSetup(EditorSceneManager.GetSceneManagerSetup());
            int c = EditorSceneManager.loadedSceneCount;

            USTDEditorPrefs.prefs.scenes.Clear();

            for (int i = 0; i < c; i++)
            {
                Scene s = EditorSceneManager.GetSceneAt(0);
                USTDEditorPrefs.prefs.scenes.Add(s.path);
            }

            Scene n = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            LevelActivator.New(selectedLevel, USTDEditorPrefs.prefs);
           // USTDEditorPrefs.prefs.SingleLevelLoadMode = true;
            EditorApplication.isPlaying = true;
        }

        static void CollectLevelsInDatabase()
        {
                string[] GUID = AssetDatabase.FindAssets("t:LevelsDatabase");
                string path = AssetDatabase.GUIDToAssetPath(GUID[0]);
                LevelsDatabase db = (LevelsDatabase)AssetDatabase.LoadAssetAtPath(path, typeof(LevelsDatabase));

                GUID = AssetDatabase.FindAssets(LevelSeachQuery);
                db.Levels.Clear();
                for (int i = 0; i < GUID.Length; i++)
                {
                    path = AssetDatabase.GUIDToAssetPath(GUID[i]);
                    Level LEV = (Level)AssetDatabase.LoadAssetAtPath(path, typeof(Level));
                    db.Levels.Add(LEV);
                }
        }

        static void RestoreScenes()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (USTDEditorPrefs.prefs.SingleLevelLoadMode)
            {
                Scene n = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.CloseScene(n, true);

                for (int i = 0; i < USTDEditorPrefs.prefs.scenes.Count; i++)
                {
                    EditorSceneManager.OpenScene(USTDEditorPrefs.prefs.scenes[i], OpenSceneMode.Additive);
                }
                USTDEditorPrefs.prefs.SingleLevelLoadMode = false;
                EditorSceneManager.RestoreSceneManagerSetup(USTDEditorPrefs.prefs.RestoreSceneSetup());
            }
        }
        [MenuItem("USTD/Test")]
        public static void Test()
        {
            InitializeLevels();
            CollectLevelsInDatabase();
        }


    }

    
}