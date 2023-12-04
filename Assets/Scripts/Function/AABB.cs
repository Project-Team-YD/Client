using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB
{
    private Vector3 size;
    private Transform thisTransform = null;

    public Vector3 GetSize { get { return size; } }
    public Transform GetTransform { get { return thisTransform; } }

    /// <summary>
    /// Setting
    /// </summary>
    /// <param name="_transform"></param> Current Object
    /// <param name="_size"></param> Current Object Size
    public AABB(Transform _transform, Vector3 _size)
    {
        size = _size * 0.5f;
        thisTransform = _transform;
    }

    public float GetLeft
    {
        get { return thisTransform.position.x - size.x; }
    }

    public float GetRight
    {
        get { return thisTransform.position.x + size.x; }
    }

    public float GetTop
    {
        get { return thisTransform.position.y + size.y; }
    }

    public float GetBottom
    {
        get { return thisTransform.position.y - size.y; }
    }

    /// <summary>
    /// This Object Other Object Collision Check
    /// </summary>
    /// <param name="_other"></param> Other Object
    /// <returns></returns>
    public bool CheckCollision(AABB _other)
    {
        // 충돌 안함
        if (GetRight < _other.GetLeft || GetLeft > _other.GetRight) return false;
        if (GetTop < _other.GetBottom || GetBottom > _other.GetTop) return false;

        // 충돌
        return true;
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
    public bool CheckCollisionOBB(AABB _other)
    {
        // 거리 계산
        var dis = thisTransform.position - _other.GetTransform.position;

        // 충돌 검사
        if (CheckOBB(_other, dis, GetTransform.up))
            return true;

        if (CheckOBB(_other, dis, _other.GetTransform.up))
            return true;

        if (CheckOBB(_other, dis, GetTransform.right))
            return true;

        if (CheckOBB(_other, dis, _other.GetTransform.right))
            return true;

        // 충돌 없음
        return false;
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
    public bool CheckOBB(AABB _other, Vector3 _dis, Vector3 _axis)
    {
        var dis = Mathf.Abs(Vector3.Dot(_dis, _axis));

        if (dis > Mathf.Abs(Vector3.Dot(_axis, GetTransform.up * size.y))
            + Mathf.Abs(Vector3.Dot(_axis, GetTransform.right * size.x))
            + Mathf.Abs(Vector3.Dot(_axis, _other.GetTransform.up * _other.size.y))
            + Mathf.Abs(Vector3.Dot(_axis, _other.GetTransform.right * _other.size.x)))
        {
            return false;
        }

        return true;
    }
}
