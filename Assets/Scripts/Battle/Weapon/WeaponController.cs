using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private const int START_WEAPON_NUM = 0;
    [SerializeField] private WeaponSlot[] slot;
    [SerializeField] private GameSceneController gameSceneController;

    private PlayerManager playerManager = null;
    private WeaponTable weaponTable = null;

    private readonly float rotate = 45f;
    private Transform playerTransform = null;
    private bool isRight = false;

    private Vector3[] weaponPos;
    private Vector3[] weaponLocalPos;

    public WeaponSlot[] GetWeapons { get { return slot; } }

    private void Awake()
    {
        playerManager = PlayerManager.getInstance;
        weaponTable = WeaponTable.getInstance;
        playerTransform = gameObject.transform.parent.transform;

        var startWeapon = playerManager.PlayerWeapons[START_WEAPON_NUM];
        isRight = false;

        var data = weaponTable.GetWeaponInfo(startWeapon.id);
        data.enhance = startWeapon.enchant;

        slot[START_WEAPON_NUM].InitWeapon(data, gameSceneController, isRight);
        slot[START_WEAPON_NUM].SetTarget(playerTransform);
        var type = slot[START_WEAPON_NUM].GetWeaponType();
        if (type == WeaponType.dagger || type == WeaponType.sword)
        {
            slot[START_WEAPON_NUM].transform.position += new Vector3(-0.6f, 0f, 0f);
            slot[START_WEAPON_NUM].transform.eulerAngles = new Vector3(0f, 0f, rotate);
        }

        weaponPos = new Vector3[slot.Length];
        weaponLocalPos = new Vector3[slot.Length];
        int count = weaponPos.Length;
        for (int i = 0; i < count; i++)
        {
            var pos = slot[i].transform.position;
            weaponPos[i] = pos;
            var localPos = slot[i].transform.localPosition;
            weaponLocalPos[i] = localPos;
        }

        playerManager.PlayerWeaponController = this;
    }
    /// <summary>
    /// WeaponSlot AttackStart
    /// </summary>
    public void StartAttack()
    {
        int weaponCount = playerManager.PlayerWeapons.Length;
        for (int i = 0; i < weaponCount; i++)
        {
            slot[i].WeaponAttack();
        }
    }
    /// <summary>
    /// WeaponSlot AttackStop
    /// </summary>
    public void StopAttack()
    {
        int slotCount = slot.Length;
        for (int i = 0; i < slotCount; i++)
        {
            slot[i].StopAttack();
        }
    }
    /// <summary>
    /// WeaponSlot 셋팅 및 추가시 Update함수.
    /// </summary>
    public void UpdateWeapon()
    {
        int weaponCount = playerManager.PlayerWeapons.Length;
        for (int i = 0; i < weaponCount; i++)
        {
            var weapons = playerManager.PlayerWeapons[i];
            isRight = i == 0 ? false : true;

            var data = weaponTable.GetWeaponInfo(weapons.id);
            data.enhance = weapons.enchant;

            slot[i].InitWeapon(data, gameSceneController, isRight);
            slot[i].SetTarget(playerTransform);
            var type = slot[i].GetWeaponType();

            // 근접무기의 경우 위치 틀어지는거 생각해보기
            slot[i].transform.position = weaponPos[i];
            slot[i].transform.localPosition = weaponLocalPos[i];

            if (type == WeaponType.dagger || type == WeaponType.sword)
            {
                if (i == 0)
                {                    
                    slot[i].transform.eulerAngles = new Vector3(0f, 0f, rotate);
                }
                else
                {
                    slot[i].transform.position += new Vector3(0.6f, 0f, 0f);
                    slot[i].transform.eulerAngles = new Vector3(0f, 0f, -rotate);
                }
            }
        }
    }
}
