using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSMLibrary.Manager;
using TMPro;

public class LobbySceneController : BaseSceneController
{
    [SerializeField] Button shoptBtn = null;
    [SerializeField] Button enhanceBtn = null;
    [SerializeField] Button rankingBtn = null;
    [SerializeField] Button gameStartBtn = null;
    [SerializeField] TextMeshProUGUI moneyText = null;
    [SerializeField] TextMeshProUGUI playerNickName = null;

    private UIManager uiMgr = null;

    private void Awake()
    {
        uiMgr = UIManager.getInstance;
        shoptBtn.onClick.AddListener(OnClickShopButton);
        enhanceBtn.onClick.AddListener(OnClickEnhanceButton);
        rankingBtn.onClick.AddListener(OnClickRankingButton);
        gameStartBtn.onClick.AddListener(OnClickDungeonButton);
        playerNickName.text = PlayerManager.getInstance.UserName;
        moneyText.text = $"{PlayerManager.getInstance.CurrentMoney}";
    }
    private void Start()
    {

    }
    /// <summary>
    /// 던전 입장 버튼 이벤트.
    /// </summary>
    public async void OnClickDungeonButton()
    {
        var popup = await uiMgr.Show<WeaponSelectPopupController>("WeaponSelectPopup");
        popup.RefreshInventorys();
    }
    /// <summary>
    /// 강화 버튼 이벤트.
    /// </summary>
    public async void OnClickEnhanceButton()
    {
        var popup = await uiMgr.Show<WeaponEnhancePopupController>("WeaponEnhancePopup");
        popup.RefreshInventorys(moneyText);
    }
    /// <summary>
    /// 상점 버튼 이벤트.
    /// </summary>
    public async void OnClickShopButton()
    {
        var popup = await uiMgr.Show<ShopPopupController>("ShopPopup");
        popup.RefreshShopItem(moneyText);
    }
    /// <summary>
    /// 랭킹 버튼 이벤트.
    /// </summary>
    public async void OnClickRankingButton()
    {
        var popup = await uiMgr.Show<RankingPopupController>("RankingPopup");
        popup.SetData();
    }
}
