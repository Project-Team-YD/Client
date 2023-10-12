using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionManager : Singleton<TransitionManager>
{
    public enum TransitionType
    {
        Fade,
    }

    private Sequence sequenceFadeInOut;   

    private void InitFadeInOut(Image _image, float _time)
    {
        sequenceFadeInOut = DOTween.Sequence()
            .SetAutoKill(false) // DoTween Sequence는 기본 일회용..재사용시 필수.
            .Append(_image.DOFade(1.0f, _time)) // Alpha값 1.0까지 _time시간동안
            .Append(_image.DOFade(0.0f, _time)) // Alpha값 0까지 _time시간동안
            .SetLoops(-1, LoopType.Yoyo); // Yoyo타입으로 무한 루프.
    }

    /// <summary>
    /// 만들어둔 DoTween Sequence Play.
    /// </summary>
    /// <param name="_type">DoTween 효과 Type</param>
    /// <param name="_image">적용시킬 이미지</param>
    /// <param name="_time">작동시간</param>
    public void Play(TransitionType _type, Image _image, float _time)
    {
        switch (_type)
        {
            case TransitionType.Fade:
                InitFadeInOut(_image, _time);
                sequenceFadeInOut.Play();
                break;
            default:
                break;
        }
    }
}
