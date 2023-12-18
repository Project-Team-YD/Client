using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlotView : MonoBehaviour
{
    [SerializeField] Image itemImage = null;
    [SerializeField] Button itemBtn = null;
    [SerializeField] GameObject soldOutDimObject = null;

    private int itemId;
    private Action<int> callback = null;
    public void InitItemSlot(ShopItem _item, Action<int> _callback)
    {
        itemId = _item.id;
        callback = _callback;
        itemImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)itemId}");        
        soldOutDimObject.SetActive(_item.isBuy);
        itemBtn.onClick.AddListener(OnClickItemSlot);
    }

    public void RefreshItemSlot(bool _isBuy)
    {
        soldOutDimObject.SetActive(_isBuy);
    }

    private void OnClickItemSlot()
    {
        callback?.Invoke(itemId);
    }
}
