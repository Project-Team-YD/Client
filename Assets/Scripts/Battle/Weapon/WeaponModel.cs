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
    private CancellationToken getTargetEnemyCancel = new CancellationToken();

    private int weaponId;
    private string weaponName;
    private float attackPower;
    private float attackSpeed;
    private float attackRange;

    private float coolTime;
    private bool isRightWeapon;
    
    public void InitWeapon(WeaponInfo _info, GameSceneController _controller, bool _isRight)
    {
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
    }

    public void SetTarget(Transform _targetTransform)
    {
        playerTransform = _targetTransform;
    }

    public WeaponType GetWeaponType()
    {
        return type;
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    public float GetAttackCoolTime()
    {
        return attackSpeed;
    }

    // TODO :: 여기서 AABB 체크도 같이해서 데미지를 주는게 맞을듯한데 의견 여쭤보기..임시 Controller Update문 함수 
    // TODO :: 원거리 무기는 Bullet같은 스크립트를 하나 파서 거기서 컨트롤해줘야..........생각해보기
    public async void WeaponAttack(WeaponType _type, Transform _transform)
    {
        if (transform != null)
        {
            switch (_type)
            {
                case WeaponType.dagger:
                    while (true)
                    {
                        coolTime = 360 / attackSpeed;
                        transform.RotateAround(playerTransform.position, Vector3.forward, coolTime * Time.deltaTime);
                        await UniTask.Yield();
                    }
                //break;
                case WeaponType.sword:
                    while (true)
                    {
                        coolTime = 360 / attackSpeed;
                        transform.RotateAround(playerTransform.position, Vector3.forward, coolTime * Time.deltaTime);
                        await UniTask.Yield();
                    }
                //break;
                case WeaponType.gun:
                    while (true)
                    {
                        if (enemy == null)
                            enemy = gameSceneController.GetTargetEnemy(attackRange);
                        if (enemy != null)
                        {
                            await UniTask.Delay((int)attackSpeed * 1000, cancellationToken: getTargetEnemyCancel);
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
                            gameSceneController.FireBullet(enemy, type, _transform);
                            enemy = null;
                        }
                        await UniTask.Yield();
                    }
                //break;
                case WeaponType.ninjastar:
                    while (true)
                    {
                        if (enemy == null)
                            enemy = gameSceneController.GetTargetEnemy(attackRange);
                        if (enemy != null)
                        {
                            await UniTask.Delay((int)attackSpeed * 1000, cancellationToken: getTargetEnemyCancel);
                            gameSceneController.FireBullet(enemy, type, _transform);
                            enemy = null;
                        }
                        await UniTask.Yield();
                    }
                //break;
                default:
                    break;
            }
        }
    }
}
