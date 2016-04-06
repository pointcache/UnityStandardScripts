using UnityEngine;
using System.Collections;
using USS.Levels;
using USS;
using UnityEngine.SceneManagement;
namespace USS.Levels
{
    /// <summary>
    /// Special Component that handles level loading
    /// </summary>
    public class LevelActivator : MonoBehaviour
    {
        [SerializeField]
        Level level;
        [SerializeField]
        USSEditorPrefs prefs;
        [SerializeField]
        bool editorMode;

        /// <summary>
        /// For launching from EDITOR
        /// </summary>
        /// <param name="level"></param>
        /// <param name="_prefs">This is done to pass editor preferences from editor</param>
        /// <returns></returns>
        public static LevelActivator New(Level level, USSEditorPrefs _prefs)
        {
            GameObject go = new GameObject("Level Activator");
            LevelActivator activator = go.AddComponent<LevelActivator>();
            activator.level = level;
            activator.prefs = _prefs;
            //indicate that we launched from the editor
            activator.editorMode = true;
            return activator;
        }

        /// <summary>
        /// Use this to load level in game. All necessary clean up operation prior to loading you have to manage yourself.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static LevelActivator New(Level level)
        {
            GameObject go = new GameObject("Level Activator");
            LevelActivator activator = go.AddComponent<LevelActivator>();
            activator.level = level;
            return activator;
        }

        // Not awake because we have to be sure that the scene were in is RUNNING
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
                        prefs.EditorSceneLaunchMode = true;
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