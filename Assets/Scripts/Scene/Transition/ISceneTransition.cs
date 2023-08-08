namespace HSMLibrary.Scene.Transition
{
    public interface ISceneTransition
    {
        /// <summary>
        /// 
        /// </summary>
        void PlayForward();
        /// <summary>
        /// 
        /// </summary>
        void PlayReverse();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsPlaying();
    }
}
