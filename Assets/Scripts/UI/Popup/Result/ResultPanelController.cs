using UnityEngine;
using UnityEngine.UI;
using HSMLibrary.UI;
using HSMLibrary.Scene;
using HSMLibrary.Manager;
using TMPro;
using Packet;

public class ResultPanelController : UIBaseController
{
    private const string RRUSLT_TITLE_TEXT = "게임 결과";
    private const string RECORD_TITLE_TEXT = "기록";
    private const string BEST_RECORD_TITLE_TEXT = "개인 최고기록";
    private const string COMPENSATION_TITLE_TEXT = "보상";
    private const string RANK_BUTTON_TITLE_TEXT = "랭킹 등록";
    private const string TITLE_BUTTON_TITLE_TEXT = "로비로 돌아가기";
    private const string GAMEOVER_TITLE_TEXT = "GAME OVER";
    private const string RANK_TITLE_TEXT = "랭킹";
    private const string RANK_TITLE_BEST_RECORD_TEXT = "최고기록갱신";

    [SerializeField] private TextMeshProUGUI resultTitleText = null;
    [SerializeField] private TextMeshProUGUI recordTitleText = null;
    [SerializeField] private TextMeshProUGUI bestRecordTitleText = null;
    [SerializeField] private TextMeshProUGUI compensationTitleText = null;
    [SerializeField] private TextMeshProUGUI rankButtonText = null;
    [SerializeField] private TextMeshProUGUI titleButtonText = null;

    [SerializeField] private GameObject rankGroup = null;
    [SerializeField] private TextMeshProUGUI rankTitleText = null;
    [SerializeField] private TextMeshProUGUI rankText = null;
    [SerializeField] private Button rankGroupButton = null;

    [SerializeField] private TextMeshProUGUI recordText = null;
    [SerializeField] private TextMeshProUGUI bestRecordText = null;
    [SerializeField] private TextMeshProUGUI compensationText = null;

    [SerializeField] private Button rankButton = null;
    [SerializeField] private Button titleButton = null;

    [SerializeField] private GameObject resultGroup = null;
    [SerializeField] private GameObject gameOverGroup = null;
    [SerializeField] private TextMeshProUGUI gameOverText = null;
    [SerializeField] private TextMeshProUGUI gameOverRecordTitleText = null;
    [SerializeField] private TextMeshProUGUI gameOverRecordText = null;
    [SerializeField] private TextMeshProUGUI gameOverCompensationTitleText = null;
    [SerializeField] private TextMeshProUGUI gameOverCompensationText = null;

    private UIManager uiManager = null;
    private PlayerManager playerManager = null;

    protected override void Awake()
    {
        base.Awake();

        titleButton.onClick.AddListener(OnClickTitleButton);
        rankGroupButton.onClick.AddListener(OnClickRankButton);

        uiManager = UIManager.getInstance;
        playerManager = PlayerManager.getInstance;

        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();

        resultTitleText.text = RRUSLT_TITLE_TEXT;
        recordTitleText.text = RECORD_TITLE_TEXT;
        bestRecordTitleText.text = BEST_RECORD_TITLE_TEXT;
        compensationTitleText.text = COMPENSATION_TITLE_TEXT;
        titleButtonText.text = TITLE_BUTTON_TITLE_TEXT;

        rankTitleText.text = RANK_TITLE_TEXT;

        gameOverText.text = GAMEOVER_TITLE_TEXT;
        gameOverRecordTitleText.text = RECORD_TITLE_TEXT;
        gameOverCompensationTitleText.text = COMPENSATION_TITLE_TEXT;
    }

    /// <summary>
    /// 클리어 결과 서버 연동
    /// 데이터 갱신
    /// </summary>
    /// <param name="_isClear"></param> 클리어 여부
    public async void SetResultData(bool _isClear = false)
    {
        rankGroup.SetActive(false);

        var record = TimeManager.getInstance.GetTime;

        if (_isClear)
        {
            RequestUpdateTimeAttackRank requestUpdateTimeAttackRank = new RequestUpdateTimeAttackRank();
            requestUpdateTimeAttackRank.recordTime = record;
            var result = await GrpcManager.GetInstance.UpdateTimeAttackRank(requestUpdateTimeAttackRank);
            if ((MessageCode)result.code == MessageCode.Success)
            {
                var bestRecord = result.recordTime;
                var rank = result.rank;

                recordText.text = string.Format("{0}:{1:N3}", (int)record / 60, record % 60);
                bestRecordText.text = string.Format("{0}:{1:N3}", (int)bestRecord / 60, bestRecord % 60);
                compensationText.text = $"{result.rewardMoney}";

                if (record == bestRecord)
                {
                    rankTitleText.text = RANK_TITLE_BEST_RECORD_TEXT;
                    rankText.text = rank != 0 ? $"현재 {rank}위" : $"개인기록갱신";
                    rankGroup.SetActive(true);
                }
                else
                {
                    if (rank != 0)
                    {
                        rankTitleText.text = RANK_TITLE_TEXT;
                        rankText.text = $"현재 {rank}위";
                        rankGroup.SetActive(true);
                    }
                }

                playerManager.CurrentMoney = result.money;
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Server Error");
#endif
            }
        }
        else
        {
            var result = await GrpcManager.GetInstance.GameOver();
            if ((MessageCode)result.code == MessageCode.Success)
            {
                playerManager.CurrentMoney = result.money;
                gameOverCompensationText.text = $"{result.rewardMoney}";
                gameOverRecordText.text = string.Format("{0}:{1:N3}", (int)record / 60, record % 60);
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Server Error");
#endif
            }
        }

        gameOverGroup.SetActive(!_isClear);
        resultGroup.SetActive(_isClear);
    }

    /// <summary>
    /// rnak 갱신 팝업 close 버튼
    /// </summary>
    private void OnClickRankButton()
    {
        rankGroup.SetActive(false);
    }

    /// <summary>
    /// 로비신 이동
    /// </summary>
    private void OnClickTitleButton()
    {
        uiManager.Hide();

        // 타이틀로 돌아가기
        uiManager.ClearAllCachedPanel();
        uiManager.ClearAllPanelStack();

        SceneHelper.getInstance.ChangeScene(typeof(LobbyScene));
    }
}
