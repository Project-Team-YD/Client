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
        PositionMove,
        Rotate,
    }

    private Sequence sequenceFadeInOut;
    private Sequence sequencePositionMove;
    private Sequence sequenceRotate;

    private void InitFadeInOut(Image _image, float _time)
    {
        sequenceFadeInOut = DOTween.Sequence()
            .SetAutoKill(false) // DoTween Sequence는 기본 일회용..재사용시 필수.
            .Append(_image.DOFade(1.0f, _time)) // Alpha값 1.0까지 _time시간동안
            .Append(_image.DOFade(0.0f, _time)) // Alpha값 0까지 _time시간동안
            .SetLoops(-1, LoopType.Yoyo); // Yoyo타입으로 무한 루프.
    }

    private void InitPositionMove(GameObject _object, Vector3 _direction, float _time)
    {
        sequencePositionMove = DOTween.Sequence()
            .SetAutoKill(false)
            .Append(_object.transform.DOMove(_direction, _time))                        
            .SetLoops(-1, LoopType.Incremental);
    }

    private void InitRotate(GameObject _object, Vector3 _rotate, float _time)
    {
        sequenceRotate = DOTween.Sequence()
            .SetAutoKill(false)
            .Append(_object.transform.DORotate(_rotate, _time, RotateMode.FastBeyond360))
            .SetEase(Ease.Linear)
            .SetRelative(true)
            .SetLoops(-1, LoopType.Incremental);        
    }

    /// <summary>
    /// 만들어둔 DoTween Sequence Play.
    /// </summary>
    /// <param name="_type">효과 타입</param>
    /// <param name="_time">적용 시간</param>
    /// <param name="_vector"></param>
    /// <param name="_image"></param>
    /// <param name="_object"></param>
    public void Play(TransitionType _type, float _time, Vector3 _vector, Image _image = null, GameObject _object = null)
    {
        switch (_type)
        {
            case TransitionType.Fade:
                InitFadeInOut(_image, _time);
                sequenceFadeInOut.Play();
                break;
            case TransitionType.PositionMove:
                InitPositionMove(_object, _vector, _time);
                sequencePositionMove.Play();
                break;
            case TransitionType.Rotate:
                InitRotate(_object, _vector, _time);
                sequenceRotate.Play();
                break;
            default:
                break;
        }
    }

    public void KillSequence(TransitionType _type)
    {
        switch (_type)
        {
            case TransitionType.Fade:
                sequenceFadeInOut.Kill(true);
                break;
            case TransitionType.PositionMove:
                sequencePositionMove.Kill(true);
                break;
            case TransitionType.Rotate:
                sequenceRotate.Kill(true);                
                break;
            default:
                break;
        }
    }
}
