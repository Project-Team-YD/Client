using System;
using System.Collections.Generic;

namespace HSMLibrary.Scene
{
    using Generics;

    public class SceneCallSet
    {
        public Type sceneType = null;
        public object postParams = null;
    }

    public class SceneCallStack : Singleton<SceneCallStack>
    {
        private const int SCENE_STACK_MAX_COUNT = 200;

        private LinkedList<SceneCallSet> sceneCallDeque = new LinkedList<SceneCallSet>();

        public void Push(SceneCallSet _sceneSet)
        {
            sceneCallDeque.AddLast(_sceneSet);

            if (sceneCallDeque.Count > SCENE_STACK_MAX_COUNT)
            {
                sceneCallDeque.RemoveFirst();
            }
        }

        public SceneCallSet Pop()
        {
            if (sceneCallDeque.Count > 0)
            {
                SceneCallSet sceneSet = sceneCallDeque.Last.Value;
                sceneCallDeque.RemoveLast();
                return sceneSet;
            }
            
            return null;
        }

        public void Clear()
        {
            sceneCallDeque.Clear();
        }
    }
}