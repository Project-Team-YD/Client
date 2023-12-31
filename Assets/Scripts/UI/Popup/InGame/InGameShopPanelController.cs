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

public class InGameShopPanelController : UIBaseController
{
    #region Const
    private const string goldStoreTextKey = "골드상점";
    private const string buyTextKey = "구매완료!!";
    private const string selectedItemTextKey = "선택 아이템";
    private const string purchaseTextKey = "구매하기";
    private const string mountingSlotTextKey = "장착슬롯";
    private const string checkTextKey = "확인";

    private const int MAX_SHOP_ITEM_COUNT = 4;
    #endregion

    #region Text
    [SerializeField] private TextMeshProUGUI goldStoreText = null;
    [SerializeField] private TextMeshProUGUI buyText = null;
    [SerializeField] private TextMeshProUGUI selectedItemText = null;
    [SerializeField] private TextMeshProUGUI purchaseText = null;
    [SerializeField] private TextMeshProUGUI mountingSlotText = null;
    [SerializeField] private TextMeshProUGUI checkText = null;
    #endregion

    [SerializeField] private InGameShopItemController shopItemController = null;

    [SerializeField] private TextMeshProUGUI possessionGoldText = null;

    [SerializeField] private Transform itemShopSlotTransform = null;
    [SerializeField] private Transform weaponItemSlotTransform = null;
    [SerializeField] private Transform itemSlotTransform = null;

    [SerializeField] private Button refreshButton = null;
    [SerializeField] private Button buyButton = null;

    [SerializeField] private TextMeshProUGUI descriptionText = null;
    [SerializeField] private TextMeshProUGUI priceText = null;

    [SerializeField] private GameObject buyObjectGroup = null;
    [SerializeField] private Button nextButton = null;

    private int MAX_PLAYER_WEAPON_COUNT;
    private const int MAX_PLAYER_PASSIVE_ITEM_COUNT = 4; 

    private WeaponInfo[] weaponInfos = null;
    private UIManager uiManager = null;
    private InGameManager inGameManager = null;
    private TableManager tableMgr = null;

    private List<InGameShopItemController> shopItemList = null;
    private List<InGameShopItemController> weaponItemList = null;
    private List<InGameShopItemController> passiveItemList = null;

    private PlayerManager playerManager = null;

    private Action callBack = null;

    private int selectItemIndex;

    protected override void Awake()
    {
        base.Awake();

        uiManager = UIManager.getInstance;
        playerManager = PlayerManager.getInstance;
        inGameManager = InGameManager.getInstance;
        tableMgr = TableManager.getInstance;

        refreshButton?.onClick.AddListener(OnClickRefreshButton);
        buyButton.onClick.AddListener(OnClickBuyButton);
        nextButton.onClick.AddListener(OnClickNextButton);

        weaponInfos = WeaponTable.getInstance.GetWeaponInfos();

        Initialized();

        // 플레이어가 가질수있는 최대 무기 개수
        MAX_PLAYER_WEAPON_COUNT = playerManager.PlayerWeaponController.GetWeapons.Length;

        weaponItemList = new List<InGameShopItemController>(MAX_PLAYER_WEAPON_COUNT);

        for (int i = 0; i < MAX_PLAYER_WEAPON_COUNT; i++)
        {
            var item = GameObject.Instantiate(shopItemController, weaponItemSlotTransform);
            item.gameObject.SetActive(false);
            weaponItemList.Add(item);
        }

        passiveItemList = new List<InGameShopItemController>(MAX_PLAYER_PASSIVE_ITEM_COUNT);

        for (int i = 0; i < MAX_PLAYER_PASSIVE_ITEM_COUNT; i++)
        {
            var item = GameObject.Instantiate(shopItemController, itemSlotTransform);
            item.gameObject.SetActive(false);
            passiveItemList.Add(item);
        }


        shopItemList = new List<InGameShopItemController>(MAX_SHOP_ITEM_COUNT);

        for (int i = 0; i < MAX_SHOP_ITEM_COUNT; i++)
        {
            var item = GameObject.Instantiate(shopItemController, itemShopSlotTransform);
            item.gameObject.SetActive(false);
            shopItemList.Add(item);
        }

        buyObjectGroup.SetActive(false);
    }

