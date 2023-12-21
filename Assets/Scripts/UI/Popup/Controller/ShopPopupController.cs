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
    private int itemIndex;
    private List<ShopItemSlotView> items = new List<ShopItemSlotView>();
    private TextMeshProUGUI lobbyMoneyText = null;

    private const string BUY_TEXT = "구매";

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
            componenet.InitItemSlot(shopitem, SetDescriptionText);
            items.Add(componenet);
        }
    }
    /// <summary>
    /// 상점 아이템 설명 셋팅 함수.
    /// </summary>
    /// <param name="_id">아이템 id</param>
    private void SetDescriptionText(int _id)
    {
        int itemCount = tableMgr.GetShopDataCount();
        for (int i = 0; i < itemCount; i++)
        {
            bool isOn = items[i].itemId == _id;
            items[i].OnOffChoiceEffectImage(isOn);
        }
        itemIndex = _id;
        descriptionText.text = $"이름 : {tableMgr.GetItemInfo(_id).itemName}\n\n" +
            $"{tableMgr.descriptions[_id]}\n\n" +
            $"공격력 : {tableMgr.GetWeaponItem(_id).damage}\n" +
            $"공격범위 : {tableMgr.GetWeaponItem(_id).range}\n" +
            $"공격속도 : {tableMgr.GetWeaponItem(_id).speed}\n\n" +
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
    /// <summary>
    /// 상점 아이템 새로고침.
    /// </summary>
    public void RefreshShopItem(TextMeshProUGUI _money = null)
    {
        int itemCount = tableMgr.GetShopDataCount();
        for (int i = 0; i < itemCount; i++)
        {
            shopitem = tableMgr.GetShopItem(i);
            items[i].RefreshItemSlot(shopitem.isBuy);
            items[i].OnOffChoiceEffectImage(false);
        }
        if (_money != null)
        {
            lobbyMoneyText = _money;
            _money.text = $"{PlayerManager.getInstance.CurrentMoney}";
        }
        descriptionText.text = string.Empty;
        buyBtn.interactable = false;
    }
    /// <summary>
    /// 상점 구매 기능.
    /// </summary>
    private async void BuyItem()
    {
        RequestBuyItem buyItem = new RequestBuyItem();
        buyItem.id = itemIndex;
        var result = await GrpcManager.GetInstance.BuyItem(buyItem);
        PlayerManager.getInstance.CurrentMoney = result.money;

        var inventory = await GrpcManager.GetInstance.LoadInventory();
        WeaponTable.getInstance.SetInventoryData(inventory.items);
        
        tableMgr.SetShopItemRefresh(itemIndex);
        RefreshShopItem();
        goodsText.text = $"{PlayerManager.getInstance.CurrentMoney}";
        lobbyMoneyText.text = $"{PlayerManager.getInstance.CurrentMoney}";
    }
    /// <summary>
    /// 상점 구매하기 버튼 이벤트
    /// </summary>
    private async void OnClickBuyButton()
    {
        var popup = await uiMgr.Show<MessageTwoButtonBoxPopupController>("MessageTwoButtonBoxPopup");
        popup.InitPopup($"이름 : {tableMgr.GetItemInfo(itemIndex).itemName}\n" +
            $"가격 : {tableMgr.GetShopItem(itemIndex).price}원\n" +
            $"정말 구매하시겠습니까?", BuyItem);
    }

    private void OnClickCloseButton()
    {
        descriptionText.text = string.Empty;
        buyBtn.interactable = false;
        int itemCount = tableMgr.GetShopDataCount();
        for (int i = 0; i < itemCount; i++)
        {
            items[i].OnOffChoiceEffectImage(false);
        }
        uiMgr.Hide();
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }
}
