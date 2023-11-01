using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private WeaponController playerWeapon = null;

    public WeaponController SetPlayerWeapon { get { return playerWeapon; } set { playerWeapon = value; } }
}
