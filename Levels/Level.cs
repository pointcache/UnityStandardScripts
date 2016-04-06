using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace USS.Levels
{
    /// <summary>
    /// This class describes level object
    /// </summary>
    public class Level : ScriptableObject
    {
        //add your own customization, for example "utility level", "race track", "village/town"
        public LevelType levelType;
        public enum LevelType
        {
            game
        }

        public string Name; //optional
        public string Description; //optional
        /// <summary>
        /// Mandatory starts after Assets/
        /// </summary>
        public string FolderPath;

        /// <summary>
        /// Indicates if level was fully loaded
        /// </summary>
        [HideInInspector]
        public bool LevelLoaded;
        /// <summary>
        /// Internal cached scene that will become active
        /// </summary>
        [HideInInspector]
        public Scene LevelActiveScene;

        /// <summary>
        /// Public field to reference your desired scene that will become the active scene of the level
        /// </summary>
        public Object ActiveScene;

        /// <summary>
        /// List of paths to the scenes in our folder
        /// </summary>
        public List<string> scenesInFolderPaths;

        /// <summary>
        /// Collect all necessary data
        /// </summary>
        /// <returns></returns>
        public List<string> Cache()
        {
            scenesInFolderPaths = new List<string>();
            var scenes = Directory.GetFiles(Application.dataPath + "/" + FolderPath);
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
                scenesInFolderPaths.Add(d);
            }
            return scenesInFolderPaths;
        }

        /// <summary>
        /// This is what actually performs the level loading from within the game, it doesnt cover Activation of the active scene,
        /// because it should be done with coroutine and coroutines work only with MonoBehaviour that is why we need level Activator
        /// </summary>
        public void LoadLevel()
        {
            int c = scenesInFolderPaths.Count;
            for (int i = 0; i < c; i++)
            {
                string scene = scenesInFolderPaths[i];
                SceneManager.LoadScene(scene.Replace(".unity", ""), LoadSceneMode.Additive);

                string[] arr_name = scene.Split('/');
                string name = arr_name[arr_name.Length - 1].Replace(".unity", "");

                if (ActiveScene.name == name)
                {
                    LevelActiveScene = SceneManager.GetSceneByPath("Assets/" + scene);
                }
            }
            LevelLoaded = true;
        }

        public void LoadLevelEditor()
        {
            int c = scenesInFolderPaths.Count;
            for (int i = 0; i < c; i++)
            {
                string scene = scenesInFolderPaths[i];
                EditorSceneManager.OpenScene(Application.dataPath + "/" + scene, OpenSceneMode.Additive);

                string[] arr_name = scene.Split('/');
                string name = arr_name[arr_name.Length - 1].Replace(".unity", "");

                if (ActiveScene.name == name)
                {
                    EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByPath("Assets/" + scene));
                }
            }
        }
    }
}