    private void Initialized()
    {
        goldStoreText.text = goldStoreTextKey;
        buyText.text = buyTextKey;
        selectedItemText.text = selectedItemTextKey;
        purchaseText.text = purchaseTextKey;
        mountingSlotText.text = mountingSlotTextKey;
        checkText.text = checkTextKey;
    }

    public override void Hide()
    {
        base.Hide();

        callBack?.Invoke();

        callBack = null;
    }
    /// <summary>
    /// 서버에서 구매할수있는 아이템 받아서 출력
    /// </summary>
    /// <param name="_callback"></param> next wave callback
    public async void SetData(Action _callback)
    {
        callBack = _callback;

        possessionGoldText.text = $"{playerManager.CurrentGold}";

        UpdateMyWeaponData();
        UpdateMyPassiveItemData();

        RequestLoadIngameShop loadIngameShop = new RequestLoadIngameShop();
        loadIngameShop.currentStage = inGameManager.CurrentStage;
        loadIngameShop.gold = (int)playerManager.CurrentGold;
        var result = await GrpcManager.GetInstance.LoadIngameShop(loadIngameShop);        
        if ((MessageCode)result.code == MessageCode.Success)
        {
            int count = result.items.Length;

            var items = result.items;
            for (int i = 0; i < count; i++)
            {
                var idx = i;
                shopItemList[i].ItemId = items[i].id;
                shopItemList[i].SetIndex = idx;
                shopItemList[i].ItemPrice = items[i].price;
                if (items[i].id <= 3)
                {
                    shopItemList[i].ItemExplanation = $"이름 : {tableMgr.GetItemInfo(items[i].id).itemName}\n\n" +
                                                      $"{tableMgr.descriptions[items[i].id]}\n\n" +
                                                      $"공격력 : {tableMgr.GetWeaponItem(items[i].id).damage}\n" +
                                                      $"공격범위 : {tableMgr.GetWeaponItem(items[i].id).range}\n" +
                                                      $"공격속도 : {tableMgr.GetWeaponItem(items[i].id).speed}\n\n";
                    // + "이미 장착중인 아이템을 구매시 해당 무기의 +1강화만큼 수치가 증가합니다.";
                }
                else
                {
                    shopItemList[i].ItemExplanation = $"{tableMgr.descriptions[items[i].id]}\n\n" +
                                                      $"패시브 효과 : {tableMgr.passiveDescriptions[(items[i].id - 4)]}";
                }
                shopItemList[i].SetShopItemData(OnClickItem);
                shopItemList[i].ActiveEnhance(false);
                shopItemList[i].gameObject.SetActive(true);
            }

            OnClickItem(0);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("ServerError");
#endif
        }
    }

    /// <summary>
    /// player item setting
    /// </summary>
    private void UpdateMyWeaponData()
    {
        var items = playerManager.PlayerWeapons;
        int count = items.Length;

        for (int i = 0; i < count; i++)
        {
            var weapon = weaponItemList[i];

            if (weapon.gameObject.activeSelf == false)
            {
                weapon.gameObject.SetActive(true);
            }

            weapon.ItemId = items[i].id;
            var idx = i;
            weapon.SetIndex = idx;
            weapon.SetShopItemData(OnClickWeaponData);
            float attackPower = WeaponTable.getInstance.GetWeaponInfo(items[i].id).attackPower;
            float attackSpeed = WeaponTable.getInstance.GetWeaponInfo(items[i].id).attackSpeed;
            float attackRange = WeaponTable.getInstance.GetWeaponInfo(items[i].id).attackRange;
            if ((WeaponType)items[i].id == WeaponType.dagger || (WeaponType)items[i].id == WeaponType.sword)
            {
                weapon.ItemExplanation = $"이름 : {tableMgr.GetItemInfo(items[i].id).itemName}\n\n" +
                                         $"{tableMgr.descriptions[items[i].id]}\n\n" +
                                         $"공격력 : {attackPower + ((items[i].enchant * WeaponTable.MELEE_WEAPON_ENHANCE_POWER) * attackPower)}\n" +
                                         $"공격범위 : {attackRange}\n" +
                                         $"공격속도 : {attackSpeed}";
            }
            else
            {
                weapon.ItemExplanation = $"이름 : {tableMgr.GetItemInfo(items[i].id).itemName}\n\n" +
                                         $"{tableMgr.descriptions[items[i].id]}\n\n" +
                                         $"공격력 : {attackPower + ((items[i].enchant * WeaponTable.RANGED_WEAPON_ENHANCE_POWER) * attackPower)}\n" +
                                         $"공격범위 : {attackRange}\n" +
                                         $"공격속도 : {attackSpeed + ((items[i].enchant * WeaponTable.RANGED_WEAPON_ENHANCE_SPEED) * attackSpeed)}";
            }
            weapon.SetEnhance = $"+{items[i].enchant}";
            weapon.ActiveEnhance(true);
        }
    }

