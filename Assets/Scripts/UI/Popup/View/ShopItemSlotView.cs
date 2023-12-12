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
    public void InitItemSlot(int _key, Action<int> _callback)
    {
        itemId = _key;
        callback = _callback;
        itemImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)_key}");
        var invenEnumerator = WeaponTable.getInstance.GetInventory().GetEnumerator();
        while (invenEnumerator.MoveNext())
        {
            bool isbuy = invenEnumerator.Current.Value.id == _key;
            soldOutDimObject.SetActive(isbuy);
        }
        itemBtn.onClick.AddListener(OnClickItemSlot);
    }

    private void OnClickItemSlot()
    {
        callback?.Invoke(itemId);
    }
}
