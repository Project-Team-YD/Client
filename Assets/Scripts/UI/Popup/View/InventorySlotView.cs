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

    /// <summary>
    /// 무기 이미지 지정 함수.
    /// </summary>
    public void SetWeaponImage()
    {
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)info.weaponId}");
    }
    /// <summary>
    /// 무기 객체정보 저장 및 버튼에 함수 추가.
    /// </summary>
    /// <param name="_info">무기 객체정보</param>
    public void InitWeaponInfo(WeaponInfo _info)
    {
        info = _info;
        slotBtn.onClick.AddListener(OnClickSlot);
    }
    /// <summary>
    /// 무기 강화 Controller 저장 함수.
    /// </summary>
    /// <param name="_controller">WeaponEnhancePopupController</param>
    public void SetWeaponEnhanceController(WeaponEnhancePopupController _controller)
    {
        weaponEnhancePopupController = _controller;
    }
    /// <summary>
    /// 게임 시작전 무기 선택팝업 Controller 저장 함수.
    /// </summary>
    /// <param name="_controller">WeaponSelectPopupController</param>
    public void SetWeaponSelectController(WeaponSelectPopupController _controller)
    {
        weaponSelectPopupController = _controller;
    }
    /// <summary>
    /// 슬릇 버튼 클릭 이벤트.
    /// </summary>
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
