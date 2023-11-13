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
        ResponseLogin loginResponse = await GrpcManager.GetInstance.Login(login);

        string result = $"MessageCode:{loginResponse.code}/{loginResponse.message}/{loginResponse.UUID}";

        Debug.Log(result);

        if (loginResponse.code == (int)MessageCode.Success)
        {
            ServerManager.GetInstance.UUID = loginResponse.UUID;
            ServerManager.GetInstance.heartBeat = loginResponse.heartBeat;

            ServerManager.GetInstance.ConnectToGrpcGameServer();
            
            try
            {
                await GrpcManager.GetInstance.ReceiveBroadcastMessages();
            }
            catch(System.OperationCanceledException)
            {
                Debug.Log("클라이언트에서 루프가 중지 되었습니다.");
            }
        }

    }

    private void Update()
    {
        //GrpcManager.GetInstance.ReceiveBroadcastFromGameServer();

    }

}

