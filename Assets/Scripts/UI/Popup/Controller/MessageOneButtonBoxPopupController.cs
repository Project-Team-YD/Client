using HSMLibrary.Manager;
using HSMLibrary.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageOneButtonBoxPopupController : UIBaseController, IPopup
{
    [SerializeField] Button okButton = null;
    [SerializeField] TextMeshProUGUI okBtnText = null;
    [SerializeField] TextMeshProUGUI contentText = null;

    private Action callback = null;
    private UIManager uiMgr = null;

    protected override void Awake()
    {
        base.Awake();
        okButton.onClick.AddListener(OnClickOkButton);
    }
    /// <summary>
    /// Popup 셋팅 함수.
    /// </summary>
    /// <param name="_content">Text 내용</param>
    /// <param name="_callback">버튼 이벤트 함수.</param>
    /// <param name="_okText">버튼 문구.</param>
    public void InitPopup(string _content, Action _callback = null, string _okText = "확인")
    {
        uiMgr = UIManager.getInstance;
        okBtnText.text = _okText;
        contentText.text = _content;
        callback = _callback;        
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }

    private void OnClickOkButton()
    {
        callback?.Invoke();
        uiMgr.Hide();
    }
}
