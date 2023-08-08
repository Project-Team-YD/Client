using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class CustomScrollRect : ScrollViewBase, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] protected GameObject Viewport;
    [SerializeField] protected GameObject Content;

    [Header("UseScrollCheck")]
    [SerializeField] private bool horizontalScrollBar = true;
    [SerializeField] private bool verticalScrollBar = true;

    private HorizontalScrollBar barHorizontal;
    private VerticalScrollBar barVertical;
    
    private bool isDragContent = false;
    private bool isVerticalScroll;
    private bool isHorizontalScroll;

    private Vector2 prevContentAnchorPosition;
    private float maxContentPosX; // Content Max X Position 콘텐트 오브젝트 최대 X 포지션의 끝값.
    private float maxContentPosY; // Content Max Y Position  콘텐트 오브젝트 최대 Y 포지션의 끝값.
    private float contentWidth; // Content X Size 콘텐트 오브젝트 Width값.
    private float contentHeight; // Content Y Size 콘텐트 오브젝트 height값.
    private float viewportWidth; // Viewport X Size 뷰포트 오브젝트 Width값.
    private float viewportHeight; // Viewport Y Size 뷰포트 오브젝트 height값.
    private float areaHeight; // VerticalSlidingArea Height 슬라이딩 영역 높이.
    private float areaWidth; // HorizontalSlidingArea Height 슬라이딩 영역 높이.     

    protected override void Start()
    {
        GameObject horizontal = FindChildByName("ScrollBarHorizontal", transform);
        GameObject vertical = FindChildByName("ScrollBarVertical", transform);
        float width = vertical != null ? vertical.GetComponent<VerticalScrollBar>().GetWidthSize() : 0f;
        float height = horizontal != null ? horizontal.GetComponent<HorizontalScrollBar>().GetHeightSize() : 0f;
        if (horizontal != null)
        {
            barHorizontal = horizontal.GetComponent<HorizontalScrollBar>();            
            handleHoriRtf = FindChildByName("HandleHorizontal", barHorizontal.transform).GetComponent<RectTransform>();            
            scrollBarHoriRtf = barHorizontal.GetComponent<RectTransform>();
            barHorizontal.SetWidthSize(width);
        }
        if (vertical != null)
        {
            barVertical = vertical.GetComponent<VerticalScrollBar>();
            handleVertiRtf = FindChildByName("HandleVertical", barVertical.transform).GetComponent<RectTransform>();            
            scrollBarVertiRtf = barVertical.GetComponent<RectTransform>();        
            barVertical.SetHeightSize(height);
        }
        horizontalScrollBar = barHorizontal != null;
        verticalScrollBar = barVertical != null;
        viewportRtf = Viewport.GetComponent<RectTransform>();
        contentRtf = Content.GetComponent<RectTransform>();
        scrollViewRtf = gameObject.GetComponent<RectTransform>();
        viewportWidth = viewportRtf.sizeDelta.x;
        viewportHeight = viewportRtf.sizeDelta.y;
        contentWidth = contentRtf.sizeDelta.x;
        contentHeight = contentRtf.sizeDelta.y;
        maxContentPosX = -contentWidth;
        maxContentPosY = contentHeight;
        isHorizontalScroll = contentRtf.sizeDelta.x > 0f;
        isVerticalScroll = contentRtf.sizeDelta.y > 0f;
        UpdateChangeViewportSize();
        base.Start();
    }

    /// <summary>
    /// Call Function When CreateObject and Reset 
    /// </summary>
    private void InitializeScrollRect()
    {
        gameObject.AddComponent<Image>();
        Viewport = FindChildByName(nameof(Viewport), transform).gameObject;
        Content = FindChildByName(nameof(Content), transform).gameObject;

        GameObject horizontalScrollBar = FindChildByName("ScrollBarHorizontal", transform);
        GameObject verticalScrollBar = FindChildByName("ScrollBarVertical", transform);
        if (horizontalScrollBar != null)
        {
            barHorizontal = horizontalScrollBar.GetComponent<HorizontalScrollBar>();
            scrollBarHoriRtf = barHorizontal.GetComponent<RectTransform>();
        }
        if (verticalScrollBar != null)
        {
            barVertical = verticalScrollBar.GetComponent<VerticalScrollBar>();
            scrollBarVertiRtf = barVertical.GetComponent<RectTransform>();
        }

        Viewport.AddComponent<Image>();
        Viewport.AddComponent<Mask>();
        
        viewportRtf = Viewport.GetComponent<RectTransform>();
        contentRtf = Content.GetComponent<RectTransform>();

        viewportRtf.SettingAnchorPreset(AnchorPresets.StretchAll);
        viewportRtf.SettingPivot(PivotPresets.TopLeft);
        viewportRtf.sizeDelta = initViewPortSize;
        contentRtf.SettingAnchorPreset(AnchorPresets.StretchAll);
        contentRtf.SettingPivot(PivotPresets.TopLeft);
        contentRtf.sizeDelta = initContentSize;
    }    

    private void Reset()
    {        
        InitializeScrollRect();        
    }

    /// <summary>
    /// ContentSize or ViewportSize ReSetting Content and Viewport Size for Change Value
    /// </summary>
    private void UpdateChangeContentSize()
    {
        if ((contentRtf.sizeDelta.x != contentWidth) || (viewportRtf.sizeDelta.x != viewportWidth))
        {
            maxContentPosX = -contentRtf.sizeDelta.x;//(scrollViewRtf.sizeDelta.x + viewportRtf.sizeDelta.x));
            contentWidth = contentRtf.sizeDelta.x;
            viewportHeight = viewportRtf.sizeDelta.x;
        }
        if ((contentRtf.sizeDelta.y != contentHeight) || (viewportRtf.sizeDelta.y != viewportHeight))
        {
            maxContentPosY = contentRtf.sizeDelta.y;//(scrollViewRtf.sizeDelta.y + viewportRtf.sizeDelta.y);
            contentHeight = contentRtf.sizeDelta.y;
            viewportHeight = viewportRtf.sizeDelta.y;
        }
        isHorizontalScroll = contentRtf.sizeDelta.x > 0f;
        isVerticalScroll = contentRtf.sizeDelta.y > 0f;
    }

    /// <summary>
    /// Check Using kind of ScrollBar and Setting Using ScrollBar for Viewport and Content
    /// </summary>
    private void UpdateChangeViewportSize()
    {
        // Not using ScrollBar
        if (!horizontalScrollBar && !verticalScrollBar)
        {
            viewportRtf.sizeDelta = Vector2.zero;
        }
        // using Horizontal && Vertical
        else if (horizontalScrollBar && verticalScrollBar)
        {
            viewportRtf.sizeDelta = new Vector2(-scrollBarVertiRtf.sizeDelta.x, -scrollBarHoriRtf.sizeDelta.y);
            scrollBarVertiRtf.sizeDelta = new Vector2(scrollBarVertiRtf.sizeDelta.x, -scrollBarHoriRtf.sizeDelta.y);
            scrollBarHoriRtf.sizeDelta = new Vector2(-scrollBarVertiRtf.sizeDelta.x, scrollBarHoriRtf.sizeDelta.y);
        }
        // using Vertical
        else if (verticalScrollBar)
        {
            viewportRtf.sizeDelta = new Vector2(-scrollBarVertiRtf.sizeDelta.x, 0f);
            scrollBarVertiRtf.sizeDelta = new Vector2(scrollBarVertiRtf.sizeDelta.x, 0f);
            maxContentPosY = contentRtf.sizeDelta.y;
        }
        // using Horizontal
        else if (horizontalScrollBar)
        {
            viewportRtf.sizeDelta = new Vector2(0, -scrollBarHoriRtf.sizeDelta.y);
            scrollBarHoriRtf.sizeDelta = new Vector2(0, scrollBarHoriRtf.sizeDelta.y);
            maxContentPosX = -contentRtf.sizeDelta.x;
        }
    }
