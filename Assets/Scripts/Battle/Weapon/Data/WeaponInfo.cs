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
    public int weaponId; // Item
    public string weaponName; // Item
    public float attackPower; // WeaponItem
    public float attackSpeed; // WeaponItem
    public float attackRange; // WeaponItem
    public int enhance; // InventoryItem
}
