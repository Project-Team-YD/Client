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

public class RankingPopupController : UIBaseController, IPopup
{
    [SerializeField] private Button closeBtn = null;
    [SerializeField] private Button timeAttackTypeBtn = null;
    [SerializeField] private TextMeshProUGUI rankingPopupText = null;
    [SerializeField] private TextMeshProUGUI timeAttackText = null;

    [SerializeField] private Transform rankingTransform = null;
    [SerializeField] private RankingElementController rankingElementController = null;

    private UIManager uiMgr = null;

    private const string RANKING_POPUP_TEXT = "랭킹목록";
    private const string TIMEATTACK_TEXT = "타임어택";
    private const int MAX_RANKING_ELEMENT_COUNT = 10;

    private RankingElementController[] rankingElementControllers = null;

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        closeBtn.onClick.AddListener(OnClickCloseButton);
        timeAttackTypeBtn.onClick.AddListener(OnClicktimeAttackTypeButton);
        Initialize();
        SetRankElement();
    }

    protected override void Initialize()
    {
        rankingPopupText.text = RANKING_POPUP_TEXT;
        timeAttackText.text = TIMEATTACK_TEXT;
    }

    private void SetRankElement()
    {
        rankingElementControllers = new RankingElementController[MAX_RANKING_ELEMENT_COUNT];
        for (int i = 0; i < MAX_RANKING_ELEMENT_COUNT; i++)
        {
            var item = GameObject.Instantiate(rankingElementController, rankingTransform);
            item.gameObject.SetActive(false);
            rankingElementControllers[i] = item;
        }
    }

    /// <summary>
    /// 게임 타입 버튼 클릭 함수
    /// </summary>
    private void OnClicktimeAttackTypeButton()
    {

    }

    private void OnClickCloseButton()
    {
        uiMgr.Hide();
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }

    public void SetData()
    {
        // 서버에서 받아오기
        // 임시 데이터

        for (int i = 0; i < 5; i++)
        {
            rankingElementControllers[i].gameObject.SetActive(true);
            rankingElementControllers[i].SetData(i.ToString(), $"{i}번", $"{i},00,000");
        }
    }
}
