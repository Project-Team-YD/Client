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
    private Dictionary<int, WeaponInfo> weaponInfoDic = null;
    private InventoryItem item = null;
    private TableManager tableMgr = null;
    private List<InventorySlotView> inventorys = new List<InventorySlotView>();

    private const string DUNGEON_JOIN_TEXT = "던전입장";
    private const string EQUIP_TEXT = "착용중";
    private const string EQUIPMENT_TEXT = "착용하기";
    private const string JOIN_TEXT = "입장하기";

    private int weaponId;

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        tableMgr = TableManager.getInstance;
        joinBtn.onClick.AddListener(OnClickJoinButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        equipmentText = equipBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        joinText = joinBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        weaponImage.enabled = false;
        enhanceText.enabled = false;

        joinBtn.interactable = false;

        Initialize();
    }

    protected override void Initialize()
    {
        var invenEnumerator = WeaponTable.getInstance.GetInventory().GetEnumerator();
        while(invenEnumerator.MoveNext())
        {
            item = invenEnumerator.Current.Value;
            GameObject newObject = GameObject.Instantiate(inventorySlot, slotRootTransform);
            var componenet = newObject.GetComponent<InventorySlotView>();
            componenet.InitWeaponInfo(item.id, item.enchant);
            componenet.SetWeaponImage();
            componenet.SetWeaponSelectController(this);
            inventorys.Add(componenet);
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
    /// 인벤토리 새로고침 함수.
    /// </summary>
    public void RefreshInventorys()
    {
        int inventoryCount = WeaponTable.getInstance.GetInventoryCount();
        for (int i = inventorys.Count; i < inventoryCount; i++)
        {
            GameObject newObject = GameObject.Instantiate(inventorySlot, slotRootTransform);
            var componenet = newObject.GetComponent<InventorySlotView>();
            componenet.SetWeaponSelectController(this);
            inventorys.Add(componenet);
        }
        int index = 0;
        var invenEnumerator = WeaponTable.getInstance.GetInventory().GetEnumerator();
        while (invenEnumerator.MoveNext())
        {
            item = invenEnumerator.Current.Value;
            inventorys[index].InitWeaponInfo(item.id, item.enchant);
            inventorys[index].SetWeaponImage();
            index++;
        }        
    }
    /// <summary>
    /// 게임 입장 전 들고 갈 무기 선택시 슬릇에 선택 무기 이미지 설정 및 정보 가져오는 함수.
    /// </summary>
    /// <param name="_key">인벤토리 무기 슬릇 id</param>
    public void SetSelectSlotWeaponImage(int _key)
    {
        int inventorysCount = inventorys.Count;
        for (int i = 0; i < inventorysCount; i++)
        {
            bool isOn = inventorys[i].id == _key;
            inventorys[i].OnOffChoiceEffectImage(isOn);
        }
        weaponImage.enabled = true;
        enhanceText.enabled = true;
        var data = WeaponTable.getInstance.GetInventoryData(_key);
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)_key}");

        joinBtn.interactable = true;

        enhanceText.text = $"+{data.enchant}";

        weaponId = _key;
    }
    /// <summary>
    /// 팝업 데이터 초기화.
    /// </summary>
    private void DataInitialization()
    {
        int inventorysCount = inventorys.Count;
        for (int i = 0; i < inventorysCount; i++)
        {            
            inventorys[i].OnOffChoiceEffectImage(false);
        }
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
        panel.SetWeapon(weaponId);
    }

    private void OnClickCloseButton()
    {
        DataInitialization();
        uiMgr.Hide();
    }
}
