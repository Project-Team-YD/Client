using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSMLibrary.Generics;
using HSMLibrary.UI;
using System;
using HSMLibrary.Extensions;
using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using TMPro;

public class BossAppearancePopupController : UIBaseController, IPopup
{
    [SerializeField] Image backgroundImage = null;
    [SerializeField] TextMeshProUGUI bossText = null;
    [SerializeField] TextMeshProUGUI warningText1 = null;
    [SerializeField] TextMeshProUGUI warningText2 = null;

    private const string BOSS_APPEARANCE_TEXT = "BOSS!!";

    private UIManager uiMgr = null;
    private TransitionManager transitionMgr = null;

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        transitionMgr = TransitionManager.getInstance;
        Initialize();
    }

    protected override void Initialize()
    {
        bossText.text = BOSS_APPEARANCE_TEXT;
        DimBackgroundImageFadeInOut().Forget();
    }
    /// <summary>
    /// DimImage DoTween FadeInOut && PositionMove Start 함수.
    /// </summary>
    /// <returns></returns>
    private async UniTask DimBackgroundImageFadeInOut()
    {
        transitionMgr.Play(TransitionManager.TransitionType.FadeInOut, 1.0f, Vector3.zero, backgroundImage.gameObject);
        transitionMgr.Play(TransitionManager.TransitionType.PositionMove, 4.0f, new Vector3(-1100, 250, 0), warningText1.gameObject);
        transitionMgr.Play(TransitionManager.TransitionType.PositionMove, 4.0f, new Vector3(1100, -250, 0), warningText2.gameObject);
        await UniTask.Delay(4000);
        transitionMgr.KillSequence(TransitionManager.TransitionType.FadeInOut);
        uiMgr.Hide();
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }
}
