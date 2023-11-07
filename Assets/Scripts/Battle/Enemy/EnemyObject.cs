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
    /// <summary>
    /// 몬스터 초기화 함수.
    /// </summary>
    /// <param name="_info">몬스터 객체 정보</param>
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
    /// <summary>
    /// 몬스터가 타겟으로 계속 이동하는 함수.
    /// </summary>
    /// <param name="_target">플레이어</param>
    public void OnMoveTarget(Transform _target)
    {
        if (nowState != MonsterState.Die)
        {           
            if (nowState != MonsterState.Hit && _target != null)
            {
                var direction = (_target.localPosition - gameObject.transform.localPosition).normalized;
                bool isLeft = direction.x < 0f;                
                spriteRenderer.flipX = isLeft;
                if (type == MonsterType.Long)
                {
                    spriteRenderer.flipX = !isLeft;
                }
                transform.localPosition += (moveSpeed * direction) * Time.deltaTime;
            }
        }
    }
    /// <summary>
    /// 몬스터가 공격 받았을때 피드백 이벤트 함수.
    /// </summary>
    /// <param name="_target">플레이어</param>
    public void SetAttack(Transform _target)
    {
        var direction = -(_target.localPosition - gameObject.transform.localPosition).normalized;
        transform.localPosition += (direction * 100f) * Time.deltaTime;
        nowState = MonsterState.Chase;
    }
    /// <summary>
    /// 몬스터가 공격 받았을때 hp계산 함수.
    /// </summary>
    /// <param name="_power">무기 공격력</param>
    public void SetDamage(float _power)
    {
        hp -= _power;
    }
    /// <summary>
    /// 몬스터 죽었는지 확인하는 함수.
    /// </summary>
    /// <returns></returns>
    public bool IsDie()
    {
        return hp <= 0;
    }
    /// <summary>
    /// 몬스터 현재 상태를 반환하는 함수.
    /// </summary>
    /// <returns></returns>
    public MonsterState GetState()
    {
        return nowState;
    }
    /// <summary>
    /// 몬스터 현재 상태를 지정하는 함수.
    /// </summary>
    /// <param name="_state">몬스터 상태값</param>
    public void SetState(MonsterState _state)
    {
        nowState = _state;
    }
    /// <summary>
    /// 몬스터 SetActive(true)
    /// </summary>
    public void OnActivate()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 몬스터 SetActive(false)
    /// </summary>
    public void OnDeactivate()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 몬스터 충돌 체크 함수.(AABB 알고리즘 사용)
    /// </summary>
    /// <param name="_other"></param>
    /// <returns></returns>
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
