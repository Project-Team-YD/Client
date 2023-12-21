using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    private SpriteRenderer spriteRenderer = null;
    private Transform enemyTransform = null;
    private Transform playerTransform = null;
    private OBB curOBB;
    private WeaponType weaponType;
    public OBB GetBulletOBB
    {
        get
        {
            if (curOBB == null)
            {
                var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
                curOBB = new OBB(this.transform, size);
            }

            return curOBB;
        }
    }
    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// 무기타입의 따른 발사체 이미지 지정 함수.
    /// </summary>
    /// <param name="_type">무기타입</param>
    /// <param name="_transform">타겟 몬스터 Transform</param>
    public void SetBulletSprite(WeaponType _type, Transform _transform)
    {
        enemyTransform = _transform;
        weaponType = _type;
        var direction = enemyTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (weaponType == WeaponType.gun)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>($"Weapon/bullet");
            transform.rotation = Quaternion.AngleAxis(angle - 135, Vector3.forward);
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>($"Weapon/{weaponType}");
            transform.rotation = Quaternion.AngleAxis(angle - 180, Vector3.forward);
        }

        var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
        curOBB = new OBB(this.transform, size);
    }
    /// <summary>
    /// 현재 발사체의 무기의 타입을 반환.
    /// </summary>
    /// <returns></returns>
    public WeaponType GetWeaponType()
    {
        return weaponType;
    }
    /// <summary>
    /// 원거리 몬스터 총알 Sprite 지정 함수.
    /// </summary>
    /// <param name="_transform">플레이어 Transform</param>
    public void SetMonsterBulletSprite(Transform _transform)
    {
        playerTransform = _transform;
        spriteRenderer.sprite = Resources.Load<Sprite>($"Weapon/Long_bullet");
        var direction = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 45, Vector3.forward);

        var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
        curOBB = new OBB(this.transform, size);
    }
    /// <summary>
    /// 보스 몬스터 총알 Sprite지정 함수.
    /// </summary>
    /// <param name="_direction">Vector3 방향</param>
    /// <param name="_rotationAngle">int 총알 회전값.</param>
    public void SetBossMonsterBulletSprite(Vector3 _direction, int _rotationAngle)
    {
        spriteRenderer.sprite = Resources.Load<Sprite>($"Weapon/Boss_bullet");
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90 + (_rotationAngle * 45), Vector3.forward);

        var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
        curOBB = new OBB(this.transform, size);
    }
    /// <summary>
    /// 보스 몬스터 몸체 공격 Sprite지정 함수.
    /// </summary>
    public void SetBossMonsterBodyAttackSprite()
    {
        spriteRenderer.sprite = Resources.Load<Sprite>($"Monster/Boss/Boss");
        transform.rotation = Quaternion.Euler(0, 0, 0);

        var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
        curOBB = new OBB(this.transform, size);
    }

    public void OnActivate()
    {
        gameObject.SetActive(true);
    }

    public void OnDeactivate()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Bullet 객체 충돌 체크 함수.
    /// </summary>
    /// <param name="_other">OBB</param>
    /// <returns></returns>
    public bool OnCheckCollision(OBB _other)
    {
        if (curOBB == null)
        {
            var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
            curOBB = new OBB(this.transform, size);
        }

        return curOBB.CheckCollision(_other);
    }
}
