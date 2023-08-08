using System;
using System.Collections.Generic;

namespace HSMLibrary.Scene.Transition
{
    public class SceneTransitionSequence : ISceneTransition
    {
        //.. FIXME ? :: LinkedList가 최선일까?
        private LinkedList<ISceneTransition> transitionList = null;

        public SceneTransitionSequence()
        {
            transitionList = new LinkedList<ISceneTransition>();
            transitionList.Clear();
        }

        /// <summary>
        /// scene transition sequeuence allacation
        /// </summary>
        /// <param name="_transitions"></param>
        public SceneTransitionSequence(params ISceneTransition[] _transitions)
        {
            if(_transitions == null)
            {
                throw new ArgumentNullException();
            }

            transitionList = new LinkedList<ISceneTransition>();
            transitionList.Clear();

            SetTransitions(_transitions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_transitions"></param>
        public void SetTransitions(params ISceneTransition[] _transitions)
        {
            foreach(var transition in _transitions)
            {
                transitionList.AddLast(transition);
            }
        }

        public void PlayForward()
        {
            var transition = transitionList.First;
            while (transition.Value != null)
            {
                transition.Value.PlayForward();
                transition = transition.Next;
            }
        }

        public void PlayReverse()
        {
            var transition = transitionList.Last;
            while (transition.Value != null)
            {
                transition.Value.PlayReverse();
                transition = transition.Previous;
            }
        }

        public bool IsPlaying()
        {
            return false;
        }

        //.. INFO :: 메모리 해제시 GC로 보낼것들 명시
        ~SceneTransitionSequence()
        {
            transitionList.Clear();
            transitionList = null;
        }
    }
}
