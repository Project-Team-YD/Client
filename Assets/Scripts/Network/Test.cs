using UnityEngine;
using Grpc.Core;
using MainGrpcClient;
using Server;
using Packet;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour
{

    private async void Start()
    {
        ServerManager.GetInstance.ConnectToGrpcLoginServer();

        //-- 로그인서버 접근하여 로그인 요청
        RequestLogin login = new RequestLogin();
        login.id = "test";
        Debug.Log("Request!!");
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
  
            try
            {
                //-- 게임서버로부터 받을 메시지들 처리
                GrpcManager.GetInstance.ReceiveBroadcastMessages();
            }
            catch(System.OperationCanceledException)
            {
                Debug.Log("클라이언트에서 루프가 중지 되었습니다.");
            }
        }
        
    }
    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var result = await GrpcManager.GetInstance.LoadTables();
            Debug.Log("LoadTables");
            Debug.Log($"Code:{result.code}, Message:{result.message}, Gold:{result.money}");
            Debug.Log("-----------------------");
            Debug.Log("Load ItemTable");
            foreach (KeyValuePair<int, Item> entry in result.itemTable)
            {
                Debug.Log($"Id: {entry.Key} / ItemName: {entry.Value.itemName}");
            }
            Debug.Log("-----------------------");
            Debug.Log("Load ItemWeaponTable");
            foreach (KeyValuePair<int, ItemWeapon> entry in result.itemWeaponTable)
            {
                Debug.Log($"Id: {entry.Key} / Damage: {entry.Value.damage}");
            }
            Debug.Log("-----------------------");
            Debug.Log("Load ItemEffectTable");
            foreach (KeyValuePair<int, ItemEffect> entry in result.itemEffectTable)
            {
                Debug.Log($"Id: {entry.Key} / MaxHp: {entry.Value.maxHp}");
            }
            Debug.Log("-----------------------");
            Debug.Log("Load ShopTable");
            foreach (KeyValuePair<int, ShopItem> entry in result.shopTable)
            {
                Debug.Log($"Id: {entry.Key} / Price: {entry.Value.price}");
            }
            Debug.Log("-----------------------");
            Debug.Log("Load WeaponEnchantTable");
            foreach (KeyValuePair<int, WeaponEnchant> entry in result.weaponEnchantTable)
            {
                Debug.Log($"Id: {entry.Key}/ Value: {entry.Value.enchant}");
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            var result = await GrpcManager.GetInstance.LoadInventory();
            Debug.Log("LoadInventory");
            Debug.Log("LoadTables");
            Debug.Log($"Code:{result.code}, Message:{result.message}");
            Debug.Log("-----------------------");
            foreach (KeyValuePair<int, InventoryItem> entry in result.items)
            {
                Debug.Log($"Id: {entry.Key}/ Enchant: {entry.Value.enchant} / Count: {entry.Value.count}");
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            RequestBuyItem buyItem = new RequestBuyItem();
            buyItem.id = 2;
            var result = await GrpcManager.GetInstance.BuyItem(buyItem);
            Debug.Log("BuyItem");
            Debug.Log($"Message: {result.message} / Id: {result.id} / Enchant: {result.enchant} / Count: {result.count} / CurrentMoney: {result.money}");
            Debug.Log("-----------------------");

        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            RequestUpgradeItem upgradeItem = new RequestUpgradeItem();
            upgradeItem.id = 1;
            var result = await GrpcManager.GetInstance.UpgradeItem(upgradeItem);
            Debug.Log("UpgradeItem");
            Debug.Log($"Message: {result.message} / Id: {result.id} / Enchant: {result.enchant} / CurrentMoney: {result.money}");
            Debug.Log("-----------------------");

        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RequestJoinGame joinGame = new RequestJoinGame();
            joinGame.itemId = 1;
            var result = await GrpcManager.GetInstance.JoinGame(joinGame);
            Debug.Log("JoinGame");
            Debug.Log($"Message: {result.message} / Gold: {result.gold} / CurrentStage : {result.currentStage}");
            for (int i= 0; i < result.slot.Length; i++)
            {
                Debug.Log($"Slot {i} / Id: {result.slot[i].id} / Enchant : {result.slot[i].enchant}");
            }
            Debug.Log("-----------------------");
            for (int i = 0; i < result.effect.Length; i++)
            {
                Debug.Log($"Effect {i} / Id: {result.effect[i].id} / Count : {result.effect[i].count}");
            }
            Debug.Log("-----------------------");

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RequestLoadIngameShop loadIngameShop = new RequestLoadIngameShop();
            loadIngameShop.currentStage = 1;
            loadIngameShop.gold = 1000;
            var result = await GrpcManager.GetInstance.LoadIngameShop(loadIngameShop);
            Debug.Log("LoadIngameShop");
            for (int i= 0; i < result.items.Length; i++)
            {
                Debug.Log($"ItemId: {result.items[i].id} / Price: {result.items[i].price}");
            }
            Debug.Log("-----------------------");

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            RequestBuyIngameItem buyIngameItem = new RequestBuyIngameItem();
            buyIngameItem.itemId = 1;
            buyIngameItem.currentStage = 1;
            var result = await GrpcManager.GetInstance.BuyIngameItem(buyIngameItem);
            Debug.Log("BuyIngameItem");
            Debug.Log($"Message: {result.message} / Gold: {result.gold} / CurrentStage : {result.currentStage}");
            for (int i = 0; i < result.slot.Length; i++)
            {
                Debug.Log($"Slot {i} / Id: {result.slot[i].id} / Enchant : {result.slot[i].enchant}");
            }
            Debug.Log("-----------------------");
            for (int i = 0; i < result.effect.Length; i++)
            {
                Debug.Log($"Effect {i} / Id: {result.effect[i].id} / Count : {result.effect[i].count}");
            }
            Debug.Log("-----------------------");

        }
    }

    public void OnApplicationQuit()
    {
        // 클라이언트 종료 시 실행되는 코드
        ServerManager.GetInstance.call.Dispose(); // 스트리밍 콜 종료
        ServerManager.GetInstance.gameChannel.ShutdownAsync().Wait(); // gRPC 채널 종료
                                                                      // 다른 자원의 해제 작업 수행
    }
}

