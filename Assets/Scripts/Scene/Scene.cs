using System;
using UnityEngine;

namespace HSMLibrary.Scene
{
    public abstract class Scene : MonoBehaviour
    {
        public abstract void OnActivate();
        public abstract void OnDeactivate();
        public abstract void OnUpdate();
    }

    public class SceneNameAttribute : Attribute
    {
        private string sceneName;

        public string SceneName { get { return sceneName; } }

        public SceneNameAttribute(string _sceneName)
        {
            sceneName = _sceneName;
        }
    }

}