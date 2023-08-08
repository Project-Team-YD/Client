using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Extensions;

public class BasePlayerController : IMoveable
{
    protected Transform transform = null;

    public BasePlayerController(Transform _transform)
    {
        transform = _transform;
    }

    public void SetInputHandler(MoveHandler _handler)
    {
        _handler.SetTarget(this);
    }

    public virtual void OnMove(float _rot, float _speed)
    {
        float speed = _speed * Time.deltaTime;
        //float rot = Mathf.PI / 180f * _rot;

        Vector3 curPos = transform.position;
        curPos.x += Mathf.Cos(_rot) * speed;
        curPos.y += Mathf.Sin(_rot) * speed;

        transform.position = curPos;
    }

    public virtual void OnMove(Vector3 _dir, float _speed)
    {
        if (_dir == Vector3.zero)
            return;

        float rot = MathHelper.TwoPointRadian(_dir, Vector2.zero);
        OnMove(rot, _speed);
    }
}
