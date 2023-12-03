using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TransitionManager : Singleton<TransitionManager>
{
    public enum TransitionType
    {
        FadeInOut,
        PositionMove,
        Invisible,
        Rotate,
        ChangeColor,
    }

    private Sequence sequenceFadeInOut;
    private Sequence sequencePositionMove;
    private Sequence sequenceInvisible;
    private Sequence sequenceRotate;
    private Sequence sequenceChangeColor;

    private Color defaultColor = Color.white;

    /// <summary>
    /// FadeInOut 초기화 함수. (깜빡거리는 효과 등..)
    /// </summary>
    /// <param name="_image">적용시킬 이미지</param>
    /// <param name="_time">적용시간</param>
    private void InitFadeInOut(Image _image, float _time)
    {
        sequenceFadeInOut = DOTween.Sequence()
            .SetAutoKill(false) // DoTween Sequence는 기본 일회용..재사용시 필수.
            .Append(_image.DOFade(1.0f, _time)) // Alpha값 1.0까지 _time시간동안
            .Append(_image.DOFade(0.0f, _time)) // Alpha값 0까지 _time시간동안
            .SetLoops(-1, LoopType.Yoyo); // Yoyo타입으로 무한 루프.
    }
    /// <summary>
    /// PositionMove 초기화 함수. (위치 이동 기능)
    /// </summary>
    /// <param name="_object">적용시킬 오브젝트</param>
    /// <param name="_direction">적용시킬 방향</param>
    /// <param name="_time">적용시간</param>
    private void InitPositionMove(GameObject _object, Vector3 _direction, float _time)
    {
        sequencePositionMove = DOTween.Sequence()
            //.SetAutoKill(false)
            .Append(_object.transform.DOLocalMove(_direction, _time))                        
            .SetLoops(1);
    }
    /// <summary>
    /// Invisible 초기화 함수. (투명화 기능)
    /// </summary>
    /// <param name="_object">적용시킬 텍스트</param>
    /// <param name="_time">적용시간</param>
    private void InitInvisible(TextMeshProUGUI _object, float _time)
    {
        sequenceInvisible = DOTween.Sequence()
            //.SetAutoKill(false) //Text(EX..Damage)의 경우 여러개가 동시에 시행될수도 있기 때문에 시행 후 AutoKill..재사용의 의미가 없음.
            .Append(_object.DOFade(0.0f, _time))
            .SetLoops(1);
    }
    /// <summary>
    /// Rotate 초기화 함수(Loops true) (회전 기능)
    /// </summary>
    /// <param name="_object">적용시킬 오브젝트</param>
    /// <param name="_rotate">적용시킬 Vector3 값</param>
    /// <param name="_time">적용시간</param>
    private void InitRotateInfinity(GameObject _object, Vector3 _rotate, float _time)
    {
        sequenceRotate = DOTween.Sequence()
            .SetAutoKill(false)
            .Append(_object.transform.DORotate(_rotate, _time, RotateMode.FastBeyond360))
            .SetEase(Ease.Linear)
            .SetRelative(true)
            .SetLoops(-1, LoopType.Incremental);        
    }
    private void InitChangeColor(SpriteRenderer _object, float _time, Color _color)
    {
        sequenceChangeColor = DOTween.Sequence()
            .Append(_object.DOColor(_color, _time))
            .Append(_object.DOColor(Color.white, _time))
            .SetLoops(1);
    }

    /// <summary>
    /// 만들어둔 DoTween Sequence Play.
    /// </summary>
    /// <param name="_type">효과 타입</param>
    /// <param name="_time">적용 시간</param>
    /// <param name="_vector">적용시킬 Vector값</param>
    /// <param name="_image">적용시킬 이미지</param>
    /// <param name="_object">적용시킬 오브젝트</param>
    public void Play(TransitionType _type, float _time, Vector3 _vector, GameObject _object = null, Color _color = new Color())
    {
        switch (_type)
        {
            case TransitionType.FadeInOut:
                if (_object.TryGetComponent<Image>(out var _image))
                {
                    InitFadeInOut(_image, _time);
                    sequenceFadeInOut.Play();
                }
                else
                    Debug.Log("이미지에 적용하세요.");
                break;
            case TransitionType.PositionMove:
                InitPositionMove(_object, _vector, _time);
                sequencePositionMove.Play();
                break;
            case TransitionType.Invisible:
                if (_object.TryGetComponent<TextMeshProUGUI>(out var _text))
                {
                    InitInvisible(_text, _time);
                    sequenceInvisible.Play();
                }
                else
                    Debug.Log("텍스트에 적용해주세요.");
                break;
            case TransitionType.Rotate:
                InitRotateInfinity(_object, _vector, _time);
                sequenceRotate.Play();
                break;
            case TransitionType.ChangeColor:
                if(_object.TryGetComponent<SpriteRenderer>(out var _sprite))
                {
                    InitChangeColor(_sprite, _time, _color);
                    sequenceChangeColor.Play();
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 현재 적용된 DoTweenSequence Kiil함수.(Scene 이동 or Destroy 상황시 kill 목적.)
    /// </summary>
    /// <param name="_type">Kill 시킬 DoTween타입</param>
    public void KillSequence(TransitionType _type)
    {
        switch (_type)
        {
            case TransitionType.FadeInOut:
                if(sequenceFadeInOut.IsPlaying())
                    sequenceFadeInOut.Kill(true);                
                break;
            case TransitionType.PositionMove:
                if(sequencePositionMove.IsPlaying())
                    sequencePositionMove.Kill(true);
                break;
            case TransitionType.Invisible:
                if(sequenceInvisible.IsPlaying())
                    sequenceInvisible.Kill(true);                
                break;
            case TransitionType.Rotate:
                sequenceRotate.Kill(false);
                break;
            case TransitionType.ChangeColor:
                if (sequenceChangeColor.IsPlaying())
                    sequenceChangeColor.Kill(true);
                break;
            default:
                break;
        }
    }
}
