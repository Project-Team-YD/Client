using UnityEngine;
using System.Collections;

namespace HSMLibrary.Scene
{
    using Transition;

    public class SceneTransition
    {
        private SceneTransitionSequence _transition;

        private float _curFadeValue;

        private bool _isScenePreLoaded = false;

        public SceneTransition()
        {
            _curFadeValue = 1f;
        }

        public void SetFade(float _v)
        {
            _curFadeValue = _v;
        }

        public bool IsScenePreLoadCompleted { get { return _isScenePreLoaded; } }

        //.. Transition간 인풋 관련 처리를 할것인가?
        public IEnumerator ScenePreLoad(System.Type sceneType_)
        {
            //.. TODO :: Transition In 선처리
            _transition = new SceneTransitionSequence();
            _transition.PlayForward();

            _isScenePreLoaded = false;

            string sceneName = sceneType_.Name;
            AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            asyncOp.completed += OnAsyncCompleted;
            while (asyncOp.isDone == false)
            {
                //.. TODO :: Progress 처리
                Debug.Log(string.Format("Load Scene : {0} / Progress : {1}", sceneName, asyncOp.progress));
                yield return new WaitForEndOfFrame();
            }

            //.. TODO :: Transition Out 후처리
            _transition.SetTransitions(null);
            _transition.PlayReverse();
        }

        private void OnAsyncCompleted(AsyncOperation op_)
        {
            _isScenePreLoaded = true;
        }

        //.. INFO :: 메모리 해제시 GC로 보낼것들 명시
        ~SceneTransition()
        {

        }
    }
}
