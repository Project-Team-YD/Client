using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : MonoBehaviour, IPoolable
{
    private MonsterType type;
    private float hp;
    private float moveSpeed;
    private float attackDistance;
    private SpriteRenderer spriteRenderer;
    public void Init(EnemyInfo _info)
    {
        name = _info.name.ToString();
        type = _info.type;
        hp = _info.hp;
        moveSpeed = _info.moveSpeed;
        attackDistance = _info.attackDistance;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void OnMoveTarget(GameObject _target)
    {
        var direction = (_target.transform.localPosition - gameObject.transform.localPosition).normalized;
        if(direction.x < 0f)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
        transform.localPosition += (moveSpeed * Time.deltaTime) * direction;
    }

    public void OnActivate()
    {
        gameObject.SetActive(true);
    }

    public void OnDeactivate()
    {
        gameObject.SetActive(false);
    }
}
