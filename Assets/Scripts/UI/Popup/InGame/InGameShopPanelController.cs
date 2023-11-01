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
    [SerializeField] private Transform itemLeftSlotTransform = null;
    [SerializeField] private Transform itemRightSlotTransform = null;

    [SerializeField] private Button refreshButton = null;
    [SerializeField] private Button buyButton = null;

    [SerializeField] private TextMeshProUGUI descriptionText = null;
    [SerializeField] private TextMeshProUGUI priceText = null;

    /// <summary>
    /// 구매완료 팝업
    /// 일단 스크립트 따로 안만들고 여기서 관리를 해줘야할지 생각 필요
    /// </summary>
    [SerializeField] private GameObject buyGroup = null;
    [SerializeField] private Button nextButton = null;

    private WeaponInfo[] weaponInfos = null;
    private UIManager uiManager = null;

    private List<InGameShopItemController> shopItemList = null;

    protected override void Awake()
    {
        base.Awake();

        uiManager = UIManager.getInstance;

        refreshButton?.onClick.AddListener(OnClickRefreshButton);
        buyButton.onClick.AddListener(OnClickBuyButton);
        nextButton.onClick.AddListener(OnClickNextButton);

        weaponInfos = WeaponTable.getInstance.GetWeaponInfos();

        Initialized();

        buyGroup.SetActive(false);
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

    /// <summary>
    /// 서버에서 데이터 받아와주고 1번째 아이템 자동 선택
    /// 서버에서 데이터 받는 메서드 따로 만들 필요 있음
    /// </summary>
    public void SetData()
    {
        // 임시
        if (shopItemList == null)
        {
            shopItemList = new List<InGameShopItemController>(MAX_SHOP_ITEM_COUNT);

            var count = weaponInfos.Length;

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

        OnClickItem(0);
    }

    private void UpdateMyWeaponData()
    {
        // 내가 장착중인 아이템 불러오기
    }

    /// <summary>
    /// 초기화
    /// </summary>
    private void ResetInGameShop()
    {
        buyGroup.SetActive(false);
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

    }

    #region 구매완료 팝업
    /// <summary>
    ///  NEXT 버튼
    /// </summary>
    private void OnClickNextButton()
    {

    }
    #endregion

    private void OnClickItem(int _idx)
    {
        var data = shopItemList[_idx];

        descriptionText.text = data.SetItemExplanation;
        priceText.text = $"{data.SetItemPrice}";

        // 선택된 아이템 기억하기
        // 선택된 아이템 표시


    }
}
