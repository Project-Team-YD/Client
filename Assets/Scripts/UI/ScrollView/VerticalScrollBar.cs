using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[ExecuteAlways]
public class VerticalScrollBar : ScrollBarBase, IPointerDownHandler,IPointerUpHandler, IBeginDragHandler, IDragHandler
{
    [SerializeField] protected GameObject ScrollBarVertical;
    [SerializeField] protected GameObject SlidingAreaVertical;
    [SerializeField] protected GameObject HandleVertical;
    [Header("ScrollBarSize")]
    [SerializeField] private float WidthSize = 10;

    private float scrollViewHeight; // ScrollView Y Size 커스텀 스크롤뷰 Y 크기.
    private float areaHeight; // SlidingArea Height 슬라이딩 영역 높이.    
    private Vector2 prevVerticalHandleAnchorPosition;        
    private float maxContentPosY; // Content Max Y Position  콘텐트 오브젝트 최대 Y 포지션의 끝값.
    private float contentHeight; // Content Y Size 콘텐트 오브젝트 height값.
    private float viewportHeight; // Viewport Y Size 뷰포트 오브젝트 height값.
    private float heightSize;    
    /// <summary>
    /// Call Function When CreateObject and Reset 
    /// </summary>
    protected void InitializeVertiScrollBar()
    {
        scrollRootObjectTrnsform = gameObject.transform.parent;

        //GameObject horizontalScrollBar = FindChildByName("ScrollBarHorizontal", scrollRootObjectTrnsform);
        //if (horizontalScrollBar != null)
        //{
        //    BarHorizontalRectTransform = horizontalScrollBar.GetComponent<RectTransform>();
        //}
        ScrollBarVertical = this.gameObject;
        viewportObj = gameObject.transform.parent.transform.GetChild(0).gameObject;
        SlidingAreaVertical = FindChildByName(nameof(SlidingAreaVertical), transform).gameObject;
        HandleVertical = FindChildByName(nameof(HandleVertical), transform).gameObject;

        ScrollBarVertical.AddComponent<Image>();
        HandleVertical.AddComponent<Image>();         
        scrollBarVertiRtf = ScrollBarVertical.GetComponent<RectTransform>();
        slidingAreaVertiRtf = SlidingAreaVertical.GetComponent<RectTransform>();
        handleVertiRtf = HandleVertical.GetComponent<RectTransform>();        
        viewportRtf = viewportObj.GetComponent<RectTransform>();
        contentRtf = viewportObj.transform.GetChild(0).gameObject.GetComponent<RectTransform>();

        scrollBarVertiRtf.SettingAnchorPreset(AnchorPresets.VerticalStretchRight);
        scrollBarVertiRtf.SettingPivot(PivotPresets.TopRight);
        scrollBarVertiRtf.sizeDelta = initScrollBarVertiSize;
        slidingAreaVertiRtf.SettingAnchorPreset(AnchorPresets.StretchAll);
        slidingAreaVertiRtf.SettingPivot(PivotPresets.MiddleCenter);
        slidingAreaVertiRtf.sizeDelta = initSlidingAreaVertiSize;
        handleVertiRtf.SettingAnchorPreset(AnchorPresets.HorizontalStretchTop);
        handleVertiRtf.SettingPivot(PivotPresets.TopRight);
        handleVertiRtf.sizeDelta = initHandleVertiSize;        

        drivenRect = new DrivenRectTransformTracker();
        drivenRect.Add(gameObject, scrollBarVertiRtf, DrivenTransformProperties.SizeDelta);
        drivenRect.Add(HandleVertical, handleVertiRtf, DrivenTransformProperties.SizeDeltaX);

        TargetGraphic = HandleVertical.GetComponent<Image>();        
    }    

    private void Reset()
    {
        InitializeVertiScrollBar();
    }

    protected override void Start()
    {
        scrollRootObjectTrnsform = gameObject.transform.parent;
        //GameObject horizontalScrollBar = FindChildByName("ScrollBarHorizontal", scrollRootObjectTrnsform);
        //if (horizontalScrollBar != null)
        //{
        //    BarHorizontalRectTransform = horizontalScrollBar.GetComponent<RectTransform>();            
        //}
        scrollViewRtf = scrollRootObjectTrnsform.gameObject.GetComponent<RectTransform>();
        viewportObj = scrollRootObjectTrnsform.GetChild(0).gameObject;
        scrollBarVertiRtf = ScrollBarVertical.GetComponent<RectTransform>();
        slidingAreaVertiRtf = SlidingAreaVertical.GetComponent<RectTransform>();
        handleVertiRtf = HandleVertical.GetComponent<RectTransform>();
        viewportRtf = viewportObj.GetComponent<RectTransform>();        
        contentRtf = viewportObj.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        TargetGraphic = HandleVertical.GetComponent<Image>();        
        scrollViewHeight = scrollViewRtf.sizeDelta.y;        
        drivenRect = new DrivenRectTransformTracker();
        drivenRect.Add(gameObject, scrollBarVertiRtf, DrivenTransformProperties.SizeDelta);
        drivenRect.Add(HandleVertical, handleVertiRtf, DrivenTransformProperties.SizeDeltaX);
        contentHeight = contentRtf.sizeDelta.y;
        viewportHeight = viewportRtf.sizeDelta.y;
        maxContentPosY = contentRtf.sizeDelta.y;
        UpdateChangeBarSize();
    }

    /// <summary>
    /// maxContentPosX and ContentWidth and ViewportWidth Resetting When Change ContentWidth or Change ViewportWidth
    /// </summary>
    protected override void UpdateChangeContentSize()
    {
        contentHeight = contentRtf.sizeDelta.y;
        maxContentPosY = contentHeight; //- (scrollViewHeight + viewportRtf.sizeDelta.y);
        viewportHeight = viewportRtf.sizeDelta.y;
    }

