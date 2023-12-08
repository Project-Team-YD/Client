using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private float currentGold;

    private float maxHp = 0;
    private float regenHp = 0;
    private float shortDamage = 0;
    private float longDamage = 0;

    private WeaponController playerWeaponController = null;

    private List<WeaponInfo> playerWeapons = new List<WeaponInfo>();

    private List<PassiveItemInfo> playerPassiveItem = new List<PassiveItemInfo>();

    public WeaponController SetPlayerWeaponController { get { return playerWeaponController; } set { playerWeaponController = value; } }

    public List<WeaponInfo> SetPlayerWeapons { get { return playerWeapons; } set { playerWeapons = value; } }

    public List<PassiveItemInfo> SetPlayerPassiveItem { get { return playerPassiveItem; } set { playerPassiveItem = value; } }

    public float SetCurrentGold { get { return currentGold; } set { currentGold = value; } }

    public float GetPlayerMaxHP { get { return maxHp; } }
    public float GetPlayerRegenHp { get { return regenHp; } }
    public float GetPlayerShortDamage { get { return shortDamage; } }
    public float GetPlayerLongDamage { get { return longDamage; } }

    /// <summary>
    /// 이미 무기 최대 무기를 가지고있을 때 대비하기
    /// </summary>
    /// <param name="_weapon"></param>
    public void AddPlayerWeapon(WeaponInfo _weapon)
    {
        var weapon = EnhanceWeapon(_weapon.weaponId);
        if (!weapon)
        {
            playerWeapons.Add(_weapon);
        }
    }

    private bool EnhanceWeapon(int _id)
    {
        int count = playerWeapons.Count;
        for (int i = 0; i < count; i++)
        {
            if (playerWeapons[i].weaponId == _id)
            {
                var weapons = playerWeapons[i];
                weapons.enhance += 1;
                playerWeapons[i] = weapons;
                return true;
            }
        }

        return false;
    }

    public void ClearPlayerWeapon()
    {
        playerWeapons.Clear();
    }

    public void AddPlayerPassiveItem(int _key)
    {
        var isPossess = EnhancePassiveItem(_key);

        if (isPossess == false)
        {
            // 테이블에서 데이터 가져와서 넣어주기
            PassiveItemInfo item = new PassiveItemInfo();
            item.passiveItemId = _key;

            playerPassiveItem.Add(item);
        }

        PassiveItemApply();
    }

    private bool EnhancePassiveItem(int _id)
    {
        int count = playerPassiveItem.Count;
        for (int i = 0; i < count; i++)
        {
            if (playerPassiveItem[i].passiveItemId == _id)
            {
                var item = playerPassiveItem[i];
                item.enhance += 1;
                playerPassiveItem[i] = item;
                return true;
            }
        }

        return false;
    }

    private void PassiveItemApply()
    {
        // 초기화
        maxHp = 0;
        regenHp = 0;
        shortDamage = 0;
        longDamage = 0;

        int count = playerPassiveItem.Count;
        for (int i = 0; i < count; i++)
        {
            maxHp += playerPassiveItem[i].maxHp;
            regenHp += playerPassiveItem[i].regenHp;
            shortDamage += playerPassiveItem[i].shortDamage;
            longDamage += playerPassiveItem[i].longDamage;
        }
    }
}
