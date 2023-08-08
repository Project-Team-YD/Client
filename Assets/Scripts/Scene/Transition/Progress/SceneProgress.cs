namespace HSMLibrary.Scene.Transition
{
    public class SceneProgress : ISceneTransition
    {
        //.. TODO :: UGUI Image uv 연출 이용할것

        public virtual void PlayForward()
        {
            
        }

        public virtual void PlayReverse()
        {
            
        }

        public bool IsPlaying()
        {
            return false;
        }

        //.. INFO :: 메모리 해제시 GC로 보낼것들 명시
        ~SceneProgress()
        {

        }
    }
}
