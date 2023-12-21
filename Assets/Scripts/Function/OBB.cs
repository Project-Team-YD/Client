using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OBB 
{
    private const float SIZE_CORRECTION = 0.5f;
    private Vector3 halfSize;
    private Transform thisTransform = null;

    public Vector3 GetSize { get { return halfSize; } }
    public Transform GetTransform { get { return thisTransform; } }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="_transform"></param> Current Object
    /// <param name="_size"></param> Current Object Size
    public OBB(Transform _transform, Vector3 _size)
    {
        halfSize = _size * SIZE_CORRECTION;
        thisTransform = _transform;
    }

    public float GetLeft
    {
        get { return thisTransform.position.x - halfSize.x; }
    }

    public float GetRight
    {
        get { return thisTransform.position.x + halfSize.x; }
    }

    public float GetTop
    {
        get { return thisTransform.position.y + halfSize.y; }
    }

    public float GetBottom
    {
        get { return thisTransform.position.y - halfSize.y; }
    }

    // 주어진 좌표를 기준점을 중심으로 angle만큼 회전시키는 함수
    public Vector3 RotatePoint(Vector3 _point)
    {
        // 기존 좌표에서 플레이어 위치를 빼고
        var dir = _point - thisTransform.position;
        // 틀어질 각도를 곱한다
        var point = Quaternion.Euler(thisTransform.rotation.eulerAngles) * dir;
        return point + thisTransform.position;
    }

    /// <summary>
    /// 회전된 오브젝트 충돌체크 OBB 적용
    /// </summary>
    /// <param name="_other"></param>
    /// <returns></returns>
    public bool CheckCollision(OBB _other)
    {
        // 거리 계산
        var dis = thisTransform.position - _other.GetTransform.position;

        // 충돌 검사
        if (CheckOBB(_other, dis, GetTransform.right) == false)
            return false;

        if (CheckOBB(_other, dis, GetTransform.up) == false)
            return false;

        if (CheckOBB(_other, dis, _other.GetTransform.right) == false)
            return false;

        if (CheckOBB(_other, dis, _other.GetTransform.up) == false)
            return false;

        return true;
    }

    /// <summary>
    /// Vector3.Dot 벡터 사이의 각도
    /// up 상단 벡터
    /// right 오른쪽방향 벡터
    /// </summary>
    /// <param name="_other"></param>   대상 오브젝트
    /// <param name="_dis"></param>     두 오브젝트의 거리 벡터
    /// <param name="_axis"></param>    체크할 기준 축
    /// <returns></returns>
    public bool CheckOBB(OBB _other, Vector3 _dis, Vector3 _axis)
    {
        // 거리 벡터
        var dis = Mathf.Abs(Vector3.Dot(_dis, _axis));

        if (dis > Mathf.Abs(Vector3.Dot(_axis, GetTransform.up * halfSize.y))
            + Mathf.Abs(Vector3.Dot(_axis, GetTransform.right * halfSize.x))
            + Mathf.Abs(Vector3.Dot(_axis, _other.GetTransform.up * _other.halfSize.y))
            + Mathf.Abs(Vector3.Dot(_axis, _other.GetTransform.right * _other.halfSize.x)))
        {
            return false;
        }

        return true;
    }
}
