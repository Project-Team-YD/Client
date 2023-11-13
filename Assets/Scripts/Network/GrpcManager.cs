using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using MainGrpcClient;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Packet;
using Server;
using System;
using unityTask = UnityEditor.VersionControl.Task;


public partial class GrpcManager
{
    private GrpcManager() 
    {
        
    }
    ~GrpcManager() 
    { 
        if (instance != null)
            instance = null;
    }
    private static GrpcManager instance = null;
    public static GrpcManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                instance = new GrpcManager();
            }
            return instance;
        }
    }
    #region

    #endregion


    public async Task<string> SendLoginRpcAsync(string rpcKey, string message)
    {
        
        try
        {
            // gRPC 패킷 전
            GlobalGrpcRequest request = new GlobalGrpcRequest
            {
                RpcKey = rpcKey,
                Message = message
            };
           
            GlobalGrpcResponse response = await ServerManager.GetInstance.grpcLoginServerClient.GlobalGRpcAsync(request);
            return response.Message;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Login gRPC Error: " + e.Message);
        }

        return null;
    }

    public async Task<string> SendRpcAsync(string rpcKey, string message)
    {

        try
        {
            // gRPC 패킷 전
            GlobalGrpcRequest request = new GlobalGrpcRequest
            {
                RpcKey = rpcKey,
                Message = message
            };

            GlobalGrpcResponse response = await ServerManager.GetInstance.grpcGameServerClient.GlobalGRpcAsync(request);
            return response.Message;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Game gRPC Error: " + e.Message);
        }

        return null;
    }

    public async Task<string> SendRpcStreamAsync(string rpcKey, string message)
    {
        try
        {
            var requestStream = ServerManager.GetInstance.grpcGameServerClient.GlobalGrpcStream();

            // 보낼 요청 메시지 설정
            var request = new GlobalGrpcRequest
            {
                RpcKey = rpcKey,
                Message = message
            };

            // 요청 메시지 보내기
            await requestStream.RequestStream.WriteAsync(request);
            
        }
        catch (System.Exception e)
        {
            Debug.LogError("gRPC Error: " + e.Message);
        }

        return null;
    }
    public async Task<string> SendRpcStreamBroadcastAsync(string rpcKey, string message)
    {
        try
        {
            var requestStream = ServerManager.GetInstance.grpcGameServerClient.GlobalGrpcStreamBroadcast();

            // 보낼 요청 메시지 설정
            var request = new GlobalGrpcRequest
            {
                RpcKey = rpcKey,
                Message = message
            };

            // 요청 메시지 보내기
            await requestStream.RequestStream.WriteAsync(request);

        }
        catch (System.Exception e)
        {
            Debug.LogError("gRPC Error: " + e.Message);
        }

        return null;
    }
    
    public async Task ReceiveBroadcastMessages()
    {

        await Task.Yield();

        try
        {
            Debug.Log("TT");
            var responseStream = ServerManager.GetInstance.grpcGameServerClient.GlobalGrpcStreamBroadcast();
            var token = ServerManager.GetInstance.cancellationTokenSource.Token;
            // 서버에서 오는 응답 메시지 처리
            while (await responseStream.ResponseStream.MoveNext() && !token.IsCancellationRequested)
            {
                Debug.Log("TTTT");
                var response = responseStream.ResponseStream.Current;
                string message = response.Message;
                Debug.Log($"Broadcast Message : {message}");
                switch (response.Opcode)
                {
                    case (int)Opcode.PINGPONG:
                        await SendRpcStreamBroadcastAsync("ping", "");
                        break;
                    case (int)Opcode.HEARTBEAT:
                        //-- 하트비트 값들어오면 하트비트에 저장
                        Response resHeartBeat = GetHeartBeat(message);
                        RequestHeartBeat heartBeat = new RequestHeartBeat();
                        resHeartBeat = await HeartBeat(heartBeat);
                        if (resHeartBeat.code != (int)MessageCode.Success)
                        {
                            //-- 하트비트 처리 제대로 안됬을경우
                        }
                        break;
                    case (int)Opcode.DUPLICATE_LOGIN:
                        //-- 중복 로그인 처리
                        Response resDuplicateLogin = GetDuplicateLogin(message);
                        
                        break;
                }


                /*
                Debug.Log("Received response from server: " + response.Message);
                string result = response.Message;
                return result;
                */
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error handling server responses: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        
    }
}
