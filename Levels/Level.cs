using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine.SceneManagement;

namespace USTD.Levels
{
    public class Level : ScriptableObject
    {

        public LevelType levelType;
        public enum LevelType
        {
            game
        }

        public string Name;
        public string Description;
        public string FolderPath;
        public bool LevelLoaded;
        [HideInInspector]
        public Scene LevelActiveScene;
        public Object ActiveScene;

        public List<string> scenesInFolderPaths;

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
                if (i == c - 1)
                {
                    LevelLoaded = true;
                }
            }
        }
    }
}