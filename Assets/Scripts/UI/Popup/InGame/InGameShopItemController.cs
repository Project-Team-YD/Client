using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameShopItemController : MonoBehaviour
{
    [SerializeField] private Image thisImage = null;
    [SerializeField] private Button thisButton = null;
    [SerializeField] private TextMeshProUGUI enhance = null;

    private string explanation;
    private int price;
    private int idx;
    private int itemId;

    private Action<int> callBack = null;

    public string ItemExplanation { get { return explanation; } set { explanation = value; } }
    public int ItemPrice { get { return price; } set { price = value; } }
    public int SetIndex { get { return idx; } set { idx = value; } }
    public string SetEnhance { set { enhance.text = value; } }
    public int ItemId { get { return itemId; } set { itemId = value; } }

    private void Awake()
    {
        thisButton.onClick.AddListener(OnClickButton);
    }

    public void SetShopItemData(Action<int> _callBack)
    {
        callBack = _callBack;
        UpdateWeaponItemImage();
    }

    public void SetWeaponItemData(Action<int> _callBack)
    {
        callBack = _callBack;
        UpdateWeaponItemImage();
    }

    public void SetPassvieItemData(Action<int> _callBack)
    {
        callBack = _callBack;
        UpdatePassiveItemImage();
    }

    private void OnClickButton()
    {
        callBack(idx);
    }

    public void UpdateWeaponItemImage()
    {
        thisImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)itemId}");
    }

    public void UpdatePassiveItemImage()
    {
        thisImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)itemId}");
    }

    public void ActiveEnhance(bool _isActive)
    {
        enhance.gameObject.SetActive(_isActive);
    }
}
