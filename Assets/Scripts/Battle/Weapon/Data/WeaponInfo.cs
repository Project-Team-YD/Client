using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    dagger = 0,
    sword,
    gun,
    ninjastar
}

public struct WeaponInfo
{    
    public int weaponId;
    public string weaponName;
    public float attackPower;
    public float attackSpeed;
    public float attackRange;
    public int enhance;
}
