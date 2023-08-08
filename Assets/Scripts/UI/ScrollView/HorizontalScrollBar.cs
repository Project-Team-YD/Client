using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class HorizontalScrollBar : ScrollBarBase, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler
{
    [SerializeField] protected GameObject ScrollBarHorizontal;
    [SerializeField] protected GameObject SlidingAreaHorizontal;
    [SerializeField] protected GameObject HandleHorizontal;    
    [Header("ScrollBarSize")]
    [SerializeField] private float HeightSize = 10;
    
    private float scrollViewWidth; // ScrollView X Size 커스텀 스크롤뷰 X 크기.
    private float areaWidth; // SlidingArea Width 슬라이딩 영역 길이.
    private Vector2 prevHorizontalHandleAnchorPosition;    
    private float maxContentPosX; // Content Max X Position 콘텐트 오브젝트 최대 X 포지션의 끝값.
    private float contentWidth; // Content X Size 콘텐트 오브젝트 width값.
    private float viewportWidth; // Viewport X Size 뷰포트 오브젝트 width값.
    private float widthSize;

    /// <summary>
    /// Call Function When CreateObject and Reset 
    /// </summary>
    protected void InitializeHoriScrollBar()
    {
        scrollRootObjectTrnsform = gameObject.transform.parent;

        //GameObject verticalScrollBar = FindChildByName("ScrollBarVertical", scrollRootObjectTrnsform);
        //if (verticalScrollBar != null)
        //{
        //    BarVerticalRectTransform = verticalScrollBar.GetComponent<RectTransform>();
        //}
        ScrollBarHorizontal = this.gameObject;
        viewportObj = scrollRootObjectTrnsform.GetChild(0).gameObject;
        SlidingAreaHorizontal = FindChildByName(nameof(SlidingAreaHorizontal), transform).gameObject;
        HandleHorizontal = FindChildByName(nameof(HandleHorizontal), transform).gameObject;       

        ScrollBarHorizontal.AddComponent<Image>();
        HandleHorizontal.AddComponent<Image>();
        scrollBarHoriRtf = ScrollBarHorizontal.GetComponent<RectTransform>();
        slidingAreaHoriRtf = SlidingAreaHorizontal.GetComponent<RectTransform>();
        handleHoriRtf = HandleHorizontal.GetComponent<RectTransform>();        
        viewportRtf = viewportObj.GetComponent<RectTransform>();
        contentRtf = viewportObj.transform.GetChild(0).gameObject.GetComponent<RectTransform>();

        scrollBarHoriRtf.SettingAnchorPreset(AnchorPresets.HorizontalStretchBottom);
        scrollBarHoriRtf.SettingPivot(PivotPresets.BottomLeft);
        scrollBarHoriRtf.sizeDelta = initScrollBarHoriSize;
        slidingAreaHoriRtf.SettingAnchorPreset(AnchorPresets.StretchAll);
        slidingAreaHoriRtf.SettingPivot(PivotPresets.MiddleCenter);
        slidingAreaHoriRtf.sizeDelta = initSlidingAreaHoriSize;
        handleHoriRtf.SettingAnchorPreset(AnchorPresets.VerticalStretchLeft);
        handleHoriRtf.SettingPivot(PivotPresets.BottomLeft);
        handleHoriRtf.sizeDelta = initHandleHoriSize;        

        drivenRect = new DrivenRectTransformTracker();
        drivenRect.Add(gameObject, scrollBarHoriRtf, DrivenTransformProperties.SizeDelta);
        drivenRect.Add(HandleHorizontal, handleHoriRtf, DrivenTransformProperties.SizeDeltaY);

        TargetGraphic = HandleHorizontal.GetComponent<Image>();
    }    

    private void Reset()
    {        
        InitializeHoriScrollBar();        
    }    

    protected override void Start()
    {
        scrollRootObjectTrnsform = gameObject.transform.parent;
        //GameObject verticalScrollBar = FindChildByName("ScrollBarVertical", scrollRootObjectTrnsform);
        //if(verticalScrollBar != null)
        //{
        //    BarVerticalRectTransform = verticalScrollBar.GetComponent<RectTransform>();            
        //}
        scrollViewRtf = scrollRootObjectTrnsform.gameObject.GetComponent<RectTransform>();
        viewportObj = scrollRootObjectTrnsform.GetChild(0).gameObject;
        scrollBarHoriRtf = ScrollBarHorizontal.GetComponent<RectTransform>();
        slidingAreaHoriRtf = SlidingAreaHorizontal.GetComponent<RectTransform>();
        handleHoriRtf = HandleHorizontal.GetComponent<RectTransform>();
        viewportRtf = viewportObj.GetComponent<RectTransform>();        
        contentRtf = viewportObj.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        TargetGraphic = HandleHorizontal.GetComponent<Image>();        
        scrollViewWidth = scrollViewRtf.sizeDelta.x;
        drivenRect = new DrivenRectTransformTracker();
        drivenRect.Add(gameObject, scrollBarHoriRtf, DrivenTransformProperties.SizeDelta);
        drivenRect.Add(HandleHorizontal, handleHoriRtf, DrivenTransformProperties.SizeDeltaY);
        contentWidth = contentRtf.sizeDelta.x;
        viewportWidth = viewportRtf.sizeDelta.x;
        maxContentPosX = -contentRtf.sizeDelta.x;
        UpdateChangeBarSize();
    }

    /// <summary>
    /// maxContentPosX and ContentWidth and ViewportWidth Resetting When Change ContentWidth or Change ViewportWidth
    /// </summary>
    protected override void UpdateChangeContentSize()
    {
        if ((contentRtf.sizeDelta.x != contentWidth) || (viewportRtf.sizeDelta.x != viewportWidth))
        {
            contentWidth = contentRtf.sizeDelta.x;
            maxContentPosX = -contentWidth;//(scrollViewWidth + viewportRtf.sizeDelta.x) - contentRtf.sizeDelta.x;
            viewportWidth = viewportRtf.sizeDelta.x;
        }
    }

    /// <summary>
    /// Called Function When Change ScrollBar Handle Width and Start Point
    /// </summary>
    protected override void UpdateChangeBarSize()
    {
        Vector2 position = Vector2.zero;
        RectTransform BarVerticalRectTransform;
        GameObject verticalScrollBar = FindChildByName("ScrollBarVertical", gameObject.transform.parent);
        if (verticalScrollBar != null)
        {
            BarVerticalRectTransform = verticalScrollBar.GetComponent<RectTransform>();
            SetWidthSize(BarVerticalRectTransform.sizeDelta.x);
            position.x = -widthSize;
            position.y = HeightSize;
            scrollBarHoriRtf.sizeDelta = position;
            BarVerticalRectTransform.sizeDelta = new Vector2(widthSize, -scrollBarHoriRtf.sizeDelta.y);
        }
        else
        {
            position.y = HeightSize;
            scrollBarHoriRtf.sizeDelta = position;
        }
        areaWidth = scrollViewWidth + (scrollBarHoriRtf.sizeDelta.x - handleHoriRtf.sizeDelta.x);
        UpdateChangeContentSize();
        viewportRtf.sizeDelta = new Vector2(viewportWidth, -scrollBarHoriRtf.sizeDelta.y);
    }
#if UNITY_EDITOR      
    /// <summary>
    /// Called Function When Change ScrollBar Direction and Start Point
    /// </summary>
    protected override void UpdateChangeDirection()
    {
        if (Direction)
        {
            handleHoriRtf.SettingAnchorPreset(AnchorPresets.VerticalStretchLeft);
            handleHoriRtf.SettingPivot(PivotPresets.BottomLeft);
        }
        else
        {
            handleHoriRtf.SettingAnchorPreset(AnchorPresets.VerticalStretchRight);
            handleHoriRtf.SettingPivot(PivotPresets.BottomRight);
        }
    }

    /// <summary>
    /// Called Function When Change Inspector Option "Value" value and Start Point
    /// </summary>
    protected override void ScrollValueChangeInspector()
    {
        Vector2 position = Vector2.zero;
        areaWidth = scrollViewWidth + (scrollBarHoriRtf.sizeDelta.x - handleHoriRtf.sizeDelta.x);
        var handlePosX = areaWidth * Value;
        var contentPosX = maxContentPosX * Value;
        float handleMinMax = CustomClamp(handlePosX, 0f, areaWidth);
        float contentMinMax = CustomClamp(contentPosX, maxContentPosX, 0f);
        position.x = handleMinMax;
        handleHoriRtf.anchoredPosition = position;
        position.x = contentMinMax;
        position.y = contentRtf.anchoredPosition.y;
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

    public void SetWidthSize(float _size)
    {
        widthSize = _size;
    }
    public float GetHeightSize()
    {
        return HeightSize;
    }

    public void OnPointerDown(PointerEventData eventData)
    {        
        Vector2 position = new Vector2();
        if(eventData.pointerCurrentRaycast.gameObject == ScrollBarHorizontal || eventData.pointerCurrentRaycast.gameObject == HandleHorizontal)
        {
            areaWidth = scrollViewWidth + (scrollBarHoriRtf.sizeDelta.x - handleHoriRtf.sizeDelta.x);
            TargetGraphic.color = PressedColor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(scrollBarHoriRtf, eventData.position, Camera.main, out localPos);
            var posX = eventData.position.x + localPos.x;
            var contentPosX = posX / areaWidth;
            #region Handle and Content position Min Max Check
            float handleMinMax = CustomClamp(posX, 0f, areaWidth);
            float contentMinMax = CustomClamp(contentPosX, 0f, 1f);
            position.x = handleMinMax;
            handleHoriRtf.anchoredPosition = position;
            position.x = maxContentPosX * contentMinMax;
            position.y = contentRtf.anchoredPosition.y;
            contentRtf.anchoredPosition = position;
            #endregion
            Value = handleHoriRtf.anchoredPosition.x / areaWidth;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {        
        TargetGraphic.color = NormalColor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        prevHorizontalHandleAnchorPosition = handleHoriRtf.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = new Vector2();
        var moveHandlePosX = (eventData.position.x - eventData.pressPosition.x) + prevHorizontalHandleAnchorPosition.x;
        var contentPosX = moveHandlePosX / areaWidth;
        #region Handle and Content position Min Max Check
        float handleMinMax = CustomClamp(moveHandlePosX, 0f, areaWidth);
        float contentMinMax = CustomClamp(contentPosX, 0f, 1f);
        position.x = handleMinMax;
        handleHoriRtf.anchoredPosition = position;
        position.x = maxContentPosX * contentMinMax;
        position.y = contentRtf.anchoredPosition.y;
        contentRtf.anchoredPosition = position;
        #endregion
        Value = handleHoriRtf.anchoredPosition.x / areaWidth;
    }
}
