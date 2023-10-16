using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    private SpriteRenderer spriteRenderer = null;
    private Transform enemyTransform = null;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetBulletSprite(WeaponType _type, Transform _transform)
    {
        enemyTransform = _transform;
        if (_type == WeaponType.gun)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>($"Weapon/bullet");
            var direction = enemyTransform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle-135, Vector3.forward);
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>($"Weapon/{_type}");
        }
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
