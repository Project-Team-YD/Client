using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceInfoController : MonoBehaviour
{
    [SerializeField] GameObject infoObject = null;
    [SerializeField] Transform infoRootTransform = null;
    [SerializeField] Button closeBtn = null;

    private void Awake()
    {
        Initialize();
        closeBtn.onClick.AddListener(OnClickCloseButton);
    }

    private void Initialize()
    {
        int infoCount = TableManager.getInstance.GetEnchantInfoCount();
        for (int i = 1; i <= infoCount; i++)
        {
            GameObject newObject = GameObject.Instantiate(infoObject, infoRootTransform);
            var componenet = newObject.GetComponent<EnhanceInfoView>();
            componenet.InitInfoView(i);
        }
    }

    private void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}
