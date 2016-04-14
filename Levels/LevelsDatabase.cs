using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace USS.Levels
{
    public class LevelsDatabase : ScriptableObject
    {

        [SerializeField]
        public List<Level> Levels = new List<Level>();
        public Dictionary<string, Level> levels = new Dictionary<string, Level>();
        public void LoadLevel(string name)
        {
            if (levels.ContainsKey(name))
            {
                LevelActivator.New(levels[name], null);
            }
        }

        public void LoadLevel(string name, Action<Level> callback)
        {
            if (levels.ContainsKey(name))
            {
                LevelActivator.New(levels[name], callback);
            }else
            {
                Debug.Log("Level " + name + " was not located by database.");
            }
        }

        public void InitializeDatabase()
        {
            foreach (var L in Levels)
            {
                if (levels.ContainsKey(L.name))
                {
                    Debug.LogError("Level with name: " + L.name + " already exists, duplicate level names are forbidden.");
                    return;
                }
                levels.Add(L.name, L);
            }
        }
    }
}