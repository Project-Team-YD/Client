using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : MonoBehaviour, IPoolable
{
    private MonsterType type;
    private MonsterState nowState;
    private float hp;
    private float maxHP;
    private float moveSpeed;
    private float attackDistance;
    private float attackPower;
    private SpriteRenderer spriteRenderer = null;
    private Animator monsterAnim = null;

    private Transform targetTransform;
    private AABB curAABB;

    [SerializeField] private RuntimeAnimatorController[] Anim;
    [SerializeField] private Transform HUDTransform;
    [SerializeField] private GameObject BossAttackPattern;
    [SerializeField] private GameObject[] BossAttackPatterns;
    private void Awake()
    {
        monsterAnim = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// 몬스터객체의 HUD Transform 반환 함수.
    /// </summary>
    /// <returns></returns>
    public Transform GetHUDTransform()
    {
        return HUDTransform;
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
        maxHP = hp;
        moveSpeed = _info.moveSpeed;
        attackDistance = _info.attackDistance;
        attackPower = _info.attackPower;
        var sprite = Resources.Load<Sprite>($"Monster/{type}/{type}");
        spriteRenderer.sprite = sprite;
        monsterAnim.runtimeAnimatorController = Anim[(int)type];
        nowState = MonsterState.Chase;
        var tempSize = sprite.rect.size / sprite.pixelsPerUnit;
        curAABB = new AABB(this.transform, tempSize);
        if(type == MonsterType.Boss)
        {
            BossAttackPattern.SetActive(true);
        }
        else
        {
            BossAttackPattern.SetActive(false);
        }
    }
    public float GetMaxHp()
    {
        return maxHP;
    }

    public float GetCurrentHp()
    {
        return hp;
    }
    /// <summary>
    /// 게임 웨이브에 따른 몬스터 스펙 강화 함수.
    /// </summary>
    /// <param name="_wave">게임 웨이브</param>
    public void WaveEnhanceMonster(float _wave)
    {
        hp += (_wave * 0.5f * hp);
        attackPower += (_wave * 0.5f * attackPower);
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
    /// Boss몬스터 타겟 지정
    /// </summary>
    /// <param name="_target">플레이어</param>
    public void SetBossTarget(Transform _target)
    {
        targetTransform = _target;
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
    /// 보스 몬스터가 공격 받았을때 피드백 이벤트 함수.
    /// </summary>
    public void SetBossAttack()
    {
        TransitionManager.getInstance.Play(TransitionManager.TransitionType.ChangeColor, 0.25f, Vector3.zero, spriteRenderer.gameObject, Color.red);
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
    /// 몬스터 타입 반환 함수.
    /// </summary>
    /// <returns>MonsterType</returns>
    public MonsterType GetMonsterType()
    {
        return type;
    }
    /// <summary>
    /// 몬스터 공격력 반환 함수.
    /// </summary>
    /// <returns></returns>
    public float GetAttackPower()
    {
        return attackPower;
    }
    /// <summary>
    /// 몬스터 공격 범위 반환 함수.
    /// </summary>
    /// <returns></returns>
    public float GetAttackRange()
    {
        return attackDistance;
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
            var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
            curAABB = new AABB(this.transform, size);
        }

        // 라인그리기
        var aabb = curAABB;
        var leftTop = new Vector3(aabb.GetLeft, aabb.GetTop, 0);
        var rightTop = new Vector3(aabb.GetRight, aabb.GetTop, 0);
        var leftBottom = new Vector3(aabb.GetLeft, aabb.GetBottom, 0);
        var rightBottom = new Vector3(aabb.GetRight, aabb.GetBottom, 0);

        Debug.DrawLine(leftTop, rightTop, Color.black);
        Debug.DrawLine(rightTop, rightBottom, Color.black);
        Debug.DrawLine(rightBottom, leftBottom, Color.black);
        Debug.DrawLine(leftBottom, leftTop, Color.black);

        return curAABB.CheckCollisionOBB(_other);
    }
    /// <summary>
    /// 보스 공격 패턴에 따른 공격 범위 컨트롤 함수.
    /// </summary>
    /// <param name="_Pattern">공격 패턴 Index</param>
    public async UniTask<bool> BossAttackRange(int _Pattern)
    {
        // TODO :: 여기서는 공격 범위 오브젝트만 컨트롤하고 공격 기능은 GameScene에서 관리할까?? 모든 공격이 GameScene에서 이루어지고 있음. return값으로 공격 가능인지 체크.
        BossMonsterAttackPattern pattern = (BossMonsterAttackPattern)_Pattern;
        if (BossAttackPattern.activeSelf)
        {
            switch (pattern)
            {
                case BossMonsterAttackPattern.BulletFire:
                    if ((targetTransform.position - transform.position).magnitude <= attackDistance)
                    {
                        var direction = targetTransform.position - transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        BossAttackPatterns[_Pattern].transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
                        BossAttackPatterns[_Pattern].SetActive(true);
                        await UniTask.Delay(1000);
                        BossAttackPatterns[_Pattern].SetActive(false);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case BossMonsterAttackPattern.BodyAttack:
                    break;
            }
        }
        return false;
    }
}
