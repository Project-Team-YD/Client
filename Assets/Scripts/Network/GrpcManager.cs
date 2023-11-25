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
            var metaData = new Metadata();
            metaData.Add("UUID", ServerManager.GetInstance.UUID);
            GlobalGrpcResponse response = await ServerManager.GetInstance.grpcGameServerClient.GlobalGRpcAsync(request, metaData);
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

    
    public async void ReceiveBroadcastMessages()
    {
        var call = ServerManager.GetInstance.call;
        var cancellationTokenSource = ServerManager.GetInstance.cancellationTokenSource;
        //dd
        // 비동기 루프를 시작하여 들어오는 메시지를 수신
        await Task.Run(async () =>
        {
            try
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var response = call.ResponseStream.Current;
                    var opcode = call.ResponseStream.Current.Opcode;
                    // 여기서 수신된 응답을 처리
                    Debug.Log($"Received message: {response.Message}");
                    switch(opcode)
                    {
                        case (int)Opcode.HEARTBEAT:
                            var requestPacket = new RequestHeartBeat();
                            requestPacket.heartBeat = ServerManager.GetInstance.heartBeat;

                            Debug.Log($"Request HeartBeat!! HeartBeat:{requestPacket.heartBeat}");
                            var responsePacket = await CheckHeartBeat(requestPacket);
                            if (responsePacket.code == (int)MessageCode.Success)
                            {
                                ServerManager.GetInstance.heartBeat = responsePacket.heartBeat;
                                Debug.Log($"Change HeartBeat!! HeartBeat:{responsePacket.heartBeat}");
                            }
                            break;
                        case (int)Opcode.DUPLICATE_LOGIN:
                            break;
                    }
                }
            }
            catch (RpcException ex)
            {
                // RpcException 처리 (예: 서버가 스트림을 닫음)
                Debug.LogError($"RpcException: {ex.Status.Detail}");
            }
        }, cancellationTokenSource.Token);
    }

}
