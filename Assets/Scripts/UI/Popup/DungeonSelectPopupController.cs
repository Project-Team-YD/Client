using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Generics;
using HSMLibrary.UI;
using System;
using UnityEngine.UI;
using HSMLibrary.Extensions;
using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using HSMLibrary.Scene;
using TMPro;

public class DungeonSelectPopupController : UIBaseController , IPopup
{
    [SerializeField] Button infinityBtn = null;
    [SerializeField] Button timeAttackBtn = null;
    [SerializeField] Button closeBtn = null;
    [SerializeField] TextMeshProUGUI modeSelectText = null;

    private TextMeshProUGUI infinityText = null;
    private TextMeshProUGUI timeAttackText = null;
    private UIManager uiMgr = null;

    private const string MODE_SELECT_TEXT = "모드선택";
    private const string INFINITY_TEXT = "무한모드";
    private const string TIMEATTACK_TEXT = "타임어택";
    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        timeAttackBtn.onClick.AddListener(OnClickTimeAttackButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        infinityText = infinityBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        timeAttackText = timeAttackBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Initialize();
    }

    protected override void Initialize()
    {
        //base.Initialize();
        modeSelectText.text = MODE_SELECT_TEXT;
        infinityText.text = INFINITY_TEXT;
        timeAttackText.text = TIMEATTACK_TEXT;
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }

    public void OnClickTimeAttackButton()
    {
        SceneHelper.getInstance.ChangeScene(typeof(GameScene));
    }

    public void OnClickInfinityButton()
    {

    }

    public void OnClickCloseButton()
    {
        uiMgr.Hide();
    }
}
