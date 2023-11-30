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

    /// <summary>
    /// 동적 생성후 관리 예정
    /// 슬롯이 없을경우에 한해서만 생성
    /// 슬롯이 존제 할경우 재사용 권장
    /// 버튼 연결해줄 콜백 함수 추가 필요
    /// </summary>
    [SerializeField] private InGameShopItemController shopItemController = null;

    [SerializeField] private TextMeshProUGUI possessionGoldText = null;

    /// <summary>
    /// 상점 아이템
    /// 왼쪽(위) 장착
    /// 오른쪽(아래) 장착
    /// </summary>
    [SerializeField] private Transform itemShopSlotTransform = null;
    [SerializeField] private Transform weaponItemSlotTransform = null;
    [SerializeField] private Transform itemSlotTransform = null;

    [SerializeField] private Button refreshButton = null;
    [SerializeField] private Button buyButton = null;

    [SerializeField] private TextMeshProUGUI descriptionText = null;
    [SerializeField] private TextMeshProUGUI priceText = null;

    /// <summary>
    /// 구매완료 팝업
    /// 일단 스크립트 따로 안만들고 여기서 관리를 해줘야할지 생각 필요
    /// </summary>
    [SerializeField] private GameObject buyObjectGroup = null;
    [SerializeField] private Button nextButton = null;

    private int MAX_PLAYER_WEAPON_COUNT;

    private WeaponInfo[] weaponInfos = null;
    private UIManager uiManager = null;

    private List<InGameShopItemController> shopItemList = null;
    private List<InGameShopItemController> weaponItemList = null;
    private List<InGameShopItemController> itemList = null;

    private PlayerManager playerManager = null;

    private Action callBack = null;

    private int selectItemIndex;

    protected override void Awake()
    {
        base.Awake();

        uiManager = UIManager.getInstance;
        playerManager = PlayerManager.getInstance;

        refreshButton?.onClick.AddListener(OnClickRefreshButton);
        buyButton.onClick.AddListener(OnClickBuyButton);
        nextButton.onClick.AddListener(OnClickNextButton);

        weaponInfos = WeaponTable.getInstance.GetWeaponInfos();

        Initialized();

        // 플레이어가 가질수있는 최대 무기 개수
        MAX_PLAYER_WEAPON_COUNT = playerManager.SetPlayerWeaponController.GetWeapons.Length;

        weaponItemList = new List<InGameShopItemController>(MAX_PLAYER_WEAPON_COUNT);

        for (int i = 0; i < MAX_PLAYER_WEAPON_COUNT; i++)
        {
            var item = GameObject.Instantiate(shopItemController, weaponItemSlotTransform);
            item.gameObject.SetActive(false);
            weaponItemList.Add(item);
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
    /// 서버에서 데이터 받아와주고 1번째 아이템 자동 선택
    /// 서버에서 데이터 받는 메서드 따로 만들 필요 있음
    /// </summary>
    public void SetData(Action _callback)
    {
        callBack = _callback;

        possessionGoldText.text = $"{playerManager.SetCurrentGold}";

        UpdateMyWeaponData();

        // MAX_PLAYER_WEAPON_COUNT와 비교해서 무기는 더이상 안나오게 바꿔야함 혹은 구매 못하게 해야함
        if (shopItemList == null)
        {
            shopItemList = new List<InGameShopItemController>(MAX_SHOP_ITEM_COUNT);

            for (int i = 0; i < MAX_SHOP_ITEM_COUNT; i++)
            {
                var item = GameObject.Instantiate(shopItemController, itemShopSlotTransform);
                item.SetWeaponInfo = weaponInfos[i];
                item.UpdateItemData();
                var idx = i;
                item.SetItemExplanation = $"{idx}번 아이템 설명";
                item.SetItemPrice = idx * 1000;
                item.SetIndex = idx;
                item.SetShopItemData(OnClickItem);
                shopItemList.Add(item);
            }
        }

        // 임시 
        var count = weaponInfos.Length;
        for (int i = 0; i < count; i++)
        {
            shopItemList[i].ActiveEnhance(false);
        }

        OnClickItem(0);
    }

    /// <summary>
    /// 충돌처리할때 어떤무기가 충돌됬는지
    /// </summary>

    // 내가 장착중인 아이템 불러오기
    private void UpdateMyWeaponData()
    {
        var items = playerManager.SetPlayerWeapons;
        int count = items.Count;

        for (int i = 0; i < count; i++)
        {
            var weapon = weaponItemList[i];

            if (weapon.gameObject.activeSelf == false)
            {
                weapon.gameObject.SetActive(true);
            }

            weapon.SetWeaponInfo = GetWeapon(items[i].weaponId);
            weapon.UpdateItemData();
            var idx = i;
            weapon.SetItemExplanation = $"{idx}번 아이템 설명";
            // weapon.SetItemPrice = idx * 1000;
            weapon.SetIndex = idx;
            // weapon.SetShopItemData(OnClickItem);

            weapon.SetEnhance = $"+{items[i].enhance}";
            weapon.ActiveEnhance(true);
        }
    }

    /// <summary>
    /// 초기화
    /// </summary>
    private void ResetInGameShop()
    {
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
    /// 구매 버튼
    /// </summary>
    private void OnClickBuyButton()
    {
        var addItem = shopItemList[selectItemIndex].SetWeaponInfo;
        playerManager.AddPlayerWeapon(addItem);

        // 구매완료 팝업 애니메이션화 후 
        buyObjectGroup.SetActive(true);

        playerManager.SetPlayerWeaponController.UpdateWeapon();
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

    private void OnClickItem(int _idx)
    {
        var data = shopItemList[_idx];

        descriptionText.text = data.SetItemExplanation;
        priceText.text = $"{data.SetItemPrice}";

        // 토글이 아닌 버튼으로 되어있어서 초기화시 선택된 index를 기억하여 초기버튼선택상태 초기화 해줘야함
        // 선택된 아이템 기억하기
        selectItemIndex = _idx;
        // 선택된 아이템 표시
    }

    private WeaponInfo GetWeapon(int _id)
    {
        var item = Array.Find(weaponInfos, x => x.weaponId == _id);
        return item;
    }
}
