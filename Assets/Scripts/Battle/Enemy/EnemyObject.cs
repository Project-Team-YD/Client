using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : MonoBehaviour, IPoolable
{
    private MonsterType type;
    private MonsterState nowState;
    private float hp;
    private float moveSpeed;
    private float attackDistance;
    private float attackPower;
    private SpriteRenderer spriteRenderer = null;
    private Animator monsterAnim = null;

    private AABB curAABB;

    [SerializeField]
    private RuntimeAnimatorController[] Anim;
    private void Awake()
    {
        monsterAnim = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void Init(EnemyInfo _info)
    {
        name = _info.name.ToString();
        type = _info.type;
        hp = _info.hp;
        moveSpeed = _info.moveSpeed;
        attackDistance = _info.attackDistance;
        attackPower = _info.attackPower;
        spriteRenderer.sprite = Resources.Load<Sprite>($"Monster/{type}/{type}_0");
        nowState = MonsterState.Chase;
        monsterAnim.runtimeAnimatorController = Anim[(int)type];
    }

    public void OnMoveTarget(Transform _target)
    {
        if (nowState != MonsterState.Die && _target != null)
        {
            var direction = (_target.localPosition - gameObject.transform.localPosition).normalized;
            bool isLeft = direction.x < 0f;
            //if (direction.x < 0f)
            //{
            //    spriteRenderer.flipX = true;
            //}
            //else
            //{
            //    spriteRenderer.flipX = false;
            //}
            spriteRenderer.flipX = isLeft;
            if (type == MonsterType.Long)
            {
                spriteRenderer.flipX = !isLeft;
            }
            transform.localPosition += (moveSpeed * direction) * Time.deltaTime;
        }
    }

    public MonsterState GetState()
    {
        return nowState;
    }

    public void SetState(MonsterState _state)
    {
        nowState = _state;
    }

    public void OnActivate()
    {
        gameObject.SetActive(true);
    }

    public void OnDeactivate()
    {
        gameObject.SetActive(false);
    }

    public bool OnCheckCollision(AABB _other)
    {
        if (curAABB == null)
        {
            var size = gameObject.GetComponent<SpriteRenderer>().size;
            curAABB = new AABB(this.transform, size);
        }

        return curAABB.CheckCollision(_other);
    }
}
