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
using HSMLibrary.Scene;
using Packet;

public class NickNamePanelController : UIBaseController
{
    [SerializeField] private Button checkButton = null;

    [SerializeField] private TextMeshProUGUI nickNameTitleText = null;
    [SerializeField] private TextMeshProUGUI nickNameInputPlaceholderText = null;
    [SerializeField] private TextMeshProUGUI nickNameInputWarningText = null;
    [SerializeField] private TextMeshProUGUI nickNameInputCheckText = null;

    [SerializeField] private TMP_InputField nickNameInputText = null;

    private UIManager uiManager = null;

    private const string NICKNAME_TITLE_TEXT = "닉네임 입력";
    private const string NICKNAME_INPUT_TEXT = "닉네임을 입력하세요";
    private const string NICKNAME_CHECK_TEXT = "확 인";
    private const string NICKNAME_CHECK_INPUT_TEXT = "최소 2자리 ~ 최대 8자리까지 입력가능합니다.";
    private const string NICKNAME_CHECK_OVERLAP_TEXT = "중복된 닉네임이 있습니다.";
    private const string NICKNAME_CHECK_WARNING_TEXT = "사용이 불가능한 닉네임이 있습니다.";

    private const int MIN_TEXT = 2;
    private const int MAX_TEXT = 8;

    protected override void Awake()
    {
        base.Awake();

        uiManager = UIManager.getInstance;

        checkButton.onClick.AddListener(OnClickCheckButton);

        nickNameInputText.onValueChanged.AddListener(InputFieldValueChanged);
    }

    public override void Show()
    {
        base.Show();

        Initialize();

        nickNameInputText.interactable = true;
        checkButton.interactable = false;
    }

    protected override void Initialize()
    {
        nickNameTitleText.text = NICKNAME_TITLE_TEXT;
        nickNameInputPlaceholderText.text = NICKNAME_INPUT_TEXT;
        nickNameInputWarningText.text = NICKNAME_CHECK_INPUT_TEXT;
        nickNameInputCheckText.text = NICKNAME_CHECK_TEXT;
    }

    /// <summary>
    /// 닉네임 체크 및 업데이트
    /// </summary>
    private async void OnClickCheckButton()
    {
        nickNameInputText.interactable = false;
        // 서버 연결 통신
        RequestUserName userName = new RequestUserName();
        userName.userName = nickNameInputText.text;
        var check = await GrpcManager.GetInstance.UserName(userName);

        if ((MessageCode)check.code == MessageCode.Success)
        {
            PlayerManager.getInstance.UserName = nickNameInputText.text;
            SceneHelper.getInstance.ChangeScene(typeof(LobbyScene));
        }
        else
        {
            nickNameInputWarningText.text = NICKNAME_CHECK_OVERLAP_TEXT;
            checkButton.interactable = false;
            nickNameInputText.interactable = true;
        }
    }

    /// <summary>
    /// 닉네임 입력 제한
    /// </summary>
    /// <param name="_text"></param> inputfield 
    private void InputFieldValueChanged(string _text)
    {
        if (_text.Length < MIN_TEXT)
        {
            checkButton.interactable = false;
            return;
        }

        else if (_text.Length > MAX_TEXT)
        {
            checkButton.interactable = false;
            return;
        }

        checkButton.interactable = true;
    }
}
