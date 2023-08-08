using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIBaseToggle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image target = null;

    protected bool isTouched = false;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isTouched = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        
    }
}
