﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace USTD.Levels
{
    /// <summary>
    /// This is functionality siute for Levels 
    /// </summary>
    [InitializeOnLoad]
    public static class LevelsTools
    {

        const string LevelSeachQuery = "t:USTD.Levels.Level"; //Used when collecting Level SctiptableObjects
        static List<Level> levels; //Used to rebuild editor build settings
        static Level selectedLevel; //level that was selected in editor

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
                List<string> scenes = L.Cache();
                //for each scene
                for (int t = 0; t < L.scenesInFolderPaths.Count; t++)
                {
                    //if scene is not registered in build settings, add it 
                    if (!currentPaths.Contains("Assets/" + L.scenesInFolderPaths[i]))
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
            EditorApplication.playmodeStateChanged += InitializeLevels;
            EditorApplication.playmodeStateChanged += CollectLevelsInDatabase;
            EditorApplication.playmodeStateChanged += RestoreScenes;
        }


        /// <summary>
        /// Launches currently selected level from editor
        /// </summary>
        [MenuItem("Assets/LaunchLevel")]
        [MenuItem("USTD/LaunchLevel(select level asset)")]
        static void LaunchLevel()
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

            EditorSceneManager.SaveOpenScenes();

            //we save our currently opened scenes setup in our Editor Prefs
            USTDEditorPrefs.prefs.StoreSceneSetup(EditorSceneManager.GetSceneManagerSetup());

            int c = EditorSceneManager.loadedSceneCount;

            //clear the scenes we stored last time
            USTDEditorPrefs.prefs.PreviousScenes.Clear();

            //now store currently opened ones
            for (int i = 0; i < c; i++)
            {
                Scene s = EditorSceneManager.GetSceneAt(0);
                USTDEditorPrefs.prefs.PreviousScenes.Add(s.path);
            }

            //Trick to clear all currently opened scenes is to just make new one
            Scene n = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            //Make level activator and provide selevted level
            LevelActivator.New(selectedLevel, USTDEditorPrefs.prefs);
            //Start game
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
            if (USTDEditorPrefs.prefs.EditorSceneLaunchMode)
            {
                //clean all up
                Scene n = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.CloseScene(n, true);

                //reopen previously stored scenes
                for (int i = 0; i < USTDEditorPrefs.prefs.PreviousScenes.Count; i++)
                {
                    EditorSceneManager.OpenScene(USTDEditorPrefs.prefs.PreviousScenes[i], OpenSceneMode.Additive);
                }
                //reset flag 
                USTDEditorPrefs.prefs.EditorSceneLaunchMode = false;
                //restore the setup to what it was before.
                EditorSceneManager.RestoreSceneManagerSetup(USTDEditorPrefs.prefs.RestoreSceneSetup());
            }
        }
    }
}