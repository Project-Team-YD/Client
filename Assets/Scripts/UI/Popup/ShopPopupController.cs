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
using Packet;

public class ShopPopupController : UIBaseController, IPopup
{
    [SerializeField] GameObject shopSlot = null;
    [SerializeField] Transform slotRootTransform = null;
    [SerializeField] Button buyBtn = null;
    [SerializeField] Button closeBtn = null;
    [SerializeField] TextMeshProUGUI goodsText = null;
    [SerializeField] TextMeshProUGUI buyText = null;
    [SerializeField] TextMeshProUGUI descriptionText = null;

    private UIManager uiMgr = null;
    private TableManager tableMgr = null;
    private ShopItem shopitem = null;

    private const string BUY_TEXT = "구매";
    private string[] descriptions = { "기본적인 단검입니다. 공격속도가 빠르지만 조금 약합니다.",
                                      "기본적인 장검입니다. 한방이 묵직하지만 공격속도는 조금 느립니다!!",
                                      "기본적인 총입니다. 최강의 한방, 공격 속도가 매우 느린편.",
                                      "기본적인 표창입니다. 공격속도와 데미지가 균형잡혀 있습니다."
                                    };

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        tableMgr = TableManager.getInstance;
        buyBtn.onClick.AddListener(OnClickBuyButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        buyBtn.interactable = false;
        Initialize();
    }
    protected override void Initialize()
    {
        buyText.text = BUY_TEXT;
        goodsText.text = $"{PlayerManager.getInstance.CurrentMoney}";
        descriptionText.text = string.Empty;
        int itemCount = tableMgr.GetShopDataCount();
        for (int i = 0; i < itemCount; i++)
        {
            shopitem = tableMgr.GetShopItem(i);
            GameObject newObject = GameObject.Instantiate(shopSlot, slotRootTransform);
            var componenet = newObject.GetComponent<ShopItemSlotView>();
            componenet.InitItemSlot(shopitem.id, SetDescriptionText);
        }
    }

    private void SetDescriptionText(int _id)
    {
        descriptionText.text = $"이름 : {tableMgr.GetItemInfo(_id).itemName}\n\n" +
            $"{descriptions[_id]}\n\n" +
            $"공격력 : {tableMgr.GetWeaponItem(_id).damage}\n" +
            $"공격 범위 : {tableMgr.GetWeaponItem(_id).range}\n" +
            $"공격 속도 : {tableMgr.GetWeaponItem(_id).speed}\n\n" +
            $"가격 : {tableMgr.GetShopItem(_id).price}원";

        if(PlayerManager.getInstance.CurrentMoney < tableMgr.GetShopItem(_id).price)
        {
            buyBtn.interactable = false;
        }
        else
        {
            buyBtn.interactable = true;
        }
    }

    private void OnClickBuyButton()
    {

    }

    private void OnClickCloseButton()
    {
        descriptionText.text = string.Empty;
        uiMgr.Hide();
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }
}
