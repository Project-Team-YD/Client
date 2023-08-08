using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottonCenter,
    BottomRight,    

    VerticalStretchLeft,
    VerticalStretchRight,
    VerticalStretchCenter,

    HorizontalStretchTop,
    HorizontalStretchMiddle,
    HorizontalStretchBottom,

    StretchAll
}
public enum PivotPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,
}
public static class RectTransformExtensions
{
    /// <summary>
    /// Setting RectTransform Anchor Preset
    /// </summary>
    /// <param name="source">Change Anchor Preset GameObject RectTransform</param>
    /// <param name="allign">AnchorPreset Enum</param>
    /// <param name="offsetX">anchorPosition.x</param>
    /// <param name="offsetY">anchorPosition.y</param>
    public static void SettingAnchorPreset(this RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
    {
        source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

        switch (allign)
        {
            case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottonCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.HorizontalStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            case (AnchorPresets.HorizontalStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
            case (AnchorPresets.HorizontalStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.VerticalStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.VerticalStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.VerticalStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
        }        
    }
    /// <summary>
    /// Setting RectTransform Anchor Pivot
    /// </summary>
    /// <param name="source">Change Pivot GameObject RectTransform</param>
    /// <param name="preset">Pivot Enum</param>
    public static void SettingPivot(this RectTransform source, PivotPresets preset)
    {
        Vector2 pivotVector = Vector2.zero;
        switch (preset)
        {
            case PivotPresets.TopLeft:
                pivotVector = new Vector2(0, 1);
                break;
            case PivotPresets.TopCenter:
                pivotVector = new Vector2(0.5f, 1);
                break;
            case PivotPresets.TopRight:
                pivotVector = new Vector2(1, 1);
                break;
            case PivotPresets.MiddleLeft:
                pivotVector = new Vector2(0, 0.5f);
                break;
            case PivotPresets.MiddleCenter:
                pivotVector = new Vector2(0.5f, 0.5f);
                break;
            case PivotPresets.MiddleRight:
                pivotVector = new Vector2(1, 0.5f);
                break;
            case PivotPresets.BottomLeft:
                pivotVector = new Vector2(0, 0);
                break;
            case PivotPresets.BottomCenter:
                pivotVector = new Vector2(0.5f, 0);
                break;
            case PivotPresets.BottomRight:
                pivotVector = new Vector2(1, 0);
                break;
        }
        source.pivot = pivotVector;
    }
}

public class ScrollViewBase : MonoBehaviour
{
    #region Inspector                     
    #endregion   

    //생성된 오브젝트들 RectTransform 및 초기 설정 Vector값
    protected RectTransform viewportRtf;
    protected RectTransform contentRtf;
    protected RectTransform scrollBarHoriRtf;
    protected RectTransform slidingAreaHoriRtf;
    protected RectTransform handleHoriRtf;
    protected RectTransform scrollBarVertiRtf;    
    protected RectTransform slidingAreaVertiRtf;    
    protected RectTransform handleVertiRtf;
    protected RectTransform scrollViewRtf;

    protected Vector2 initViewPortSize = new Vector2(-10, -10);
    protected Vector2 initContentSize = new Vector2(300, 300);
    protected Vector2 initScrollBarHoriSize = new Vector2(-10, 10);
    protected Vector2 initScrollBarVertiSize = new Vector2(10, -10);
    protected Vector2 initSlidingAreaHoriSize = Vector2.zero;
    protected Vector2 initSlidingAreaVertiSize = Vector2.zero;
    protected Vector2 initHandleHoriSize = new Vector2(30, 0);
    protected Vector2 initHandleVertiSize = new Vector2(0, 30);
    
    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
        
    }   

    //코드 받음..분석결과 Transform 매개변수 안에 들어간 오브젝트의 자식오브젝트들을 foreach로 모두 검사하며 string매개변수와 이름이 같은 오브젝트를 찾아 반환하는 함수.
    //private Transform FindChildByName(string ThisName, Transform ThisGObj)
    //{
    //    Transform ReturnObj;
    //    if (ThisGObj.name == ThisName)
    //        return ThisGObj.transform;
    //    foreach (Transform child in ThisGObj)
    //    {
    //        ReturnObj = FindChildByName(ThisName, child);
    //        if (ReturnObj != null)
    //            return ReturnObj;
    //    }
    //    return null;
    //}    
    /// <summary>
    /// Find and return the child objects of the Root game object by the name of the object....Root게임오브젝트의 자식 객체들중 해당 오브젝트의 이름으로 찾아 반환.
    /// </summary>
    /// <param name="ThisName">Find GameObjectName</param>
    /// <param name="ThisGObj">RootObjectTransform</param>
    /// <returns></returns>
    protected GameObject FindChildByName(string ThisName, Transform ThisGObj)
    {
        GameObject ReturnObj;
        if (ThisGObj.name == ThisName)
            return ThisGObj.gameObject;
        foreach (Transform child in ThisGObj)
        {
            ReturnObj = FindChildByName(ThisName, child);
            if (ReturnObj != null)
                return ReturnObj;
        }
        return null;
    }
    /// <summary>
    /// CustomClampFunction
    /// </summary>
    /// <param name="_value">TargetValue</param>
    /// <param name="_min">TargetValue <= _min = _min</param>
    /// <param name="_max">TargetValue >= _max = _max</param>
    /// <returns></returns>
    protected float CustomClamp(float _value, float _min, float _max)
    {
        if (_value <= _min)
            return _min;
        else if (_value >= _max)
            return _max;        
        else
            return _value;
    }
#if UNITY_EDITOR
    WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    protected IEnumerator CoValueChangeUpdate()
    {
        yield return endOfFrame;
        UpdateInspectorValue();
    }
#endif

    protected virtual void UpdateInspectorValue()
    {

    }
}
