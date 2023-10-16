using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Generics;
using HSMLibrary.Tables;
using Cysharp.Threading.Tasks;

public class WeaponTable : Singleton<WeaponTable>, ITable
{
    private WeaponInfo[] weaponInfos = null;

    public async UniTask<bool> Initialize()
    {
        weaponInfos = await TableLoader.getInstance.LoadTableJson<WeaponInfo[]>("WeaponInfo");

        return true;
    }

    public WeaponInfo GetWeaponInfoByIndex(int _index)
    {
        if (_index >= weaponInfos.Length)
            _index = weaponInfos.Length - 1;
        return weaponInfos[_index];
    }
}
