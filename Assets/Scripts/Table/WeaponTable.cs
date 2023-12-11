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
        int count = tableManager.GetWeaponDataCount();
        weaponInfos = new WeaponInfo[count];
        for (int i = 0; i < count; i++)
        {
            WeaponInfo weapon = new WeaponInfo
            {
                weaponId = tableManager.GetItemInfo(i).id,
                weaponName = tableManager.GetItemInfo(i).itemName,
                attackPower = tableManager.GetWeaponItem(i).damage,
                attackRange = tableManager.GetWeaponItem(i).range,
                attackSpeed = tableManager.GetWeaponItem(i).speed,
                enhance = 0
            };
            //if (inventoryItem.Count > i)
            //{
            //    weapon.enhance = inventoryItem[i + 1].enchant;
            //}
            //else
            //{
            //    weapon.enhance = 0;
            //}
            weaponInfos[i] = weapon;
        }
    }    

    public void SetInventoryData(Dictionary<int, InventoryItem> _dictionary)
    {
        inventoryItem = _dictionary;
    }

    public InventoryItem GetInventoryData(int _key)
    {
        return inventoryItem[_key];
    }

    public int GetInventoryCount()
    {
        return inventoryItem.Count;
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
