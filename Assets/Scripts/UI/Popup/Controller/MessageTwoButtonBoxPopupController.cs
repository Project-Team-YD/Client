using HSMLibrary.Manager;
using HSMLibrary.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageTwoButtonBoxPopupController : UIBaseController, IPopup
{
    [SerializeField] Button okButton = null;
    [SerializeField] Button cancelButton = null;
    [SerializeField] TextMeshProUGUI okBtnText = null;
    [SerializeField] TextMeshProUGUI cancelBtnText = null;
    [SerializeField] TextMeshProUGUI contentText = null;

    private Action callback = null;
    private UIManager uiMgr = null;

    protected override void Awake()
    {
        base.Awake();
        okButton.onClick.AddListener(OnClickOkButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    public void InitPopup(string _content, Action _callback = null, string _okText = "확인", string _cancelText = "취소")
    {
        uiMgr = UIManager.getInstance;
        okBtnText.text = _okText;
        cancelBtnText.text = _cancelText;
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

    private void OnClickCancelButton()
    {
        uiMgr.Hide();
    }
}
