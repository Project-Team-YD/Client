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
            .SetAutoKill(false) // DoTween Sequence�� �⺻ ��ȸ��..����� �ʼ�.
            .Append(_image.DOFade(1.0f, _time)) // Alpha�� 1.0���� _time�ð�����
            .Append(_image.DOFade(0.0f, _time)) // Alpha�� 0���� _time�ð�����
            .SetLoops(-1, LoopType.Yoyo); // YoyoŸ������ ���� ����.
    }

    /// <summary>
    /// ������ DoTween Sequence Play.
    /// </summary>
    /// <param name="_type">DoTween ȿ�� Type</param>
    /// <param name="_image">�����ų �̹���</param>
    /// <param name="_time">�۵��ð�</param>
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
