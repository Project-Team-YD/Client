using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIJoyPad : MoveHandler
{
    private const float HALF = 0.5f;

    private RectTransform joypadBackgroundRectTransform = null;
    private RectTransform joypadStickRectTransform = null;

    private bool isUsingJoypad = true;
    private bool isPushJoypad = false;

    private float maxDistance = 0f;
    private Vector2 direction = Vector2.zero;
    private float distance = 0f;
    private float joypadInputSpeed = 0f;

    // joypad Transform setting
    public UIJoyPad(RectTransform _joypadBackground, RectTransform _joypadStick)
    {
        joypadBackgroundRectTransform = _joypadBackground;
        joypadStickRectTransform = _joypadStick;

        joypadBackgroundRectTransform.gameObject.SetActive(false);
    }

    /// <summary>
    /// Joypad inputting
    /// </summary>
    /// <param name="_inputPos"></param> input position    
    public void OnJoypad(Vector2 _inputPos)
    {
        if (isPushJoypad == true)
        {
            MoveJoypad(_inputPos);
        }
    }

    /// <summary>
    /// target move
    /// </summary>
    private void MoveUpdate()
    {
        OnMove(direction, joypadInputSpeed);
    }

    /// <summary>
    /// Calculating joypad input direction speed
    /// </summary>
    /// <param name="_inputPos"></param> input position
    private void MoveJoypad(Vector2 _inputPos)
    {
        Vector2 joypadBackgroundPos = joypadBackgroundRectTransform.position;

        float distanceInput = Vector2.Distance(joypadBackgroundPos, _inputPos);

        // 방향 벡터
        direction = (_inputPos - joypadBackgroundPos).normalized;

        UpdateJoypadUI(joypadBackgroundPos, distanceInput, direction);

        distance = Vector2.Distance(joypadBackgroundRectTransform.position, joypadStickRectTransform.position);

        // speed를 0~1로 계산
        // joypadInputSpeed = distance / maxDistance; // 조이스틱의 움직임 반영
        joypadInputSpeed = 1f;

        MoveUpdate();
    }

    /// <summary>
    /// joypad ui renewal
    /// </summary>
    /// <param name="_joypadBackgroundPos"></param> joypadBackground ui position
    /// <param name="_distanceInput"></param> input distance
    /// <param name="_direction"></param> input direction
    private void UpdateJoypadUI(Vector2 _joypadBackgroundPos, float _distanceInput, Vector2 _direction)
    {
        float distancePos = _distanceInput >= maxDistance ? maxDistance : _distanceInput;

        Vector2 moveJoypad = _direction * distancePos;
        joypadStickRectTransform.position = _joypadBackgroundPos + moveJoypad;
    }

    /// <summary>
    /// input down
    /// </summary>
    /// <param name="_inputPos"></param> input position
    public void OnJoypadDown(Vector2 _inputPos)
    {
        if (isUsingJoypad == false)
        {
            return;
        }

        Rect joypadRect = joypadBackgroundRectTransform.rect;

        joypadBackgroundRectTransform.position = _inputPos;

        maxDistance = joypadRect.width * HALF;
        isPushJoypad = true;

        joypadBackgroundRectTransform.gameObject.SetActive(true);

        return;

        #region Joypad Fix
        // 조이패드 고정일때 조이패드 위치 확인
        //Vector2 joypadPos = new Vector2(joypadBackgroundRectTransform.position.x - (joypadRect.width * HALF), joypadBackgroundRectTransform.position.y - (joypadRect.height * HALF));

        //if (joypadPos.x < _inputPos.x && joypadPos.x + joypadRect.width > _inputPos.x
        //    && joypadPos.y < _inputPos.y && joypadPos.y + joypadRect.height > _inputPos.y)
        //{
        //    maxDistance = joypadRect.width * HALF;
        //    isPushJoypad = true;
        //}
        #endregion
    }

    /// <summary>
    /// Reset after input
    /// </summary>
    public void OnJoypadUp()
    {
        joypadBackgroundRectTransform.gameObject.SetActive(false);

        isPushJoypad = false;
        joypadStickRectTransform.position = joypadBackgroundRectTransform.position;
        direction = Vector2.zero;
        distance = 0f;
        joypadInputSpeed = 0f;
    }
}
