using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnhanceInfoView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enhanceText = null;
    [SerializeField] TextMeshProUGUI probabilityText = null;
    [SerializeField] TextMeshProUGUI priceText = null;

    public void InitInfoView(int _enhance)
    {
        var data = TableManager.getInstance.GetWeaponEnchantInfo(_enhance);
        enhanceText.text = $"+{data.enchant}";
        probabilityText.text = $"{data.probability / 10000}%";
        priceText.text = $"{data.price}";
    }
}
