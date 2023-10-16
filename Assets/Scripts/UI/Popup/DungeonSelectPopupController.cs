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

public class DungeonSelectPopupController : UIBaseController , IPopup
{
    [SerializeField] Button infinityBtn = null;
    [SerializeField] Button timeAttackBtn = null;
    [SerializeField] Button closeBtn = null;

    private UIManager uiMgr = null;
    protected override void Awake()
    {
        base.Awake();
        uiMgr = UIManager.getInstance;
        timeAttackBtn.onClick.AddListener(OnClickTimeAttackButton);
        closeBtn.onClick.AddListener(OnClickCloseButton);
    }

    public T Show<T>() where T : IPopup
    {
        throw new NotImplementedException();
    }

    public void OnClickTimeAttackButton()
    {
        SceneHelper.getInstance.ChangeScene(typeof(GameScene));
    }

    public void OnClickInfinityButton()
    {

    }

    public void OnClickCloseButton()
    {
        uiMgr.Hide();
    }
}
