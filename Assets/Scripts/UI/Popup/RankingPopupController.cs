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

public class RankingPopupController : UIBaseController, IPopup
{
    [SerializeField] Button closeBtn = null;
    [SerializeField] Button timeAttackTypeBtn = null;
    [SerializeField] TextMeshProUGUI rankingPopupText = null;
    [SerializeField] TextMeshProUGUI timeAttackText = null;

    private UIManager uiMgr = null;

    private const string RANKING_POPUP_TEXT = "랭킹목록";
    private const string TIMEATTACK_TEXT = "타임어택";

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        closeBtn.onClick.AddListener(OnClickCloseButton);
        timeAttackTypeBtn.onClick.AddListener(OnClicktimeAttackTypeButton);
        Initialize();
    }

    protected override void Initialize()
    {
        rankingPopupText.text = RANKING_POPUP_TEXT;
        timeAttackText.text = TIMEATTACK_TEXT;
    }

    public void OnClicktimeAttackTypeButton()
    {

    }

    public void OnClickCloseButton()
    {
        uiMgr.Hide();
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }
}
