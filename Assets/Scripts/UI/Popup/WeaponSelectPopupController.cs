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

public class WeaponSelectPopupController : UIBaseController, IPopup
{
    [SerializeField] Button equipBtn = null;
    [SerializeField] Button joinBtn = null;
    [SerializeField] Button closeBtn = null;

    private UIManager uiMgr = null;

    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        joinBtn.onClick.AddListener(OnClickJoinButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }

    public async void OnClickJoinButton()
    {
        await uiMgr.Show<DungeonSelectPopupController>("DungeonSelectPopup");
    }

    public void OnClickCloseButton()
    {
        uiMgr.Hide();
    }
}
