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

    public async void OnClickDungeonButton()
    {
        await uiMgr.Show<WeaponSelectPopupController>("WeaponSelectPopup");
    }

    public async void OnClickEnhanceButton()
    {
        await uiMgr.Show<WeaponEnhancePopupController>("WeaponEnhancePopup");
    }

    public async void OnClickShopButton()
    {
        await uiMgr.Show<ShopPopupController>("ShopPopup");
    }

    public async void OnClickRankingButton()
    {
        await uiMgr.Show<RankingPopupController>("RankingPopup");
    }
}
