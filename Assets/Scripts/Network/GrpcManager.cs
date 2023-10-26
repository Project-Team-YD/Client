using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using MainGrpcClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Packet;
using Server;
using System;
using UnityEditor.VersionControl;

public class GrpcManager
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


    public async Task<string> SendRpcAsync(string rpcKey, string message)
    {
        
        try
        {
            // gRPC ��û ����
            GlobalGrpcRequest request = new GlobalGrpcRequest
            {
                RpcKey = rpcKey,
                Message = message
            };
           
            GlobalGrpcResponse response = await ServerManager.GetInstance.grpcClient.GlobalGRpcAsync(request);
            return response.Message;
        }
        catch (System.Exception e)
        {
            Debug.LogError("gRPC Error: " + e.Message);
        }

        return null;
    }


    public async Task<string> SendRpcStreamAsync(string rpcKey, string message)
    {
        try
        {
            var requestStream = ServerManager.GetInstance.grpcClient.GlobalGrpcStream();

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

    public async Task<string> ReceiveRpcStreamAsync()
    {
        try
        {
            var responseStream = ServerManager.GetInstance.grpcClient.GlobalGrpcStream();

            // 서버에서 오는 응답 메시지 처리
            while (await responseStream.ResponseStream.MoveNext())
            {
                var response = responseStream.ResponseStream.Current;
                Debug.Log("Received response from server: " + response.Message);
                string result = response.Message;
                return result; 
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error handling server responses: " + e.Message);
        }

        return null;
    }






    public async Task<ResponseTest> RpcTest(RequestTest requestPacket)
    {
        string rpcKey = "rpcTest";
        string jsonData = JsonConvert.SerializeObject(requestPacket);
        
        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseTest>(result);
        return response;
    }

    public async Task<ResponseTest> RpcStreamTest(RequestTest requestPacket)
    {
        string rpcKey = "rpcTest";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcStreamAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseTest>(result);
        return response;
    }

}
