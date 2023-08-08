using System;
using System.Collections.Generic;

namespace HSMLibrary.Scene.Transition
{
    public class SceneTransitionMixer : ISceneTransition
    {
        private List<ISceneTransition> transitionList = null;
        
        public SceneTransitionMixer(params ISceneTransition[] _transitions)
        {
            if (_transitions == null)
            {
                throw new ArgumentNullException();
            }

            transitionList = new List<ISceneTransition>();
            transitionList.Clear();

            foreach (var transition in _transitions)
            {
                transitionList.Add(transition);
            }
        }

        public void PlayForward()
        {
            foreach (var transition in transitionList)
            {
                transition.PlayForward();
            }
        }

        public void PlayReverse()
        {
            foreach (var transition in transitionList)
            {
                transition.PlayReverse();
            }
        }

        public bool IsPlaying()
        {
            return false;
        }

        //.. INFO :: 메모리 해제시 GC로 보낼것들 명시
        ~SceneTransitionMixer()
        {
            transitionList.Clear();
            transitionList = null;
        }
    }
}
