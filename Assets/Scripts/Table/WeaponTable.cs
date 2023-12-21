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
    private Dictionary<int, WeaponInfo> weaponInfoDict;
    private TableManager tableManager;

    public const float MELEE_WEAPON_ENHANCE_POWER = 0.5f;
    public const float RANGED_WEAPON_ENHANCE_POWER = 0.2f;
    public const float RANGED_WEAPON_ENHANCE_SPEED = 0.2f;
    public async UniTask<bool> Initialize()
    {
        //weaponInfos = await TableLoader.getInstance.LoadTableJson<WeaponInfo[]>("WeaponInfo");

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
            weaponInfos[i] = weapon;
        }



        weaponInfoDict = new Dictionary<int, WeaponInfo>(count);

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

            if (weaponInfoDict.ContainsKey(weapon.weaponId) == false)
                weaponInfoDict.Add(weapon.weaponId, weapon);
        }
    }
    /// <summary>
    /// 인벤토리 데이터 셋팅.
    /// </summary>
    /// <param name="_dictionary">인벤토리Dictionary</param>
    public void SetInventoryData(Dictionary<int, InventoryItem> _dictionary)
    {
        inventoryItem = _dictionary;
    }
    /// <summary>
    /// 인벤토리Dictionary반환 함수.
    /// </summary>
    /// <returns>inventoryItem</returns>
    public Dictionary<int, InventoryItem> GetInventory()
    {
        return inventoryItem;
    }
    /// <summary>
    /// 인벤토리Dictionary.value 반환 함수.
    /// </summary>
    /// <param name="_key">key</param>
    /// <returns>inventoryItem[_key]</returns>
    public InventoryItem GetInventoryData(int _key)
    {
        return inventoryItem[_key];
    }
    /// <summary>
    /// 인벤토리DictionaryCount return.
    /// </summary>
    /// <returns>inventoryItem.Count</returns>
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
    /// <summary>
    /// 무기 객체 배열 반환 함수.
    /// </summary>
    /// <returns>weaponInfos</returns>
    public WeaponInfo[] GetWeaponInfos()
    {
        return weaponInfos;
    }
    /// <summary>
    /// WeaponInfoDictionary Value Retrun.
    /// </summary>
    /// <param name="_key">Key</param>
    /// <returns>weaponInfoDict[_key]</returns>
    public WeaponInfo GetWeaponInfo(int _key)
    {
        return weaponInfoDict[_key];
    }
}
