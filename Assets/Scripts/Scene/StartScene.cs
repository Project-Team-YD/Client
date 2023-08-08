using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using HSMLibrary.Scene;
using System.Collections;
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
        isTableLoadSuccess ^= await EnemyTable.getInstance.Initialize();
        //TODO :: 임시..팀장님께 XOR 연산자 TableLoad 어떻게 해야 좋을지 여쭤보기
        await MapTable.getInstance.Initialize();

        if(!isTableLoadSuccess)
        {
            throw new Exception("TableLoad Fail");
        }

        isInitialized = true;

        SceneHelper.getInstance.ChangeScene(typeof(IntroScene));
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