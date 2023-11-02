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

        RequestLogin login = new RequestLogin();
        login.id = "test";
        ResponseLogin loginResponse = await GrpcManager.GetInstance.Login(login);

        string result = $"MessageCode:{loginResponse.code}/{loginResponse.message}/{loginResponse.UUID}";

        Debug.Log(result);

        if (loginResponse.code == (int)MessageCode.Success)
        {
            ServerManager.GetInstance.UUID = loginResponse.UUID;
            ServerManager.GetInstance.ConnectToGrpcGameServer();
            StartCoroutine(Receive());
        }
    }

    private async void Update()
    {

     
    }


    public IEnumerator Receive()
    {
        GrpcManager.GetInstance.ReceiveBroadcastFromGameServer();

        yield return null;
    }
}

