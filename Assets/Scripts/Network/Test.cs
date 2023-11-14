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
    }

    public void OnApplicationQuit()
    {
        // 클라이언트 종료 시 실행되는 코드
        ServerManager.GetInstance.call.Dispose(); // 스트리밍 콜 종료
        ServerManager.GetInstance.gameChannel.ShutdownAsync().Wait(); // gRPC 채널 종료
                                                                      // 다른 자원의 해제 작업 수행
    }
}

