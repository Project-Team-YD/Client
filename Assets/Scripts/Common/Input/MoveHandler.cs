using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    public void OnMove(Vector3 _dir, float _speed);
}

public class MoveHandler
{
    protected IMoveable moveTarget = null; 

    public void SetTarget(IMoveable _target)
    {
        moveTarget = _target;
    }

    protected virtual void OnMove(Vector3 _dir, float _speed)
    {
        moveTarget.OnMove(_dir, _speed);
    }
}
