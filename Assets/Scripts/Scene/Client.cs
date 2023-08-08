using UnityEngine;

namespace HSMLibrary.Scene
{
    using Sound;

    public class Client : MonoBehaviour
    {
        private static bool initialized = false;

        private static SceneManager sceneManager;
        public static SceneManager SceneManager
        {
            get { return sceneManager; }
        }

        private static MusicManager musicManager;
        public static MusicManager MusicManager
        {
            get { return musicManager; }
        }

        private static ResolutionManager resolutionManager;
        public static ResolutionManager ResolutionManager
        {
            get { return resolutionManager; }
        }

        private void Awake()
        {
            if (!initialized)
            {
                DontDestroyOnLoad(this.gameObject);
                initialized = true;

                Constructor();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Constructor()
        {
            sceneManager = GetComponentInChildren<SceneManager>();
            musicManager = GetComponentInChildren<MusicManager>();
            resolutionManager = GetComponentInChildren<ResolutionManager>();
        }
    }
}