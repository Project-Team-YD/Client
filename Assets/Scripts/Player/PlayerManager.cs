using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private WeaponController playerWeaponController = null;

    private WeaponInfo startWeapon;

    private List<WeaponInfo> playerWeapons = new List<WeaponInfo>();

    public WeaponInfo SetStartWeapon { get { return startWeapon; } set { startWeapon = value; } }

    public WeaponController SetPlayerWeaponController { get { return playerWeaponController; } set { playerWeaponController = value; } }

    public List<WeaponInfo> SetPlayerWeapons { get { return playerWeapons; } set { playerWeapons = value; } }


    public void AddPlayerWeapon(WeaponInfo _weapon)
    {
        int count = playerWeapons.Count;
        for (int i = 0; i < count; i++)
        {
            // 강화
            if (playerWeapons[i].weaponId == _weapon.weaponId)
            {

            }
            // 추가
            else
            {
                playerWeapons.Add(_weapon);
            }
        }
    }
}
