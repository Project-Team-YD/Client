using HSMLibrary.Generics;
using Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private const float ONE_HUNDREDTH = 0.01f;

    private string userName = null;

    private float currentMoney;
    private float currentGold;

    private float maxHp = 0;
    private float regenHp = 0;
    private float speed = 0;
    private float damage = 0;
    private float attackSpeed = 0;

    private TableManager tableManager = TableManager.getInstance;
    private WeaponTable weaponTable = WeaponTable.getInstance;
    private WeaponController playerWeaponController = null;
    private Weapon[] playerWeapons;
    private Effect[] playerPassiveItem;

    public string UserName { get { return userName; } set { userName = value; } }

    public WeaponController PlayerWeaponController { get { return playerWeaponController; } set { playerWeaponController = value; } }

    public Weapon[] PlayerWeapons { get { return playerWeapons; } set { playerWeapons = value; } }

    public Effect[] PlayerPassiveItem { get { return playerPassiveItem; } set { playerPassiveItem = value; } }

    public float CurrentMoney { get { return currentMoney; } set { currentMoney = value; } }
    public float CurrentGold { get { return currentGold; } set { currentGold = value; } }

    public float GetPlayerMaxHP { get { return maxHp * ONE_HUNDREDTH; } }
    public float GetPlayerRegenHp { get { return regenHp; } }
    public float GetPlayerSpeed { get { return speed * ONE_HUNDREDTH; } }
    public float GetPlayerDamage { get { return damage * ONE_HUNDREDTH; } }
    public float GetPlayerAttackSpeed { get { return attackSpeed * ONE_HUNDREDTH; } }

    private void PassiveItemApply()
    {
        // 초기화
        maxHp = 0;
        regenHp = 0;
        damage = 0;
        speed = 0;
        attackSpeed = 0;

        int count = playerPassiveItem.Length;
        for (int i = 0; i < count; i++)
        {
            var data = playerPassiveItem[i];
            var item = tableManager.GetEffectItem(data.id);
            maxHp += item.maxHp * data.count;
            regenHp += item.regenHp * data.count;
            damage += item.damage * data.count;
            speed += item.speed * data.count;
            attackSpeed += item.attackSpeed * data.count;
        }
    }

    public void UpdatePlayerWeapon(Weapon[] _weapon, Effect[] _passive)
    {
        playerWeapons = _weapon;

        playerPassiveItem = _passive;
        if (playerPassiveItem != null)
            PassiveItemApply();


        if (playerPassiveItem != null)
        {
            Debug.Log($"시작");
            for (int i = 0; i < _weapon.Length; i++)
            {
                Debug.Log($"Slot {i} / Id: {_weapon[i].id} / Enchant : {_weapon[i].enchant}");
            }
            Debug.Log("-----------------------");
            for (int i = 0; i < _passive.Length; i++)
            {
                Debug.Log($"Effect {i} / Id: {_passive[i].id} / Count : {_passive[i].count}");
            }
            Debug.Log($"끝");
        }
    }
}
