using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameShopItemController : MonoBehaviour
{
    [SerializeField] private Image thisImage = null;
    [SerializeField] private Button thisButton = null;

    private WeaponInfo info;
    private string explanation;
    private int price;
    private int idx;

    private Action<int> callBack = null;

    public WeaponInfo SetWeaponInfo { set { info = value; } }
    public string SetItemExplanation { get { return explanation; } set { explanation = value; } }
    public int SetItemPrice { get { return price; } set { price = value; } }
    public int SetIndex { get { return idx; } set { idx = value; } }

    private void Awake()
    {
        thisButton.onClick.AddListener(OnClickButton);
    }

    public void SetShopItemData(Action<int> _callBack)
    {
        callBack = _callBack;
    }

    private void OnClickButton()
    {
        callBack(idx);
    }

    public void UpdateItemData()
    {
        thisImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)info.weaponId}");
    }
}
