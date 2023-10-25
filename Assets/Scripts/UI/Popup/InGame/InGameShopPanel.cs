using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameShopPanel : MonoBehaviour
{
    [SerializeField] Image leftSlot = null;
    [SerializeField] Image rightSlot = null;

    public void UpdateLeftSlot(Sprite _sprite)
    {
        leftSlot.sprite = _sprite;
    }

    public void UpdateRightSlot(Sprite _sprite)
    {
        rightSlot.sprite = _sprite;
    }
}
