using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponSlot[] slot;
    [SerializeField] private GameSceneController gameSceneController;

    private PlayerManager playerManager = null;

    private readonly float rotate = 45f;
    private Transform playerTransform = null;
    private bool isRight = false;

    public WeaponSlot[] GetWeapons { get { return slot; } }

    private void Awake()
    {
        playerManager = PlayerManager.getInstance;

        playerTransform = gameObject.transform.parent.transform;
        int slotCount = slot.Length;
        for (int i = 0; i < slotCount; i++) // 슬릇 두개에 근접무기 넣어두고 회전테스트.
        {
            WeaponInfo info = WeaponTable.getInstance.GetWeaponInfoByIndex(i + 2); // 임시..테스트용
            isRight = i == 0 ? false : true;
            slot[i].InitWeapon(info, gameSceneController, isRight);
            slot[i].SetTarget(playerTransform);
            var type = slot[i].GetWeaponType();
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

        playerManager.SetPlayerWeapon = this;
    }

    private void Start()
    {
        int slotCount = slot.Length;
        for (int i = 0; i < slotCount; i++)
        {
            slot[i].WeaponAttack();
        }
    }
}
