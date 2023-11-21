using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using UnityEngine;


public class StartScene : Scene
{
    private static bool isInitialized = false;

    private async UniTaskVoid OnInitializeAppAsync()
    {
        if (isInitialized)
        {
            return;
        }

        bool isTableLoadSuccess = false;        

        isTableLoadSuccess = await TableLoading();

        if(!isTableLoadSuccess)
        {
            throw new Exception("TableLoad Fail");
        }

        isInitialized = true;

        SceneHelper.getInstance.ChangeScene(typeof(IntroScene));
    }
    private async UniTask<bool> TableLoading()
    {
        await UniTask.WhenAll(EnemyTable.getInstance.Initialize()
            , MapTable.getInstance.Initialize()
            , WeaponTable.getInstance.Initialize()
            , StageTable.getInstance.Initialize());
        return true;
    }
    public override void OnActivate()
    {
        OnInitializeAppAsync().Forget();
    }

    public override void OnDeactivate()
    {

    }

    public override void OnUpdate()
    {

    }
}