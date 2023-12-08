using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Generics;
using HSMLibrary.Tables;
using Cysharp.Threading.Tasks;
using Packet;

public class WeaponTable : Singleton<WeaponTable>, ITable
{
    private WeaponInfo[] weaponInfos = null;
    private Dictionary<int, InventoryItem> inventoryItem;
    private TableManager tableManager;

    public async UniTask<bool> Initialize()
    {
        weaponInfos = await TableLoader.getInstance.LoadTableJson<WeaponInfo[]>("WeaponInfo");

        return true;
    }

    public void InitWeaponInfo()
    {
        tableManager = TableManager.getInstance;
        int count = tableManager.GetWeaponData();
        for (int i = 0; i < count; i++)
        {
            WeaponInfo weapon = new WeaponInfo
            {
                weaponId = tableManager.GetItemInfo(i + 1).id,
                weaponName = tableManager.GetItemInfo(i + 1).itemName,
                attackPower = tableManager.GetWeaponItem(i + 1).damage,
                attackRange = tableManager.GetWeaponItem(i + 1).range,
                attackSpeed = tableManager.GetWeaponItem(i + 1).speed,                
            };
            if (inventoryItem.Count <= i)
            {
                weapon.enhance = inventoryItem[i + 1].enchant;
            }
            else
            {
                weapon.enhance = 0;
            }
            weaponInfos[i] = weapon;
        }
    }    

    public void SetinventoryData(Dictionary<int, InventoryItem> _dictionary)
    {
        inventoryItem = _dictionary;
    }

    public WeaponInfo GetWeaponInfoByIndex(int _index)
    {
        if (_index >= weaponInfos.Length)
            _index = weaponInfos.Length - 1;
        return weaponInfos[_index];
    }

    public WeaponInfo[] GetWeaponInfos()
    {
        return weaponInfos;
    }
}
