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

    public void InitPopup(string _content, Action _callback = null, string _okText = "확인")
    {
        uiMgr = UIManager.getInstance;
        okBtnText.text = _okText;
        contentText.text = _content;
        callback = _callback;
        okButton.onClick.AddListener(OnClickOkButton);
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
