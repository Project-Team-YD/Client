using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Generics;
using HSMLibrary.UI;
using System;
using UnityEngine.UI;
using HSMLibrary.Extensions;
using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using HSMLibrary.Scene;
using TMPro;
using Packet;

public class DungeonSelectPopupController : UIBaseController, IPopup
{
    [SerializeField] private Button infinityBtn = null;
    [SerializeField] private Button timeAttackBtn = null;
    [SerializeField] private Button closeBtn = null;
    [SerializeField] private TextMeshProUGUI modeSelectText = null;

    private TextMeshProUGUI infinityText = null;
    private TextMeshProUGUI timeAttackText = null;
    private UIManager uiMgr = null;
    private PlayerManager playerManager = null;

    private const string MODE_SELECT_TEXT = "모드선택";
    private const string INFINITY_TEXT = "무한모드";
    private const string TIMEATTACK_TEXT = "타임어택";

    private int selectWeaponsId;

    protected override void Awake()
    {
        base.Awake();

        uiMgr = UIManager.getInstance;
        playerManager = PlayerManager.getInstance;

        timeAttackBtn.onClick.AddListener(OnClickTimeAttackButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
        infinityText = infinityBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        timeAttackText = timeAttackBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Initialize();
    }

    protected override void Initialize()
    {
        //base.Initialize();
        modeSelectText.text = MODE_SELECT_TEXT;
        infinityText.text = INFINITY_TEXT;
        timeAttackText.text = TIMEATTACK_TEXT;
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// 타입어택 모드 버튼 클릭시 호출..게임 진입
    /// </summary>
    private async void OnClickTimeAttackButton()
    {
        RequestJoinGame joinGame = new RequestJoinGame();
        joinGame.itemId = selectWeaponsId;
        var result = await GrpcManager.GetInstance.JoinGame(joinGame);

        if ((MessageCode)result.code == MessageCode.Success)
        {
            InGameManager.getInstance.CurrentStage = result.currentStage;
            playerManager.CurrentGold = result.gold;

            // playerManager.AddPlayerWeapon(selectWeaponsId);
            playerManager.UpdatePlayerWeapon(result.slot, result.effect);
            uiMgr.ClearAllCachedPanel();
            uiMgr.ClearAllPanelStack();
            SceneHelper.getInstance.ChangeScene(typeof(GameScene));
        }
        else
        {
            Debug.Log("ServerError");
        }
    }
    /// <summary>
    /// 무한모드 버튼 클릭시 호출..게임 진입
    /// </summary>
    private void OnClickInfinityButton()
    {

    }

    public void OnClickCloseButton()
    {
        uiMgr.Hide();
    }

    public void SetWeapon(int _id)
    {
        // 초기화 만들기
        selectWeaponsId = _id;
    }
}
