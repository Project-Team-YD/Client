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

    private const string DUNGEON_JOIN_TEXT = "던전입장";
    private const string EQUIP_TEXT = "착용중";
    private const string EQUIPMENT_TEXT = "착용하기";
    private const string JOIN_TEXT = "입장하기";

    private int weaponIndex;

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

        joinBtn.interactable = false;

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

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }

    public override void Hide()
    {
        base.Hide();
        // 초기화
        joinBtn.interactable = false;
    }

    /// <summary>
    /// 게임 입장 전 들고 갈 무기 선택시 슬릇에 선택 무기 이미지 설정 및 정보 가져오는 함수.
    /// </summary>
    /// <param name="_slotIndex">인벤토리 무기 슬릇 인덱스</param>
    public void SetSelectSlotWeaponImage(int _slotIndex)
    {
        weaponImage.enabled = true;
        enhanceText.enabled = true;
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)_slotIndex}");

        joinBtn.interactable = true;

        weaponIndex = _slotIndex;
    }
    /// <summary>
    /// 팝업 데이터 초기화.
    /// </summary>
    private void DataInitialization()
    {
        weaponImage.enabled = false;
        enhanceText.enabled = false;
        weaponImage.sprite = null;
    }
    /// <summary>
    /// 무기 선택 완료후 입장하기 버튼 클릭 호출 함수.
    /// </summary>
    private async void OnClickJoinButton()
    {
        var panel = await uiMgr.Show<DungeonSelectPopupController>("DungeonSelectPopup");
        /// 나중에 보유 무기에서 들고와야함(서버 반영 필요)
        panel.SetWeapon(weaponInfos[weaponIndex]);
    }

    private void OnClickCloseButton()
    {
        DataInitialization();
        uiMgr.Hide();
    }
}
