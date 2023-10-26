using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameShopItemController : MonoBehaviour
{
    [SerializeField] private Image thisImage = null;
    [SerializeField] private Button thisButton = null;

    private void Awake()
    {
        thisButton.onClick.AddListener(OnClickButton);
    }

    public void SetShopItemData()
    {

    }

    private void OnClickButton()
    {

    }
}
