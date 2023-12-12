using Cysharp.Threading.Tasks;
using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Packet;

public class TableManager : Singleton<TableManager>
{
    private Dictionary<int, Item> itemTable;
    private Dictionary<int, ItemWeapon> itemWeaponTable; // 1 2 3 4
    private Dictionary<int, ItemEffect> itemEffectTable; // 5 6 7 8 
    private Dictionary<int, ShopItem> shopTable;
    private Dictionary<int, WeaponEnchant> weaponEnchantTable;    

    public void SetTableItem(Dictionary<int, Item> _dictionary)
    {
        itemTable = _dictionary;
    }

    public Item GetItemInfo(int _key)
    {
        if (itemTable.ContainsKey(_key))
            return itemTable[_key];
        else
            return null;
    }

    public int GetWeaponDataCount()
    {
        return itemWeaponTable.Count;
    }

    public void SetTableWeapon(Dictionary<int, ItemWeapon> _dictionary)
    {
        itemWeaponTable = _dictionary;
    }

    public ItemWeapon GetWeaponItem(int _key)
    {
        if (itemWeaponTable.ContainsKey(_key))
            return itemWeaponTable[_key];
        else
            return null;
    }

    public void SetTableEffect(Dictionary<int, ItemEffect> _dictionary)
    {
        itemEffectTable = _dictionary;
    }

    public ItemEffect GetEffectItem(int _key)
    {
        if (itemEffectTable.ContainsKey(_key))
            return itemEffectTable[_key];
        else
            return null;
    }

    public void SetTableShop(Dictionary<int, ShopItem> _dictionary)
    {
        shopTable = _dictionary;
    }

    public ShopItem GetShopItem(int _key)
    {
        if (shopTable.ContainsKey(_key))
            return shopTable[_key];
        else
            return null;
    }

    public int GetShopDataCount()
    {
        return shopTable.Count;
    }

    public void SetTableEnhant(Dictionary<int, WeaponEnchant> _dictionary)
    {
        weaponEnchantTable = _dictionary;
    }

    public WeaponEnchant GetWeaponEnchantInfo(int _key)
    {
        if (weaponEnchantTable.ContainsKey(_key))
            return weaponEnchantTable[_key];
        else
            return null;
    }

    public int GetEnchantInfoCount()
    {
        return weaponEnchantTable.Count;
    }
}
