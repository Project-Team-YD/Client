namespace HSMLibrary.Scene.Transition
{
    public class SceneCircleFadeInOut : SceneFadeInOut
    {
        public SceneCircleFadeInOut(float _duration, float _val)
        {
            duration = _duration;
            val = _val;
        }

        public override void PlayForward()
        {
            
        }

        public override void PlayReverse()
        {
            
        }

        public override bool IsPlaying()
        {
            return false;
        }

        //.. INFO :: 메모리 해제시 GC로 보낼것들 명시
        ~SceneCircleFadeInOut()
        {

        }
    }
}
