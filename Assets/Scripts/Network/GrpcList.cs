using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Packet;
public partial class GrpcManager 
{
    public async Task<ResponseLogin> Login(RequestLogin requestPacket)
    {
        string rpcKey = "login";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendLoginRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseLogin>(result);
        return response;
    }
    
    public async Task<Response> HeartBeat(RequestHeartBeat requestPacket)
    {
        string rpcKey = "check_heartbeat";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<Response>(result);
        return response;
    }

    public Response GetHeartBeat(string message)
    {
        var response = JsonConvert.DeserializeObject<Response>(message);
        return response;
    }

    public Response GetDuplicateLogin(string message)
    {
        var response = JsonConvert.DeserializeObject<Response>(message);
        return response;
    }
}

