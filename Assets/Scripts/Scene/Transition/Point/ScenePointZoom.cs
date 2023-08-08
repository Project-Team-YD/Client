namespace HSMLibrary.Scene.Transition
{
    public class ScenePointZoom : ISceneTransition
    {
        //.. TODO :: 전용 세이더 따로 개발할것 [multi texture 활용]

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
        ~ScenePointZoom()
        {

        }
    }
}
