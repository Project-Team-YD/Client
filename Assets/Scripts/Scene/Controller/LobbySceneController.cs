using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSMLibrary.Manager;

public class LobbySceneController : BaseSceneController
{
    [SerializeField] Button shoptBtn = null;
    [SerializeField] Button enhanceBtn = null;
    [SerializeField] Button rankingBtn = null;
    [SerializeField] Button gameStartBtn = null;

    private UIManager uiMgr = null;

    private void Awake()
    {
        uiMgr = UIManager.getInstance;
        shoptBtn.onClick.AddListener(OnClickShopButton);
        enhanceBtn.onClick.AddListener(OnClickEnhanceButton);
        rankingBtn.onClick.AddListener(OnClickRankingButton);
        gameStartBtn.onClick.AddListener(OnClickDungeonButton);
    }
    private void Start()
    {

    }
    /// <summary>
    /// 던전 입장 버튼 이벤트.
    /// </summary>
    public async void OnClickDungeonButton()
    {
        await uiMgr.Show<WeaponSelectPopupController>("WeaponSelectPopup");
    }
    /// <summary>
    /// 강화 버튼 이벤트.
    /// </summary>
    public async void OnClickEnhanceButton()
    {
        await uiMgr.Show<WeaponEnhancePopupController>("WeaponEnhancePopup");
    }
    /// <summary>
    /// 상점 버튼 이벤트.
    /// </summary>
    public async void OnClickShopButton()
    {
        await uiMgr.Show<ShopPopupController>("ShopPopup");
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
