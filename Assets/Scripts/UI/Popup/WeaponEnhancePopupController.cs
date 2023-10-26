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

    private UIManager uiMgr = null;
    private int costValue = 100000000;
    private WeaponInfo[] weaponInfos = null;

    private const string ENHANCE_POPUP_TEXT = "장비강화";
    private const string ENHANCE_TEXT = "강화하기";
    
    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        enhanceBtn.onClick.AddListener(OnClickEnhanceButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        weaponInfos = WeaponTable.getInstance.GetWeaponInfos();
        weaponImage.enabled = false;
        enhanceText.enabled = false;
        Initialize();
    }

    protected override void Initialize()
    {
        int weaponCount = weaponInfos.Length;        
        for (int i = 0; i < weaponCount; i++)
        {
            GameObject newObject = GameObject.Instantiate(inventorySlot, slotRootTransform);
            var componenet = newObject.GetComponent<InventorySlotView>();            
            componenet.InitWeaponInfo(weaponInfos[i]);
            componenet.SetWeaponImage();
            componenet.SetWeaponEnhanceController(this);
        }
        enhancePopupText.text = ENHANCE_POPUP_TEXT;
        enhanceButtonText.text = ENHANCE_TEXT;
        costText.text = string.Format("비용 : {0:0,0}", costValue);
    }
    /// <summary>
    /// 무기강화 팝업 인벤토리에서 무기 선택시 해당 무기의 이미지셋팅과 정보를 가져오는 함수.
    /// </summary>
    /// <param name="_slotIndex">인벤토리 무기 슬릇의 인덱스</param>
    public void SetSelectSlotWeaponImage(int _slotIndex)
    {
        weaponImage.enabled = true;
        enhanceText.enabled = true;
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)_slotIndex}");
    }
    /// <summary>
    /// 팝업정보들 초기상태도 되돌림.
    /// </summary>
    private void DataInitialization()
    {
        weaponImage.enabled = false;
        enhanceText.enabled = false;
        weaponImage.sprite = null;
    }
    /// <summary>
    /// 무기 강화 버튼 클릭시 호출되는 함수.
    /// </summary>
    private void OnClickEnhanceButton()
    {

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
