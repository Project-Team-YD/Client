using UnityEngine;
using UnityEngine.UI;
using HSMLibrary.UI;
using HSMLibrary.Scene;
using HSMLibrary.Manager;
using TMPro;

public class ResultPanelController : UIBaseController
{
    private const string RRUSLT_TITLE_TEXT = "게임 결과";
    private const string RECORD_TITLE_TEXT = "기록";
    private const string BEST_RECORD_TITLE_TEXT = "개인 최고기록";
    private const string COMPENSATION_TITLE_TEXT = "보상";
    private const string RANK_BUTTON_TITLE_TEXT = "랭킹 등록";
    private const string TITLE_BUTTON_TITLE_TEXT = "로비로 돌아가기";
    private const string GAMEOVER_TITLE_TEXT = "GAME OVER";
    private const string RANK_TITLE_TEXT = "GAME OVER";

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
    }

    /// <summary>
    /// 현재 기록 가져오기 
    /// 기존 최고기록 기억한거 들고오기
    /// 보상 정산 어떻게 할지
    /// 클리어 여부로 랭킹 등록 버튼 활성화
    /// </summary>
    public void SetData(bool _isClear = false)
    {
        gameOverGroup.SetActive(!_isClear);
        resultGroup.SetActive(_isClear);

        // 조건 서버에서 랭킹 갱신됐는지 받아오기
        rankGroup.SetActive(false);
    }

    /// <summary>
    /// 랭킹 갱신시 팝업 띄우고 팝업 화면 터치시 랭킹 갱신 팝업 꺼주기
    /// </summary>
    private void OnClickRankButton()
    {
        rankGroup.SetActive(false);
    }

    private void OnClickTitleButton()
    {
        playerManager.ClearPlayerWeapon();

        uiManager.Hide();

        // 타이틀로 돌아가기
        uiManager.ClearAllCachedPanel();
        uiManager.ClearAllPanelStack();

        SceneHelper.getInstance.ChangeScene(typeof(LobbyScene));
    }
}