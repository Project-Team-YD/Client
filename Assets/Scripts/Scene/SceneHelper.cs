using System;
using UnityEngine;

namespace HSMLibrary.Scene
{
    using Generics;

    public class SceneHelper : Singleton<SceneHelper>
    {
        private string queuedResponseJson = null;

        public SceneHelper()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        ~SceneHelper()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// 해당 씬으로 이동한다.
        /// </summary>
        /// <param name="_sceneType"></param>
        public void ChangeScene(Type _sceneType)
        {
            SceneCallStack.getInstance.Push(new SceneCallSet { sceneType = _sceneType });
            Client.SceneManager.SetScene(_sceneType);
        }

        public bool BackScene()
        {
            SceneCallStack.getInstance.Pop();
            SceneCallSet callSet = SceneCallStack.getInstance.Pop();
            if (callSet == null)
            {
                //ChangeScene(typeof(GamePlayScene));
                return true;
            }

            ChangeScene(callSet.sceneType);

            return true;
        }

        // 씬의 Start() 이전에 반드시 불린다.
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene _scene, UnityEngine.SceneManagement.LoadSceneMode _mode)
        {
            GameObject controllerObj = GameObject.Find("Controller");
            if (controllerObj != null)
            {
                BaseSceneController controller = controllerObj.GetComponent<BaseSceneController>();

                string musicName = controller.musicName;
                if (!string.IsNullOrEmpty(musicName))
                {
                    Client.MusicManager.Play(musicName);
                }

                if (queuedResponseJson != null)
                {
                    controller.OnResponsePacket(queuedResponseJson);
                    queuedResponseJson = null;
                }
            }
        }
    }
}