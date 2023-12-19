using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using UnityEngine;
using Server;
using Packet;


public class StartScene : Scene
{
    private static bool isInitialized = false;
    private TableManager tablemanager;

    private async UniTaskVoid OnInitializeAppAsync()
    {
        if (isInitialized)
        {
            return;
        }

        var loadingPanel = UIManager.getInstance.Show<LoadingPanelController>("LoadingPanel");

        ServerManager.GetInstance.ConnectToGrpcLoginServer();

        //-- 로그인서버 접근하여 로그인 요청
        RequestLogin login = new RequestLogin();
        login.id = SystemInfo.deviceUniqueIdentifier;
        ResponseLogin loginResponse = await GrpcManager.GetInstance.Login(login);

        string result = $"Response MessageCode:{loginResponse.code}/{loginResponse.message}/{loginResponse.UUID}";

        Debug.Log(result);
        Debug.Log($"HeartBeat:{loginResponse.heartBeat}");
        //-- 로그인 성공시
        if (loginResponse.code == (int)MessageCode.Success)
        {
            //-- 로그인서버로부터 발급받은 UUID, HeartBeat값으로 초기화
            ServerManager.GetInstance.UUID = loginResponse.UUID;
            ServerManager.GetInstance.heartBeat = loginResponse.heartBeat;

            //-- 게임서버 연결
            ServerManager.GetInstance.ConnectToGrpcGameServer();

            PlayerManager.getInstance.UserName = loginResponse.userName;

            try
            {
                //-- 게임서버로부터 받을 메시지들 처리
                GrpcManager.GetInstance.ReceiveBroadcastMessages();
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("클라이언트에서 루프가 중지 되었습니다.");
            }
        }
        else
        {
            var panel = await UIManager.getInstance.Show<MessageOneButtonBoxPopupController>("MessageOneButtonBoxPopup");
            panel.InitPopup("서버와 연결을 실패했습니다.\n게임을 다시 시작해주세요!", Application.Quit);
        }
        tablemanager = TableManager.getInstance;
        var tables = await GrpcManager.GetInstance.LoadTables();
        tablemanager.SetTableItem(tables.itemTable);
        tablemanager.SetTableWeapon(tables.itemWeaponTable);
        tablemanager.SetTableEffect(tables.itemEffectTable);
        tablemanager.SetTableShop(tables.shopTable);
        tablemanager.SetTableEnhant(tables.weaponEnchantTable);
        PlayerManager.getInstance.CurrentMoney = tables.money;

        bool isTableLoadSuccess = false;

        isTableLoadSuccess = await TableLoading();


        if (!isTableLoadSuccess)
        {
            throw new Exception("TableLoad Fail");
        }

        isInitialized = true;

        UIManager.getInstance.Hide();

        SceneHelper.getInstance.ChangeScene(typeof(IntroScene));
    }
    private async UniTask<bool> TableLoading()
    {
        await UniTask.WhenAll(EnemyTable.getInstance.Initialize()
            , MapTable.getInstance.Initialize()
            // , WeaponTable.getInstance.Initialize()
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