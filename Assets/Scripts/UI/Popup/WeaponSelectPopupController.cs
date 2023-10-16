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

public class WeaponSelectPopupController : UIBaseController, IPopup
{
    [SerializeField] Button equipBtn = null;
    [SerializeField] Button joinBtn = null;
    [SerializeField] Button closeBtn = null;
    [SerializeField] TextMeshProUGUI dungeonJoinText = null;
    [SerializeField] TextMeshProUGUI equipText = null;
    [SerializeField] TextMeshProUGUI enhanceText = null;

    private TextMeshProUGUI equipmentText = null;
    private TextMeshProUGUI joinText = null;
    private UIManager uiMgr = null;

    private const string DUNGEON_JOIN_TEXT = "던전입장";
    private const string EQUIP_TEXT = "착용중";
    private const string EQUIPMENT_TEXT = "착용하기";
    private const string JOIN_TEXT = "입장하기";

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        joinBtn.onClick.AddListener(OnClickJoinButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        equipmentText = equipBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        joinText = joinBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Initialize();
    }

    protected override void Initialize()
    {
        dungeonJoinText.text = DUNGEON_JOIN_TEXT;
        equipText.text = EQUIP_TEXT;
        equipmentText.text = EQUIPMENT_TEXT;
        joinText.text = JOIN_TEXT;
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }

    public async void OnClickJoinButton()
    {
        await uiMgr.Show<DungeonSelectPopupController>("DungeonSelectPopup");
    }

    public void OnClickCloseButton()
    {
        uiMgr.Hide();
    }
}
