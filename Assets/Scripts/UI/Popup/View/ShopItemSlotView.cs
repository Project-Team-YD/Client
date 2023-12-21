using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlotView : MonoBehaviour
{
    [SerializeField] Image itemImage = null;
    [SerializeField] Image choiceImage = null;
    [SerializeField] Button itemBtn = null;
    [SerializeField] GameObject soldOutDimObject = null;

    [NonSerialized] public int itemId;
    private Action<int> callback = null;
    public void InitItemSlot(ShopItem _item, Action<int> _callback)
    {
        itemId = _item.id;
        callback = _callback;
        choiceImage.enabled = false;
        itemImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)itemId}");
        soldOutDimObject.SetActive(_item.isBuy);
        itemBtn.onClick.AddListener(OnClickItemSlot);
    }
    /// <summary>
    /// 선택한 아이템 효과 이미지 OnOff함수.
    /// </summary>
    /// <param name="_isOn">On/Off여부</param>
    public void OnOffChoiceEffectImage(bool _isOn)
    {
        choiceImage.enabled = _isOn;
    }
    /// <summary>
    /// 상점 구매한 아이템 Dim효과를 켜주기 위해 구매 후 새로 고쳐주는 함수.
    /// </summary>
    /// <param name="_isBuy">아이템 샀는지 bool값</param>
    public void RefreshItemSlot(bool _isBuy)
    {
        soldOutDimObject.SetActive(_isBuy);
    }

    private void OnClickItemSlot()
    {
        callback?.Invoke(itemId);
    }
}