#if UNITY_EDITOR

    /// <summary>
    /// Function Called When Changing the Inspector Value
    /// </summary>
    protected override void UpdateInspectorValue()
    {
        if(barHorizontal != null)
            barHorizontal.gameObject.SetActive(horizontalScrollBar);
        if(barVertical != null)
            barVertical.gameObject.SetActive(verticalScrollBar);
        UpdateChangeViewportSize();
    }
    private void OnValidate()
    {                
        if(gameObject.activeSelf)
            StartCoroutine(CoValueChangeUpdate());
    }
#endif
    /// <summary>
    /// Change Value HorizontalScroll
    /// </summary>
    /// <param name="_value"></param>
    public void ChangeHorizontalValue(float _value)
    {
        barHorizontal.SetValueDragPositionChange(_value);
    }
    /// <summary>
    /// Change Value VerticalScroll
    /// </summary>
    /// <param name="_value"></param>
    public void ChangeVerticalValue(float _value)
    {
        barVertical.SetValueDragPositionChange(_value);
    }
    /// <summary>
    /// Change Content AnchorPosition by _value
    /// </summary>
    /// <param name="_value"></param>
    public void ChangeValueContentPosition(float _value)
    {
        Vector2 position = Vector2.zero;
        if(isHorizontalScroll)
        {
            position.x = maxContentPosX * _value;
        }
        if(isVerticalScroll)
        {
            position.y = maxContentPosY * _value;
        }
        contentRtf.anchoredPosition = position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateChangeContentSize();
        if(horizontalScrollBar)
            areaWidth = scrollViewRtf.sizeDelta.x + (scrollBarHoriRtf.sizeDelta.x - handleHoriRtf.sizeDelta.x);
        if(verticalScrollBar)
            areaHeight = scrollViewRtf.sizeDelta.y + (scrollBarVertiRtf.sizeDelta.y - handleVertiRtf.sizeDelta.y);
        if (eventData.pointerCurrentRaycast.gameObject == Content || eventData.pointerCurrentRaycast.gameObject == Viewport)
        {
            isDragContent = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragContent = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragContent)
        {
            prevContentAnchorPosition = contentRtf.anchoredPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragContent)
        {
            Vector2 anchoredPosition = Vector2.zero;
            if (isVerticalScroll)
            {
                var moveContentPosY = (eventData.position.y - eventData.pressPosition.y) + prevContentAnchorPosition.y;
                var handlePosY = -(moveContentPosY / maxContentPosY);
                var contentMoveMinMax = CustomClamp(moveContentPosY, 0f, maxContentPosY);
                anchoredPosition.y = contentMoveMinMax;                
                if (barVertical != null)
                {
                    var handleMinMax = CustomClamp(handlePosY, -1f, 0f);
                    handleVertiRtf.anchoredPosition = new Vector2(0f, areaHeight * handleMinMax);
                    ChangeVerticalValue(-(handleVertiRtf.anchoredPosition.y / areaHeight));
                }
            }

            if (isHorizontalScroll)
            {
                var moveContentPosX = (eventData.pressPosition.x - eventData.position.x) - prevContentAnchorPosition.x;
                var handlePosX = moveContentPosX / -maxContentPosX;
                var contentMoveMinMax = CustomClamp(moveContentPosX, 0f, -maxContentPosX);                
                anchoredPosition.x = -contentMoveMinMax;
                if (barHorizontal != null)
                {
                    var handleMinMax = CustomClamp(handlePosX, 0f, 1f);
                    handleHoriRtf.anchoredPosition = new Vector2(areaWidth * handleMinMax, 0f);
                    ChangeHorizontalValue(handleHoriRtf.anchoredPosition.x / areaWidth);
                }
            }

            contentRtf.anchoredPosition = anchoredPosition;
            #region OldCode
            //if(!GetHoriScrollBarCheck() && !GetVertiScrollBarCheck())
            //{
            //    float moveContentPosX = 0f;
            //    float contentMoveMinMaxHori = 0f;
            //    float moveContentPosY = 0f;
            //    float contentMoveMinMaxVerti = 0f;
            //    if (contentRtf.sizeDelta.x != 0f)
            //    {
            //        moveContentPosX = (eventData.pressPosition.x - eventData.position.x) - prevContentAnchorPosition.x;
            //        contentMoveMinMaxHori = CustomClamp(moveContentPosX, 0f, -maxContentPosX);
            //    }
            //    if (contentRtf.sizeDelta.y != 0f)
            //    {
            //        moveContentPosY = (eventData.position.y - eventData.pressPosition.y) + prevContentAnchorPosition.y;
            //        contentMoveMinMaxVerti = CustomClamp(moveContentPosY, 0f, -maxContentPosY);
            //    }
            //    contentRtf.anchoredPosition = new Vector2(-contentMoveMinMaxHori, contentMoveMinMaxVerti);
            //}
            //// Using Vertical Scroll
            //else if (!GetHoriScrollBarCheck())
            //{
            //    var moveContentPosY = (eventData.position.y - eventData.pressPosition.y) + prevContentAnchorPosition.y;
            //    var handlePosY = -(moveContentPosY / maxContentPosY);
            //    var contentMoveMinMax = CustomClamp(moveContentPosY, 0f, maxContentPosY);
            //    contentRtf.anchoredPosition = new Vector2(0f, contentMoveMinMax);
            //    var handleMinMax = CustomClamp(handlePosY, -1f, 0f);
            //    handleVertiRtf.anchoredPosition = new Vector2(0f, areaHeight * handleMinMax);
            //    barVertical.SetValueDragPositionChange(-(handleVertiRtf.anchoredPosition.y / areaHeight));
            //}
            //// Using Horizontal Scroll
            //else if (!GetVertiScrollBarCheck())
            //{
            //    var moveContentPosX = (eventData.pressPosition.x - eventData.position.x) - prevContentAnchorPosition.x;
            //    var handlePosX = moveContentPosX / -maxContentPosX;
            //    var contentMoveMinMax = CustomClamp(moveContentPosX, 0f, -maxContentPosX);
            //    var handleMinMax = CustomClamp(handlePosX, 0f, 1f);
            //    contentRtf.anchoredPosition = new Vector2(-contentMoveMinMax, 0f);
            //    handleHoriRtf.anchoredPosition = new Vector2(areaWidth * handleMinMax, 0f);
            //    barHorizontal.SetValueDragPositionChange(handleHoriRtf.anchoredPosition.x / areaWidth);
            //}
            //// Using All ScrollBar
            //else
            //{
            //    var moveContentPosY = (eventData.position.y - eventData.pressPosition.y) + prevContentAnchorPosition.y;
            //    var handlePosY = -(moveContentPosY / maxContentPosY);
            //    var moveContentPosX = (eventData.pressPosition.x - eventData.position.x) - prevContentAnchorPosition.x;
            //    var handlePosX = moveContentPosX / -maxContentPosX;
            //    var contentMoveMinMaxVerti = CustomClamp(moveContentPosY, 0f, maxContentPosY);
            //    var handleMinMaxVerti = CustomClamp(handlePosY, -1f, 0f);
            //    var contentMoveMinMaxHori = CustomClamp(moveContentPosX, 0f, -maxContentPosX);
            //    var handleMinMaxHori = CustomClamp(handlePosX, 0f, 1f);

            //    contentRtf.anchoredPosition = new Vector2(-contentMoveMinMaxHori, contentMoveMinMaxVerti);
            //    handleVertiRtf.anchoredPosition = new Vector2(0f, areaHeight * handleMinMaxVerti);
            //    handleHoriRtf.anchoredPosition = new Vector2(areaWidth * handleMinMaxHori, 0f);
            //}
            #endregion
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragContent = false;
    }
}
