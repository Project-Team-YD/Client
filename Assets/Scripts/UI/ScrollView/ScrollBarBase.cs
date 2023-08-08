using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScrollBarBase : ScrollViewBase
{
    [ColorUsage(true)]
    [SerializeField] protected Color32 NormalColor = new Color32(255, 255, 255, 255);
    [SerializeField] protected Color32 HighlightedColor = new Color32(255, 255, 255, 255);
    [SerializeField] protected Color32 PressedColor = new Color32(200, 200, 200, 255);
    [SerializeField] protected Color32 SelectedColor = new Color32(255, 255, 255, 255);
    [SerializeField] protected Color32 DisabledColor = new Color32(200, 200, 200, 255);
    [Header("Direction")]
    [Tooltip("Check = Left To Right")]
    [SerializeField] protected bool Direction = true;
    [Header("Transition")]
    [SerializeField] protected Image TargetGraphic;
    [Header("HandleOption")]
    [SerializeField] [Range(0, 1)] protected float Value;
    
    protected bool isClick = false;
    protected Transform scrollRootObjectTrnsform;
    protected GameObject viewportObj;    
    protected DrivenRectTransformTracker drivenRect;
    protected Vector2 localPos = Vector2.zero;    
    public void SetValueDragPositionChange(float _value)
    {
        Value = _value;
    }
    protected abstract void UpdateChangeBarSize();
    protected abstract void UpdateChangeContentSize();
#if UNITY_EDITOR
    protected abstract void UpdateChangeDirection();
    protected abstract void ScrollValueChangeInspector();
#endif
}
