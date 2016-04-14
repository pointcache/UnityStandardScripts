using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.IO;

namespace USS.Levels
{
    /// <summary>
    /// This is functionality siute for Levels 
    /// </summary>
    [InitializeOnLoad]
    public static class LevelsTools
    {

        const string LevelSeachQuery = "t:USS.Levels.Level"; //Used when collecting Level SctiptableObjects
        static List<Level> levels; //Used to rebuild editor build settings
        public static Level selectedLevel; //level that was selected in editor


        public static void InitializeLevels()
        {

            levels = new List<Level>();
            //get current build settings
            var currentBuildScenes = EditorBuildSettings.scenes.ToList();
            //find all levels
            string[] GUID = AssetDatabase.FindAssets(LevelSeachQuery);

            for (int i = 0; i < GUID.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(GUID[i]);
                Level LEV = (Level)AssetDatabase.LoadAssetAtPath(path, typeof(Level));
                levels.Add(LEV);
            }
            //for each level 
            for (int i = 0; i < levels.Count; i++)
            {
                Level L = levels[i];
                //get paths to all scenes
                List<string> currentPaths = currentBuildScenes.Select(x => x.path).ToList();
                //force level to Cache the scenes
                CacheLevel(L);
                //for each scene
                for (int t = 0; t < L.scenesInFolderPaths.Count; t++)
                {
                    //if scene is not registered in build settings, add it 
                    if (!currentPaths.Contains("Assets/" + L.scenesInFolderPaths[t]))
                    {
                        currentBuildScenes.Add(new EditorBuildSettingsScene("Assets/" + L.scenesInFolderPaths[t], true));
                    }
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //assign changed build settings back
            EditorBuildSettings.scenes = currentBuildScenes.ToArray();
        }

        /// <summary>
        /// This constructor works with [InitializeOnLoad] attribute, this ensures that our registered handlers will survive Assembly reload on Play
        /// </summary>
        static LevelsTools()
        {
            EditorApplication.playmodeStateChanged += RestoreScenes;
        }

        [MenuItem("USS/InitializeLevels")]
        static void Init()
        {
            InitializeLevels();
            CollectLevelsInDatabase();
        }

        /// <summary>
        /// Launches currently selected level from editor
        /// </summary>
        [MenuItem("Assets/LaunchLevel (runtime)")]
        [MenuItem("USS/LaunchLevel(select level asset)")]
        public static void LaunchSelectedLevel()
        {
            LaunchLevel(false);
        }
        
        public static void LaunchLevel(bool ignoreSelection)
        {
            InitializeLevels();
            CollectLevelsInDatabase();
            if (!ignoreSelection)
            {
                //get selection
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
            }
            EditorSceneManager.SaveOpenScenes();

            //we save our currently opened scenes setup in our Editor Prefs
            USSEditorPrefs.prefs.StoreSceneSetup(EditorSceneManager.GetSceneManagerSetup());

            int c = EditorSceneManager.loadedSceneCount;

            //clear the scenes we stored last time
            USSEditorPrefs.prefs.PreviousScenes.Clear();

            //now store currently opened ones
            for (int i = 0; i < c; i++)
            {
                Scene s = EditorSceneManager.GetSceneAt(0);
                USSEditorPrefs.prefs.PreviousScenes.Add(s.path);
            }

            if (USSEditorPrefs.prefs.GetLevelLaunchOverrideCallback() == null)
            {
                LaunchDefault(selectedLevel);
            }
            else
            {
                UnityAction<Level> cb = USSEditorPrefs.prefs.GetLevelLaunchOverrideCallback();
                cb(selectedLevel);
            }
                
            
        }

        static void _FinalizeLaunch(Level level)
        {

        }

        /// <summary>
        /// If the launch sequence is not injected this method is used.
        /// </summary>
        /// <param name="level"></param>
        static void LaunchDefault(Level level)
        {
            //Trick to clear all currently opened scenes is to just make new one
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single); 
            //Make level activator and provide selevted level
            LevelActivator.NewEditor(level, USSEditorPrefs.vars, null);
            //Start game
            EditorApplication.isPlaying = true;
        }

        /// <summary>
        /// Passively loads all scene from level into editor without changing EditorState
        /// </summary>
        [MenuItem("Assets/OpenLevel (editor)")]
        static void LoadLevel()
        {
            InitializeLevels();
            CollectLevelsInDatabase();
            //get selection
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

            //Trick to clear all currently opened scenes is to just make new one
            Scene n = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            

            LoadLevelEditor(selectedLevel);
            EditorSceneManager.CloseScene(n, true);
        }

        static void CollectLevelsInDatabase()
        {
            string[] GUID = AssetDatabase.FindAssets("t:USS.Levels.LevelsDatabase");
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

            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Restores scenes after we stopped playing to the state it was before
        /// </summary>
        static void RestoreScenes()
        {
            if (Application.isPlaying)
            {
                return;
            }
            //If we detect we launched level already
            if (USSEditorPrefs.vars.EditorSceneLaunchMode)
            {
                //clean all up
                Scene n = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                

                //reopen previously stored scenes
                for (int i = 0; i < USSEditorPrefs.prefs.PreviousScenes.Count; i++)
                {
                    EditorSceneManager.OpenScene(USSEditorPrefs.prefs.PreviousScenes[i], OpenSceneMode.Additive);
                }
                EditorSceneManager.CloseScene(n, true);
                //reset flag 
                USSEditorPrefs.vars.EditorSceneLaunchMode = false;
                //restore the setup to what it was before.
                EditorSceneManager.RestoreSceneManagerSetup(USSEditorPrefs.prefs.RestoreSceneSetup());
            }
        }

        public static void LoadLevelEditor(Level level)
        {
            int c = level.scenesInFolderPaths.Count;
            for (int i = 0; i < c; i++)
            {
                string scene = level.scenesInFolderPaths[i];
                EditorSceneManager.OpenScene(Application.dataPath + "/" + scene, OpenSceneMode.Additive);

                string[] arr_name = scene.Split('/');
                string name = arr_name[arr_name.Length - 1].Replace(".unity", "");

                if (level.ActiveScene.name == name)
                {
                    EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByPath("Assets/" + scene));
                }
            }
        }

        /// <summary>
        /// Collect all necessary data
        /// </summary>
        /// <returns></returns>
        public static void CacheLevel(Level level)
        {
            level.FolderPath = AssetDatabase.GetAssetPath(level.GetInstanceID());
            string[] arr = level.FolderPath.Split('/');
            arr[0] = "";
            arr[arr.Length - 1] = "";
            level.FolderPath = "";

            for (int i = 1; i < arr.Length; i++)
            {
                if (i == arr.Length - 1)
                    level.FolderPath += arr[i];
                else
                    level.FolderPath += arr[i] + "/";
            }

            level.scenesInFolderPaths = new List<string>();
            var scenes = Directory.GetFiles(Application.dataPath + "/" + level.FolderPath);
            foreach (var s in scenes)
            {
                if (s.Contains(".asset"))
                {
                    continue;
                }
                string d = s.Replace(Application.dataPath + "/", "");
                if (d.Contains("meta"))
                    continue;

                d = d.Replace('\\', '/');
                level.scenesInFolderPaths.Add(d);
            }
           
        }
    }
}