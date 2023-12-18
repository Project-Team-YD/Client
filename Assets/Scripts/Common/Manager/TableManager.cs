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

    public string[] descriptions = { "기본적인 단검입니다. 공격속도가 빠르지만 조금 약합니다.",
                                      "기본적인 장검입니다. 한방이 묵직하지만 공격속도는 조금 느립니다!!",
                                      "기본적인 총입니다. 최강의 한방, 공격 속도가 매우 느린편.",
                                      "기본적인 표창입니다. 공격속도와 데미지가 균형잡혀 있습니다.",
                                      "먹으면 체력이 불끈!!맛있어 보이는 힘쌘 한우 스테이크 입니다.",
                                      "재생력이 올라갈 것 같은 악마의 피입니다.",
                                      "발걸음이 가벼워 질 것 같은 강화 초록 물약입니다.",
                                      "엄청난 공격력을 가졌던 드래곤의 진주입니다.",
                                      "공격을 좀 더 빠르게 할 용기가 생기는 용기의 물약입니다." };

    public string[] passiveDescriptions = { "최대 HP 10% 증가",
                                            "5초마다 체력 10회복",
                                            "이동속도 10% 증가",
                                            "공격력 10% 증가",
                                            "공격속도 10% 증가" };

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

    public void SetShopItemRefresh(int _key)
    {
        if (shopTable.ContainsKey(_key))
        {
            shopTable[_key].isBuy = true;
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("해당하는 상점 아이템 키값이 없습니다.");
#endif
        }
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
