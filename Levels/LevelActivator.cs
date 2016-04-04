using UnityEngine;
using System.Collections;
using USTD.Levels;
using USTD;
using UnityEngine.SceneManagement;
namespace USTD.Levels
{
    public class LevelActivator : MonoBehaviour
    {
        [SerializeField]
        Level level;
        [SerializeField]
        USTDEditorPrefs prefs;
        [SerializeField]
        bool editorMode;

        /// <summary>
        /// For launching from EDITOR
        /// </summary>
        /// <param name="level"></param>
        /// <param name="_prefs"></param>
        /// <returns></returns>
        public static LevelActivator New(Level level, USTDEditorPrefs _prefs)
        {
            GameObject go = new GameObject("Level Activator");
            LevelActivator activator = go.AddComponent<LevelActivator>();
            activator.level = level;
            activator.prefs = _prefs;
            activator.editorMode = true;
            return activator;
        }

        public static LevelActivator New(Level level)
        {
            GameObject go = new GameObject("Level Activator");
            LevelActivator activator = go.AddComponent<LevelActivator>();
            activator.level = level;
            return activator;
        }

        // Use this for initialization
        void Start()
        {
            level.LoadLevel();
            if (editorMode)
                StartCoroutine(TrackLoadingEditor());
            else
                StartCoroutine(TrackLoading());
        }

        IEnumerator TrackLoadingEditor()
        {
            int i = 0;
            while (true)
            {
                //Wait 1 frame to activate the level ACTIVE scene
                if (i == 1)
                {
                    if (level.LevelLoaded)
                    {
                        SceneManager.SetActiveScene(level.LevelActiveScene);
                        prefs.SingleLevelLoadMode = true;
                        //self destruct
                        //Destroy(this.gameObject);
                        yield break;
                    }
                }
                i++;
                yield return null;
            }
        }

        IEnumerator TrackLoading()
        {
            int i = 0;
            while (true)
            {
                //Wait 1 frame to activate the level ACTIVE scene
                if (i == 1)
                {
                    if (level.LevelLoaded)
                    {
                        SceneManager.SetActiveScene(level.LevelActiveScene);
                        //self destruct
                        Destroy(this.gameObject);
                        yield break;
                    }
                }
                i++;
                yield return null;
            }
        }
    }
}