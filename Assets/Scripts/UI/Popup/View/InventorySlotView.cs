using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour
{
    [SerializeField] private Image choiceImage = null;
    [SerializeField] private Image weaponImage = null;
    [SerializeField] private Button slotBtn = null;
    [SerializeField] private TextMeshProUGUI enhanceText = null;

    private WeaponEnhancePopupController weaponEnhancePopupController = null;
    private WeaponSelectPopupController weaponSelectPopupController = null;
    private int id;

    /// <summary>
    /// 무기 이미지 지정 함수.
    /// </summary>
    public void SetWeaponImage()
    {
        weaponImage.color = Color.white;
        weaponImage.sprite = Resources.Load<Sprite>($"Weapon/{(WeaponType)id}");
    }
    /// <summary>
    /// 무기 객체정보 저장 및 버튼에 함수 추가.
    /// </summary>
    /// <param name="_info">무기 객체정보</param>
    public void InitWeaponInfo(int _id, int _enchant)
    {
        id = _id;
        enhanceText.text = $"+{_enchant}";
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
        weaponEnhancePopupController = null;
        weaponSelectPopupController = _controller;
    }
    /// <summary>
    /// 슬릇 선택 효과 이미지 On / Off 함수.
    /// </summary>
    /// <param name="_isOn">슬릇 선택 On / Off값</param>
    public void OnOffChoiceEffectImage(bool _isOn)
    {
        choiceImage.enabled = _isOn;
    }
    /// <summary>
    /// 슬릇 버튼 클릭 이벤트.
    /// </summary>
    public void OnClickSlot()
    {
        if (weaponEnhancePopupController != null)
        {
            weaponEnhancePopupController.SetSelectSlotWeaponImage(id);
        }
        else
        {
            weaponSelectPopupController.SetSelectSlotWeaponImage(id);
        }
    }
}
