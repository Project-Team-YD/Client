using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private WeaponController playerWeaponController = null;

    private List<WeaponInfo> playerWeapons = new List<WeaponInfo>();

    public WeaponController SetPlayerWeaponController { get { return playerWeaponController; } set { playerWeaponController = value; } }

    public List<WeaponInfo> SetPlayerWeapons { get { return playerWeapons; } set { playerWeapons = value; } }


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
}
