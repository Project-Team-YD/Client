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

public class WeaponEnhancePopupController : UIBaseController, IPopup
{
    [SerializeField] private GameObject inventorySlot = null;
    [SerializeField] private Transform slotRootTransform = null;
    [SerializeField] private Button enhanceBtn = null;
    [SerializeField] private Button closeBtn = null;
    [SerializeField] private Image weaponImage = null;
    [SerializeField] private TextMeshProUGUI enhancePopupText = null;
    [SerializeField] private TextMeshProUGUI enhanceText = null;
    [SerializeField] private TextMeshProUGUI enhanceButtonText = null;
    [SerializeField] private TextMeshProUGUI costText = null;
    [SerializeField] private Button EnhanceInfoBtn = null;
    [SerializeField] private GameObject EnhanceInfoPopup = null;

    private UIManager uiMgr = null;
    private InventoryItem item = null;
    private TableManager tableMgr = null;

    private const string ENHANCE_POPUP_TEXT = "장비강화";
    private const string ENHANCE_TEXT = "강화하기";

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        tableMgr = TableManager.getInstance;
        enhanceBtn.onClick.AddListener(OnClickEnhanceButton);
        EnhanceInfoBtn.onClick.AddListener(OnClickEnhanceInfoButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        weaponImage.enabled = false;
        enhanceText.enabled = false;
        EnhanceInfoPopup.SetActive(false);
        enhanceBtn.interactable = false;
        Initialize();
    }

    protected override void Initialize()
    {
        var invenEnumerator = WeaponTable.getInstance.GetInventory().GetEnumerator();
        while (invenEnumerator.MoveNext())
        {
            item = invenEnumerator.Current.Value;
            GameObject newObject = GameObject.Instantiate(inventorySlot, slotRootTransform);
            var componenet = newObject.GetComponent<InventorySlotView>();
            componenet.InitWeaponInfo(item.id, item.enchant);
            componenet.SetWeaponImage();
            componenet.SetWeaponEnhanceController(this);
        }
        
        enhancePopupText.text = ENHANCE_POPUP_TEXT;
        enhanceButtonText.text = ENHANCE_TEXT;
        costText.text = string.Format("비용 : {0}", 0);
    }
    /// <summary>
    /// 무기강화 팝업 인벤토리에서 무기 선택시 해당 무기의 이미지셋팅과 정보를 가져오는 함수.
    /// </summary>
    /// <param name="_key">인벤토리 무기 슬릇의 인덱스</param>
    public void SetSelectSlotWeaponImage(int _key)
    {
        var data = WeaponTable.getInstance.GetInventoryData(_key);
        int price = tableMgr.GetWeaponEnchantInfo(data.enchant + 1).price;
        weaponImage.enabled = true;
        enhanceText.enabled = true;
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)_key}");
        costText.text = string.Format("비용 : {0:0,0}", price);
        if (PlayerManager.getInstance.CurrentMoney >= price)
        {
            enhanceBtn.interactable = true;
        }
    }
    /// <summary>
    /// 팝업정보들 초기상태도 되돌림.
    /// </summary>
    private void DataInitialization()
    {
        weaponImage.enabled = false;
        enhanceText.enabled = false;
        weaponImage.sprite = null;
        costText.text = string.Format("비용 : {0}", 0);
    }
    /// <summary>
    /// 무기 강화 버튼 클릭시 호출되는 함수.
    /// </summary>
    private void OnClickEnhanceButton()
    {

    }

    private void OnClickEnhanceInfoButton()
    {
        EnhanceInfoPopup.SetActive(true);
    }

    private void OnClickCloseButton()
    {
        DataInitialization();
        uiMgr.Hide();
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }
}
