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

public class InGameShopPanelController : UIBaseController
{
    [SerializeField] private InGameShopPanel inGameShopPanel = null;
    // 悼利 积己饶 包府 抗沥
    [SerializeField] private InGameShopItemController shopItemController = null;

    [SerializeField] Transform itemSlotTransform = null;

    [SerializeField] Button RefreshButton = null;
    [SerializeField] Button NextButton = null;

    private UIManager uiManager = null;

    protected override void Awake()
    {
        base.Awake();

        uiManager = UIManager.getInstance;
        RefreshButton.onClick.AddListener(OnClickRefreshButton);
        NextButton.onClick.AddListener(OnClickNextButton);
    }

    private void ResetInGameShop()
    {

    }

    private void OnClickRefreshButton()
    {

    }

    private void OnClickNextButton()
    {

    }
}