    /// <summary>
    /// Called Function When Change ScrollBar Handle Height and Start Point
    /// </summary>
    protected override void UpdateChangeBarSize()
    {
        Vector2 position = Vector2.zero;
        RectTransform BarHorizontalRectTransform;
        GameObject horizontalScrollBar = FindChildByName("ScrollBarHorizontal", gameObject.transform.parent);
        if (horizontalScrollBar != null)
        {
            BarHorizontalRectTransform = horizontalScrollBar.GetComponent<RectTransform>();
            SetHeightSize(BarHorizontalRectTransform.sizeDelta.y);
            position.x = WidthSize;
            position.y = -heightSize;
            scrollBarVertiRtf.sizeDelta = position;
            BarHorizontalRectTransform.sizeDelta = new Vector2(-scrollBarVertiRtf.sizeDelta.x, heightSize);
        }
        else
        {
            position.x = WidthSize;
            scrollBarVertiRtf.sizeDelta = position;
        }
        areaHeight = scrollViewHeight + (scrollBarVertiRtf.sizeDelta.y - handleVertiRtf.sizeDelta.y);
        UpdateChangeContentSize();
        viewportRtf.sizeDelta = new Vector2(-scrollBarVertiRtf.sizeDelta.x, viewportHeight);
    }

#if UNITY_EDITOR     
    /// <summary>
    /// Called Function When Change ScrollBar Direction and Start Point
    /// </summary>
    protected override void UpdateChangeDirection()
    {
        if (Direction)
        {
            handleVertiRtf.SettingAnchorPreset(AnchorPresets.HorizontalStretchTop);
            handleVertiRtf.SettingPivot(PivotPresets.TopRight);
        }
        else
        {
            handleVertiRtf.SettingAnchorPreset(AnchorPresets.HorizontalStretchBottom);
            handleVertiRtf.SettingPivot(PivotPresets.BottomRight);
        }
    }

    /// <summary>
    /// Called Function When Change Inspector Option "Value" value and Start Point
    /// </summary>
    protected override void ScrollValueChangeInspector()
    {
        Vector2 position = Vector2.zero;
        areaHeight = scrollViewHeight + (scrollBarVertiRtf.sizeDelta.y - handleVertiRtf.sizeDelta.y);
        var handlePosY = areaHeight * Value;
        var contentPosY = maxContentPosY * Value;
        float handleMinMax = -CustomClamp(handlePosY, 0f, areaHeight);
        float contentMinMax = CustomClamp(contentPosY, 0f, maxContentPosY);
        position.y = handleMinMax;
        handleVertiRtf.anchoredPosition = position;
        position.x = contentRtf.anchoredPosition.x;
        position.y = contentMinMax;
        contentRtf.anchoredPosition = position;
    }

    protected override void UpdateInspectorValue()
    {
        UpdateChangeBarSize();
        UpdateChangeDirection();
        ScrollValueChangeInspector();
    }
    private void OnValidate()
    {
        if(gameObject.activeSelf)
            StartCoroutine(CoValueChangeUpdate());
    }
#endif

    public void SetHeightSize(float _size)
    {
        heightSize = _size;
    }
    public float GetWidthSize()
    {
        return WidthSize;
    }

    public void OnPointerDown(PointerEventData eventData)
    {        
        Vector2 position = new Vector2();
        if (eventData.pointerCurrentRaycast.gameObject == ScrollBarVertical || eventData.pointerCurrentRaycast.gameObject == HandleVertical)
        {
            areaHeight = scrollViewHeight + (scrollBarVertiRtf.sizeDelta.y - handleVertiRtf.sizeDelta.y);
            TargetGraphic.color = PressedColor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(scrollBarVertiRtf, eventData.position, Camera.main, out localPos);
            var posY = eventData.position.y + localPos.y;
            var contentPosY = -(posY / areaHeight);            
            #region Handle and Content position Min Max Check
            float handleMinMax = CustomClamp(posY, -areaHeight, 0f);
            float contentMinMax = CustomClamp(contentPosY, 0f, 1f);
            position.y = handleMinMax;
            handleVertiRtf.anchoredPosition = position;
            position.y = maxContentPosY * contentMinMax;
            position.x = contentRtf.anchoredPosition.x;
            contentRtf.anchoredPosition = position;
            #endregion
            Value = -(handleVertiRtf.anchoredPosition.y / areaHeight);
        }        
    }

    public void OnPointerUp(PointerEventData eventData)
    {                
        TargetGraphic.color = NormalColor;
    }    

    public void OnBeginDrag(PointerEventData eventData)
    {        
        prevVerticalHandleAnchorPosition = handleVertiRtf.anchoredPosition;        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = new Vector2();
        var moveHandlePosY = (eventData.position.y - eventData.pressPosition.y) + prevVerticalHandleAnchorPosition.y;
        var contentPosY = -(moveHandlePosY / areaHeight);
        #region Handle and Content position Min Max Check
        float handleMinMax = CustomClamp(moveHandlePosY, -areaHeight, 0f);
        float contentMinMax = CustomClamp(contentPosY, 0f, 1f);
        position.y = handleMinMax;
        handleVertiRtf.anchoredPosition = position;
        position.y = maxContentPosY * contentMinMax;
        position.x = contentRtf.anchoredPosition.x;
        contentRtf.anchoredPosition = position;
        #endregion
        Value = -(handleVertiRtf.anchoredPosition.y / areaHeight);
    }    
}
