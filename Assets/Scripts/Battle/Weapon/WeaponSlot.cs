using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class WeaponSlot : MonoBehaviour
{
    private Transform playerTransform = null;
    private SpriteRenderer weaponSprite = null;
    private GameSceneController gameSceneController = null;
    private EnemyObject enemy = null;
    private WeaponType type;
    private WeaponInfo info;

    private PlayerManager playerManager = null;

    private int weaponId;
    private string weaponName;
    private float attackPower;
    private float attackSpeed;
    private float attackRange;

    private float coolTime;
    private bool isRightWeapon;

    private AABB curAABB;

    private CancellationTokenSource cancellationTokenSource;

    private bool isBossAttack = false;

    public int GetWeaponID { get { return weaponId; } }

    /// <summary>
    /// 근접무기용 AABB
    /// </summary>
    public AABB GetWeaponAABB
    {
        get
        {
            if (curAABB == null)
            {
                var size = weaponSprite.sprite.rect.size / weaponSprite.sprite.pixelsPerUnit;
                curAABB = new AABB(this.transform, size);
            }

            return curAABB;
        }
    }
    /// <summary>
    /// 무기 객체 초기화 함수.
    /// </summary>
    /// <param name="_info">무기 객체정보</param>
    /// <param name="_controller"></param>
    /// <param name="_isRight">캐릭터 기준 오른쪽 슬릇인지의 대한 값</param>
    public void InitWeapon(WeaponInfo _info, GameSceneController _controller, bool _isRight)
    {
        info = _info;
        weaponId = _info.weaponId;
        weaponName = _info.weaponName;
        attackPower = _info.attackPower;
        attackSpeed = _info.attackSpeed;
        attackRange = _info.attackRange;

        weaponSprite = GetComponent<SpriteRenderer>();
        type = (WeaponType)weaponId;
        weaponSprite.sprite = Resources.Load<Sprite>($"Weapon/{type}");
        gameSceneController = _controller;
        isRightWeapon = _isRight;

        playerManager = PlayerManager.getInstance;
    }
    /// <summary>
    /// 플레이어 Transform 저장함수.
    /// </summary>
    /// <param name="_targetTransform">PlayerTransform</param>
    public void SetTarget(Transform _targetTransform)
    {
        playerTransform = _targetTransform;
    }
    /// <summary>
    /// 해당 슬릇의 무기 타입 반환 함수.
    /// </summary>
    /// <returns></returns>
    public WeaponType GetWeaponType()
    {
        return type;
    }
    /// <summary>
    /// 해당 슬릇의 무기 객체 정보 반환 함수.
    /// </summary>
    /// <returns></returns>
    public WeaponInfo GetWeaponInfo()
    {
        return info;
    }

    /// <summary>
    /// 무기 타입별 공격 로직 함수.
    /// while 조건 변경 필요할듯 다음 웨이브 때 공격하려면
    /// </summary>
    public async void WeaponAttack()
    {
        if (transform != null)
        {
            cancellationTokenSource = new CancellationTokenSource();

            switch (type)
            {
                case WeaponType.dagger:
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        if (playerTransform != null)
                        {
                            var attackSpeeds = attackSpeed + (attackSpeed * playerManager.GetPlayerAttackSpeed);
                            coolTime = 360 / attackSpeeds;
                            transform.RotateAround(playerTransform.position, Vector3.forward, coolTime * Time.deltaTime);

#if UNITY_EDITOR
                            var aabb = GetWeaponAABB;
                            var leftTop = new Vector3(aabb.GetLeft, aabb.GetTop, 0);
                            var rightTop = new Vector3(aabb.GetRight, aabb.GetTop, 0);
                            var leftBottom = new Vector3(aabb.GetLeft, aabb.GetBottom, 0);
                            var rightBottom = new Vector3(aabb.GetRight, aabb.GetBottom, 0);

                            Vector3 leftTop2 = aabb.RotatePoint(leftTop);
                            Vector3 rightTop2 = aabb.RotatePoint(rightTop);
                            Vector3 leftBottom2 = aabb.RotatePoint(leftBottom);
                            Vector3 rightBottom2 = aabb.RotatePoint(rightBottom);

                            Debug.DrawLine(leftTop2, rightTop2, Color.blue);
                            Debug.DrawLine(rightTop2, rightBottom2, Color.blue);
                            Debug.DrawLine(rightBottom2, leftBottom2, Color.blue);
                            Debug.DrawLine(leftBottom2, leftTop2, Color.blue);
#endif

                            // TODO:: 가만히있을땐 문제 없음 이동할때 문제 있음 확인중
                            if (isBossAttack == false)
                            {
                                isBossAttack = await gameSceneController.CheckMonsterAttack(this);

                                if (isBossAttack)
                                    BossAttackDelay().Forget();
                            }
                        }
                        await UniTask.Yield(cancellationTokenSource.Token);
                    }
                    break;
                case WeaponType.sword:
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        if (playerTransform != null)
                        {
                            var attackSpeeds = attackSpeed + (attackSpeed * playerManager.GetPlayerAttackSpeed);
                            coolTime = 360 / attackSpeeds;
                            transform.RotateAround(playerTransform.position, Vector3.forward, coolTime * Time.deltaTime);

#if UNITY_EDITOR
                            var aabb = GetWeaponAABB;
                            var leftTop = new Vector3(aabb.GetLeft, aabb.GetTop, 0);
                            var rightTop = new Vector3(aabb.GetRight, aabb.GetTop, 0);
                            var leftBottom = new Vector3(aabb.GetLeft, aabb.GetBottom, 0);
                            var rightBottom = new Vector3(aabb.GetRight, aabb.GetBottom, 0);

                            Vector3 leftTop2 = aabb.RotatePoint(leftTop);
                            Vector3 rightTop2 = aabb.RotatePoint(rightTop);
                            Vector3 leftBottom2 = aabb.RotatePoint(leftBottom);
                            Vector3 rightBottom2 = aabb.RotatePoint(rightBottom);

                            Debug.DrawLine(leftTop2, rightTop2, Color.blue);
                            Debug.DrawLine(rightTop2, rightBottom2, Color.blue);
                            Debug.DrawLine(rightBottom2, leftBottom2, Color.blue);
                            Debug.DrawLine(leftBottom2, leftTop2, Color.blue);
#endif

                            // TODO:: 가만히있을땐 문제 없음 이동할때 문제 있음 확인중
                            if (isBossAttack == false)
                            {
                                isBossAttack = await gameSceneController.CheckMonsterAttack(this);

                                if (isBossAttack)
                                    BossAttackDelay().Forget();
                            }
                        }
                        await UniTask.Yield(cancellationTokenSource.Token);
                    }
                    break;
                case WeaponType.gun:
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        if (enemy == null)
                            enemy = gameSceneController.GetTargetEnemy(attackRange);
                        if (enemy != null)
                        {
                            if (isRightWeapon)
                            {
                                var direction = enemy.transform.position - transform.position;
                                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                            }
                            else
                            {
                                var direction = enemy.transform.position - transform.position;
                                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                                transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
                            }
                            gameSceneController.FireBullet(enemy, this).Forget();

                            var attackSpeeds = (attackSpeed - ((info.enhance * WeaponTable.RANGED_WEAPON_ENHANCE_SPEED) * attackSpeed));
                            attackSpeeds = (attackSpeeds - (attackSpeeds * playerManager.GetPlayerAttackSpeed)) * 1000;

                            await UniTask.Delay((int)attackSpeeds, cancellationToken: cancellationTokenSource.Token);

                            enemy = null;
                        }

                        await UniTask.Yield(cancellationTokenSource.Token);
                    }
                    break;
                case WeaponType.ninjastar:
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        if (enemy == null)
                            enemy = gameSceneController.GetTargetEnemy(attackRange);
                        if (enemy != null)
                        {
                            gameSceneController.FireBullet(enemy, this).Forget();

                            var attackSpeeds = (attackSpeed - ((info.enhance * WeaponTable.RANGED_WEAPON_ENHANCE_SPEED) * attackSpeed));
                            attackSpeeds = (attackSpeeds - (attackSpeeds * playerManager.GetPlayerAttackSpeed)) * 1000;

                            await UniTask.Delay((int)attackSpeeds, cancellationToken: cancellationTokenSource.Token);

                            enemy = null;
                        }

                        await UniTask.Yield(cancellationTokenSource.Token);
                    }
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 보스 몬스터 공격 딜레이 함수.
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid BossAttackDelay()
    {
        await UniTask.Delay(1000);
        isBossAttack = false;
    }

    /// <summary>
    /// WeaponAttackCancelToken.Cancel()
    /// </summary>
    public void StopAttack()
    {
        cancellationTokenSource?.Cancel();
    }
}
