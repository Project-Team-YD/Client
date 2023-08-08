namespace HSMLibrary.Scene.Transition
{
    public abstract class SceneFadeInOut : ISceneTransition
    {
        //.. TODO :: 전용 쉐이더 개발할 것 [Alpha 컨트롤 관련]

        //.. FIXME? :: subclass로 보내버릴까 고민중
        protected float duration = 0f;
        protected float val = 0f;

        public abstract void PlayForward();
        public abstract void PlayReverse();
        public abstract bool IsPlaying();

        ~SceneFadeInOut()
        {

        }
    }
}
