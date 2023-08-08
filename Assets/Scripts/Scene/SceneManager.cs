using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnitySceneMgr = UnityEngine.SceneManagement.SceneManager;

namespace HSMLibrary.Scene
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField]
        private string startSceneName = null;

        private Scene currentScene = null;
        private string currentSceneName = null;

        public Scene Scene
        {
            get { return currentScene; }
        }

        public string StartSceneName { get => startSceneName; set => startSceneName = value; }

        private List<Type> sceneTypes = new List<Type>();

        private void Awake()
        {
            Type ti = typeof(Scene);
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (ti.IsAssignableFrom(type) && type.IsClass)
                    {
                        sceneTypes.Add(type);
                    }
                }
            }

            UnitySceneMgr.sceneLoaded += OnSceneLoaded;
        }

        private void Start()
        {
            foreach (Type sceneType in sceneTypes)
            {
                if (sceneType.Name == StartSceneName)
                {
                    SetScene(sceneType);
                }
            }
        }

        public void SetScene(Type _newSceneType)
        {
            if (currentScene != null)
            {
                currentScene.OnDeactivate();
            }

            if (!_newSceneType.IsSubclassOf(typeof(Scene)))
            {
                throw new Exception("Invalid Scene!");
            }

            Destroy(currentScene);

            currentScene = this.gameObject.AddComponent(_newSceneType) as Scene;

#if UNITY_EDITOR
            Debug.Log($"Scene : {_newSceneType}");
#endif

            if (currentScene != null)
            {
                SceneNameAttribute sceneEntryAttr = null;

                object[] attributes = currentScene.GetType().GetCustomAttributes(typeof(SceneNameAttribute), true);
                foreach (Attribute attr in attributes)
                {
                    sceneEntryAttr = attr as SceneNameAttribute;

                    break;
                }

                if (sceneEntryAttr == null)
                {
                    currentSceneName = currentScene.GetType().Name;
                }
                else
                {
                    currentSceneName = sceneEntryAttr.SceneName;
                }

                UnitySceneMgr.LoadScene(currentSceneName);
            }
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (scene.name == currentSceneName)
            {
                currentScene.OnActivate();
            }
        }

        private void Update()
        {
            if (currentScene != null)
            {
                currentScene.OnUpdate();
            }
        }
    }
}