using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using MainGrpcClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Packet;
using Server;


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
    public async Task<string> SendRpcAsync(string rpcKey, string message)
    {
        
        try
        {
            // gRPC 요청 생성
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

    public async Task<ResponseTest> RpcTest(RequestTest requestPacket)
    {
        string rpcKey = "rpcTest";
        string jsonData = JsonConvert.SerializeObject(requestPacket);
        
        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseTest>(result);
        return response;
    }
}
