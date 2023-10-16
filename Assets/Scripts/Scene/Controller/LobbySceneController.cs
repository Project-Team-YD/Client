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
    [SerializeField] Button RankingBtn = null;
    [SerializeField] Button gameStartBtn = null;

    private int mapMaxCount = 0;
    private void Awake()
    {
        gameStartBtn.onClick.AddListener(OnClickDungeonButton);
        mapMaxCount = MapTable.getInstance.GetMapCount();
    }
    private void Start()
    {
        
    }

    public async void OnClickDungeonButton()
    {
        await UIManager.getInstance.Show<WeaponSelectPopupController>("WeaponSelectPopup");
    }

    public void OnClickEnhanceButton()
    {

    }

    public void OnClickShopButton()
    {

    }

    public void OnClickRankingButton()
    {

    }

    // TODO :: 착용 무기 정보 GameScene으로 넘겨줘야함.
    public void OnClickGameStartButton()
    {
        SceneHelper.getInstance.ChangeScene(typeof(GameScene));
    }    
}
