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
    [SerializeField] private GameObject inventorySlot = null;
    [SerializeField] private Transform slotRootTransform = null;
    [SerializeField] private Button equipBtn = null;
    [SerializeField] private Button joinBtn = null;
    [SerializeField] private Button closeBtn = null;
    [SerializeField] private Image weaponImage = null;
    [SerializeField] private TextMeshProUGUI dungeonJoinText = null;
    [SerializeField] private TextMeshProUGUI equipText = null;
    [SerializeField] private TextMeshProUGUI enhanceText = null;

    private TextMeshProUGUI equipmentText = null;
    private TextMeshProUGUI joinText = null;
    private UIManager uiMgr = null;
    private WeaponInfo[] weaponInfos = null;

    private const string DUNGEON_JOIN_TEXT = "��������";
    private const string EQUIP_TEXT = "������";
    private const string EQUIPMENT_TEXT = "�����ϱ�";
    private const string JOIN_TEXT = "�����ϱ�";

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        joinBtn.onClick.AddListener(OnClickJoinButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        equipmentText = equipBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        joinText = joinBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
            componenet.SetWeaponSelectController(this);
        }
        dungeonJoinText.text = DUNGEON_JOIN_TEXT;
        equipText.text = EQUIP_TEXT;
        equipmentText.text = EQUIPMENT_TEXT;
        joinText.text = JOIN_TEXT;
    }

    public void DataInitialization()
    {
        weaponImage.enabled = false;
        enhanceText.enabled = false;
        weaponImage.sprite = null;
    }

    public void SetSelectSlotWeaponImage(int _slotIndex)
    {
        weaponImage.enabled = true;
        enhanceText.enabled = true;
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)_slotIndex}");
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
        DataInitialization();
        uiMgr.Hide();
    }
}