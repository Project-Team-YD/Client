using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// interface �����غ���
public class AABB
{
    private Vector3 size;
    private Transform thisTransform = null;

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

    public float GetButtom
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
        // �浹 ����
        if (GetRight < _other.GetLeft || GetLeft > _other.GetRight) return false;
        if (GetTop < _other.GetButtom || GetButtom > _other.GetTop) return false;

        // �浹
        return true;
    }
}
