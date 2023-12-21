using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB
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
    public AABB(Transform _transform, Vector3 _size)
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
}
