using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour
{
    [SerializeField] private Image weaponImage = null;
    [SerializeField] private Button slotBtn = null;

    private WeaponEnhancePopupController weaponEnhancePopupController = null;
    private WeaponSelectPopupController weaponSelectPopupController = null;
    private WeaponInfo info;    

    public void SetWeaponImage()
    {
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)info.weaponId}");
    }

    public void InitWeaponInfo(WeaponInfo _info)
    {
        info = _info;
        slotBtn.onClick.AddListener(OnClickSlot);
    }

    public void SetWeaponEnhanceController(WeaponEnhancePopupController _controller)
    {
        weaponEnhancePopupController = _controller;
    }

    public void SetWeaponSelectController(WeaponSelectPopupController _controller)
    {
        weaponSelectPopupController = _controller;
    }

    public void OnClickSlot()
    {
        if (weaponEnhancePopupController != null)
        {
            weaponEnhancePopupController.SetSelectSlotWeaponImage(info.weaponId);
        }
        else
        {
            weaponSelectPopupController.SetSelectSlotWeaponImage(info.weaponId);
        }
    }
}
