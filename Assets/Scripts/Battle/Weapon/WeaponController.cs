using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private const int START_WEAPON_NUM = 0;
    [SerializeField] private WeaponSlot[] slot;
    [SerializeField] private GameSceneController gameSceneController;

    private PlayerManager playerManager = null;

    private readonly float rotate = 45f;
    private Transform playerTransform = null;
    private bool isRight = false;

    private Vector3[] weaponPos;
    private Vector3[] weaponLocalPos;

    public WeaponSlot[] GetWeapons { get { return slot; } }

    private void Awake()
    {
        playerManager = PlayerManager.getInstance;

        playerTransform = gameObject.transform.parent.transform;

        SetStartWeapon();

        var startWeapon = playerManager.SetPlayerWeapons[START_WEAPON_NUM];
        WeaponInfo info = WeaponTable.getInstance.GetWeaponInfoByIndex(startWeapon.weaponId);
        isRight = false;
        slot[START_WEAPON_NUM].InitWeapon(info, gameSceneController, isRight);
        slot[START_WEAPON_NUM].SetTarget(playerTransform);
        var type = slot[START_WEAPON_NUM].GetWeaponType();
        if (type == WeaponType.dagger || type == WeaponType.sword)
        {
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

        playerManager.SetPlayerWeaponController = this;
    }

    public void StartAttack()
    {
        int weaponCount = playerManager.SetPlayerWeapons.Count;
        for (int i = 0; i < weaponCount; i++)
        {
            slot[i].SetAttack = true;
            slot[i].WeaponAttack();
        }
    }

    public void StopAttack()
    {
        int slotCount = slot.Length;
        for (int i = 0; i < slotCount; i++)
        {
            slot[i].SetAttack = false;
        }
    }

    private void SetStartWeapon()
    {
        var startWeapon = playerManager.SetStartWeapon;
        if (playerManager.SetPlayerWeapons.Count <= 0)
        {
            playerManager.SetPlayerWeapons.Add(startWeapon);
        }
    }

    public void UpdateWeapon()
    {
        int weaponCount = playerManager.SetPlayerWeapons.Count;
        for (int i = 0; i < weaponCount; i++)
        {
            var weapons = playerManager.SetPlayerWeapons[i];
            WeaponInfo info = WeaponTable.getInstance.GetWeaponInfoByIndex(weapons.weaponId);
            isRight = i == 0 ? false : true;
            slot[i].InitWeapon(info, gameSceneController, isRight);
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
                    slot[i].transform.eulerAngles = new Vector3(0f, 0f, -rotate);
                }
            }
        }
    }
}