    /// <summary>
    /// player passive item setting
    /// </summary>
    private void UpdateMyPassiveItemData()
    {
        var items = playerManager.PlayerPassiveItem;
        if (items != null)
        {
            int count = items.Length;

            for (int i = 0; i < count; i++)
            {
                var item = passiveItemList[i];

                if (item.gameObject.activeSelf == false)
                {
                    item.gameObject.SetActive(true);
                }

                item.ItemId = items[i].id;
                var idx = i;
                item.SetIndex = idx;
                item.ItemExplanation = $"{tableMgr.descriptions[items[i].id]}\n\n" +
                                       $"패시브 효과 : {tableMgr.passiveDescriptions[(items[i].id - 4)]}";
                //item.ItemExplanation = $"{items[i].id}번 아이템 설명";
                item.SetShopItemData(OnClickPassiveItemData);
                // 여긴 확인필요
                item.SetEnhance = $"+{items[i].count}";
                item.ActiveEnhance(true);
            }
        }
    }

    /// <summary>
    /// 초기화
    /// </summary>
    private void ResetInGameShop()
    {
        int weaponItemCount = weaponItemList.Count;
        for (int i = 0; i < weaponItemCount; i++)
        {
            weaponItemList[i].gameObject.SetActive(false);
            weaponItemList[i].ActiveEnhance(false);
        }

        int passiveItemCount = passiveItemList.Count;
        for (int i = 0; i < passiveItemCount; i++)
        {
            passiveItemList[i].gameObject.SetActive(false);
            passiveItemList[i].ActiveEnhance(false);
        }

        // 선택 index 변수
        selectItemIndex = -1;
        // 버튼 초기화
        buyObjectGroup.SetActive(false);
    }

    /// <summary>
    /// 새로고침 버튼 추가시 필요
    /// </summary>
    private void OnClickRefreshButton()
    {

    }

    /// <summary>
    /// 구매 버튼 서버 연동
    /// </summary>
    private async void OnClickBuyButton()
    {
        RequestBuyIngameItem buyIngameItem = new RequestBuyIngameItem();
        buyIngameItem.itemId = shopItemList[selectItemIndex].ItemId;
        buyIngameItem.currentStage = inGameManager.CurrentStage;
        var result = await GrpcManager.GetInstance.BuyIngameItem(buyIngameItem);
        if ((MessageCode)result.code == MessageCode.Success)
        {
            inGameManager.CurrentStage = result.currentStage;
            playerManager.CurrentGold = result.gold;

            playerManager.UpdatePlayerWeapon(result.slot, result.effect);

            buyObjectGroup.SetActive(true);

            playerManager.PlayerWeaponController.UpdateWeapon();

            playerManager.CurrentGold = result.gold;
            possessionGoldText.text = $"{playerManager.CurrentGold}";
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("ServerError");
#endif
        }
    }

    #region 구매완료 팝업
    /// <summary>
    ///  NEXT 버튼
    /// </summary>
    private void OnClickNextButton()
    {
        uiManager.Hide();

        ResetInGameShop();
    }
    #endregion

    /// <summary>
    /// 상점 아이템 선택 함수
    /// </summary>
    /// <param name="_idx"></param> select item index
    private void OnClickItem(int _idx)
    {
        int shopItemCount = shopItemList.Count;
        for (int i = 0; i < shopItemCount; i++)
        {
            shopItemList[i].OnOffChoiceEffectImage(false);
        }
        shopItemList[_idx].OnOffChoiceEffectImage(true);
        var data = shopItemList[_idx];

        descriptionText.text = data.ItemExplanation;
        priceText.text = $"{data.ItemPrice}";

        selectItemIndex = _idx;
    }

    #region item info 
    private WeaponInfo GetWeapon(int _id)
    {
        var item = Array.Find(weaponInfos, x => x.weaponId == _id);
        return item;
    }

    private void OnClickWeaponData(int _key)
    {

    }

    private void OnClickPassiveItemData(int _key)
    {

    }
    #endregion
}
