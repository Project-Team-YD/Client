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
    [SerializeField] private TextMeshProUGUI nowEnhance = null;
    [SerializeField] private TextMeshProUGUI nextEnhance = null;
    [SerializeField] private TextMeshProUGUI nowEnhanceInfo = null;
    [SerializeField] private TextMeshProUGUI nextEnhanceInfo = null;
    [SerializeField] private TextMeshProUGUI arrowText = null;

    private UIManager uiMgr = null;
    private InventoryItem item = null;
    private TableManager tableMgr = null;
    private int weaponIndex;
    
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
        weaponIndex = _key;
        var data = WeaponTable.getInstance.GetInventoryData(_key);
        int price = tableMgr.GetWeaponEnchantInfo(data.enchant + 1).price;
        weaponImage.enabled = true;
        enhanceText.enabled = true;
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)_key}");
        costText.text = string.Format("비용 : {0:0,0}", price);
        nowEnhance.text = $"+{data.enchant}";
        nextEnhance.text = $"+{data.enchant + 1}";
        arrowText.text = "->";
        float attackPower = WeaponTable.getInstance.GetWeaponInfo(_key).attackPower;
        float attackSpeed = WeaponTable.getInstance.GetWeaponInfo(_key).attackSpeed;
        float attackRange = WeaponTable.getInstance.GetWeaponInfo(_key).attackRange;
        if ((WeaponType)data.id == WeaponType.dagger || (WeaponType)data.id == WeaponType.sword)
        {
            nowEnhanceInfo.text = $"공격력 : {attackPower + ((data.enchant * WeaponTable.MELEE_WEAPON_ENHANCE_POWER) * attackPower)}\n" +
                $"공격범위 : {attackRange}\n" +
                $"공격속도 : {attackSpeed}";
            nextEnhanceInfo.text = $"공격력 : {attackPower + (((data.enchant + 1) * WeaponTable.MELEE_WEAPON_ENHANCE_POWER) * attackPower)}\n" +
                $"공격범위 : {attackRange}\n" +
                $"공격속도 : {attackSpeed}";
        }
        else
        {
            nowEnhanceInfo.text = $"공격력 : {attackPower + ((data.enchant * WeaponTable.RANGED_WEAPON_ENHANCE_POWER) * attackPower)}\n" +
                $"공격범위 : {attackRange}\n" +
                $"공격속도 : {attackSpeed + ((data.enchant * WeaponTable.RANGED_WEAPON_ENHANCE_SPEED) * attackSpeed)}";
            nextEnhanceInfo.text = $"공격력 : {attackPower + (((data.enchant + 1) * WeaponTable.RANGED_WEAPON_ENHANCE_POWER) * attackPower)}\n" +
                $"공격범위 : {attackRange}\n" +
                $"공격속도 : {attackSpeed + (((data.enchant + 1) * WeaponTable.RANGED_WEAPON_ENHANCE_SPEED) * attackSpeed)}";
        }
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
        nowEnhance.text = string.Empty;
        nextEnhance.text = string.Empty;
        nowEnhanceInfo.text = string.Empty;
        nextEnhanceInfo.text = string.Empty;
        arrowText.text = string.Empty;
        costText.text = string.Format("비용 : {0}", 0);
    }
    /// <summary>
    /// 강화 기능 함수.
    /// </summary>
    private async void Enhance()
    {
        RequestUpgradeItem upgradeItem = new RequestUpgradeItem();
        upgradeItem.id = weaponIndex;
        var result = await GrpcManager.GetInstance.UpgradeItem(upgradeItem);
        PlayerManager.getInstance.CurrentMoney = result.money;

        var inventory = await GrpcManager.GetInstance.LoadInventory();
        WeaponTable.getInstance.SetInventoryData(inventory.items);
    }
    /// <summary>
    /// 무기 강화 버튼 클릭시 호출되는 함수.
    /// </summary>
    private async void OnClickEnhanceButton()
    {
        var popup = await uiMgr.Show<MessageTwoButtonBoxPopupController>("MessageTwoButtonBoxPopup");
        var data = WeaponTable.getInstance.GetInventoryData(weaponIndex);
        popup.InitPopup($"이름 : {tableMgr.GetItemInfo(weaponIndex).itemName}\n" +
            $"+{data.enchant} -> +{data.enchant + 1}\n" +
            $"성공 확률 : {tableMgr.GetWeaponEnchantInfo(data.enchant + 1).probability / 10000}%\n" +
            $"강화 비용 : {tableMgr.GetWeaponEnchantInfo(data.enchant + 1).price}\n" +
            $"정말 강화하시겠습니까?", Enhance);
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