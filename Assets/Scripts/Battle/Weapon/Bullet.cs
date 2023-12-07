using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    private SpriteRenderer spriteRenderer = null;
    private Transform enemyTransform = null;
    private Transform playerTransform = null;
    private AABB curAABB;
    private WeaponType weaponType;
    public AABB GetBulletAABB
    {
        get
        {
            if (curAABB == null)
            {
                var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
                curAABB = new AABB(this.transform, size);
            }

            return curAABB;
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
        curAABB = new AABB(this.transform, size);
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
        curAABB = new AABB(this.transform, size);
    }

    public void SetBossMonsterBulletSprite(Vector3 _direction, int _rotationAngle)
    {        
        spriteRenderer.sprite = Resources.Load<Sprite>($"Weapon/Boss_bullet");        
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90 + (_rotationAngle * 45), Vector3.forward);

        var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
        curAABB = new AABB(this.transform, size);
    }

    public void SetBossMonsterBodyAttackSprite(Transform _transform)
    {
        playerTransform = _transform;
        spriteRenderer.sprite = Resources.Load<Sprite>($"Monster/Boss/Boss");
        var direction = playerTransform.position - transform.position;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
        curAABB = new AABB(this.transform, size);
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
            var size = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
            curAABB = new AABB(this.transform, size);
        }

        // 라인그리기
        var aabb = GetBulletAABB;
        var leftTop = new Vector3(aabb.GetLeft, aabb.GetTop, 0);
        var rightTop = new Vector3(aabb.GetRight, aabb.GetTop, 0);
        var leftBottom = new Vector3(aabb.GetLeft, aabb.GetBottom, 0);
        var rightBottom = new Vector3(aabb.GetRight, aabb.GetBottom, 0);

        // 45도로 회전한 AABB의 네 꼭짓점 계산
        Vector3 leftTop2 = aabb.RotatePoint(leftTop);
        Vector3 rightTop2 = aabb.RotatePoint(rightTop);
        Vector3 leftBottom2 = aabb.RotatePoint(leftBottom);
        Vector3 rightBottom2 = aabb.RotatePoint(rightBottom);

        // 회전된 AABB를 라인으로 그리기
        Debug.DrawLine(leftTop2, rightTop2, Color.blue);
        Debug.DrawLine(rightTop2, rightBottom2, Color.blue);
        Debug.DrawLine(rightBottom2, leftBottom2, Color.blue);
        Debug.DrawLine(leftBottom2, leftTop2, Color.blue);

        return curAABB.CheckCollisionOBB(_other);
    }
}
