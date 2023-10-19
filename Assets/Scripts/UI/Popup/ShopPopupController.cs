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

public class ShopPopupController : UIBaseController, IPopup
{
    [SerializeField] GameObject shopSlot = null;
    [SerializeField] Transform slotRootTransform = null;
    [SerializeField] Button buyBtn = null;
    [SerializeField] Button closeBtn = null;
    [SerializeField] TextMeshProUGUI goodsText = null;
    [SerializeField] TextMeshProUGUI buyText = null;

    private UIManager uiMgr = null;

    private const string BUY_TEXT = "±¸¸Å";

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        buyBtn.onClick.AddListener(OnClickBuyButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        Initialize();
    }
    protected override void Initialize()
    {
        buyText.text = BUY_TEXT;
    }

    public void OnClickBuyButton()
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
