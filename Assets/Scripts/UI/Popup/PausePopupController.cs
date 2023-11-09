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

public class PausePopupController : UIBaseController, IPopup
{
    [SerializeField] TextMeshProUGUI popupText = null;
    [SerializeField] TextMeshProUGUI buttonText = null;
    [SerializeField] Button playButton = null;

    private const string POPUP_NAME = "일시정지";
    private const string PLAY_TEXT = "Play";
    private UIManager uiMgr = null;
    private TimeManager timeMgr = null;
    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        timeMgr = TimeManager.getInstance;
        playButton.onClick.AddListener(OnClickPlayButton);
        Initialize();
    }

    protected override void Initialize()
    {
        popupText.text = POPUP_NAME;
        buttonText.text = PLAY_TEXT;
    }

    private void OnClickPlayButton()
    {
        uiMgr.Hide();
        timeMgr.PlayTime();
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }
}
