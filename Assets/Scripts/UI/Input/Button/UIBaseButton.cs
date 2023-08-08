using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIBaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] protected Image target = null;

    public delegate void OnInputEvent(PointerEventData eventData);

    public OnInputEvent OnInputDown = null;
    public OnInputEvent OnInputHover = null;
    public OnInputEvent OnInputOut = null;
    public OnInputEvent OnInputUp = null;

    protected bool isTouched = false;

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        isTouched = true;

#if UNITY_EDITOR
        Debug.Log($"UI INPUT DOWN : {gameObject.name}");
#endif
    }

    //.. INFO :: Mouse Over
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log($"UI INPUT ENTER : {gameObject.name}");
#endif
    }

    //.. INFO :: Mouse Over
    public virtual void OnPointerExit(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log($"UI INPUT ENTER : {gameObject.name}");
#endif
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
#if UNITY_EDITOR
        Debug.Log($"UI INPUT UP : {gameObject.name}");
#endif
    }
}
