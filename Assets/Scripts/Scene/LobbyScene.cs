using HSMLibrary.Manager;
using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : Scene
{
    public override async void OnActivate()
    {
        TimeManager.getInstance.PlayTime();
        var result = await GrpcManager.GetInstance.LoadInventory();
        WeaponTable.getInstance.SetinventoryData(result.items);
        WeaponTable.getInstance.InitWeaponInfo();
        //SceneHelper.getInstance.ChangeScene(typeof(GameScene));
    }
    public override void OnDeactivate()
    {

    }
    public override void OnUpdate()
    {

    }
}
