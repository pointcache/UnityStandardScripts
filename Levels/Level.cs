using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

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
        /// Is cached automatically in CACHE()
        /// </summary>
        [HideInInspector]
        public string FolderPath;

        /// <summary>
        /// Indicates if level was fully loaded
        /// </summary>
        
        public bool LevelLoaded;
        /// <summary>
        /// Internal cached scene that will become active
        /// </summary>
        [HideInInspector]
        public Scene LevelActiveScene;

        /// <summary>
        /// Public field to reference your desired scene that will become the active scene of the level
        /// </summary>
        public UnityEngine.Object ActiveScene;

        /// <summary>
        /// List of paths to the scenes in our folder
        /// </summary>
        public List<string> scenesInFolderPaths;

        public  Action<Level> OnLevelLoaded;


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


    }
}