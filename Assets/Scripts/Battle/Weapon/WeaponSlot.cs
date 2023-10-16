using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel
{
    private Transform playerTransform;
    private WeaponType type;
    private int weaponId;
    private float attackPower;
    private float attackSpeed;
    private float attackRange;
    
    public void InitWeapon(WeaponInfo _info)
    {
        weaponId = _info.weaponId;
        attackPower = _info.attackPower;
        attackSpeed = _info.attackSpeed;
        attackRange = _info.attackRange;
    }

    public void SetTarget(Transform _targetTransform)
    {
        playerTransform = _targetTransform;
    }

    public WeaponType GetWeaponType()
    {
        return type;
    }

    public void WeaponAttack(WeaponType _type)
    {
        switch (_type)
        {
            case WeaponType.dagger:
                break;
            case WeaponType.sword:
                break;
            case WeaponType.gun:
                break;
            case WeaponType.ninjastar:
                break;
            default:
                break;
        }
    }
}
