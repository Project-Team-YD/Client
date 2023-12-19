using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameShopItemController : MonoBehaviour
{
    [SerializeField] private Image choiceEffect = null;
    [SerializeField] private Image thisImage = null;
    [SerializeField] private Button thisButton = null;
    [SerializeField] private TextMeshProUGUI enhance = null;

    private string explanation;
    private int price;
    private int idx;
    private int itemId;
    private bool isChoice;

    private TableManager tableManager = null;

    private Action<int> callBack = null;

    public string ItemExplanation { get { return explanation; } set { explanation = value; } }
    public int ItemPrice { get { return price; } set { price = value; } }
    public int SetIndex { set { idx = value; } }
    public string SetEnhance { set { enhance.text = value; } }
    public int ItemId { get { return itemId; } set { itemId = value; } }

    private void Awake()
    {
        tableManager = TableManager.getInstance;
        thisButton.onClick.AddListener(OnClickButton);
        isChoice = false;
    }

    public void SetShopItemData(Action<int> _callBack)
    {
        callBack = _callBack;

        UpdateItemImage();
    }

    private void OnClickButton()
    {
        callBack(idx);
        isChoice = true;
    }

    public void OnOffChoiceEffectImage(bool _isOn)
    {
        choiceEffect.enabled = _isOn;
    }
    /// <summary>
    /// image update
    /// </summary>
    public void UpdateItemImage()
    {
        var item = tableManager.GetItemInfo(itemId);

        if (item.itemType == 0)
        {
            thisImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)itemId}");
        }
        else
        {
            thisImage.sprite = Resources.Load<Sprite>($"Passive/{(PassiveType)itemId}");
        }
    }

    /// <summary>
    /// 강화 ui active
    /// </summary>
    /// <param name="_isActive"></param>
    public void ActiveEnhance(bool _isActive)
    {
        enhance.gameObject.SetActive(_isActive);
    }
}
