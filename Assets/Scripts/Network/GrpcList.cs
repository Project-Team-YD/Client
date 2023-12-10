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
    
    public async Task<ResponseHeartBeat> CheckHeartBeat(RequestHeartBeat requestPacket)
    {
        string rpcKey = "check_heartbeat";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseHeartBeat>(result);
        return response;
    }

    public Response GetDuplicateLogin(string message)
    {
        var response = JsonConvert.DeserializeObject<Response>(message);
        return response;
    }


    public async Task<ResponseGameDB> LoadTables()
    {
        string rpcKey = "load_tables";

        string result = await SendRpcAsync(rpcKey);

        var response = JsonConvert.DeserializeObject<ResponseGameDB>(result);
        return response;
    }

    public async Task<ResponseInventory> LoadInventory()
    {
        string rpcKey = "load_inventory";

        string result = await SendRpcAsync(rpcKey);

        var response = JsonConvert.DeserializeObject<ResponseInventory>(result);
        return response;
    }
    public async Task<ResponseBuyItem> BuyItem(RequestBuyItem requestPacket)
    {
        string rpcKey = "buy_item";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseBuyItem>(result);
        return response;
    }
    public async Task<ResponseUpgradeItem> UpgradeItem(RequestUpgradeItem requestPacket)
    {
        string rpcKey = "upgrade_item";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseUpgradeItem>(result);
        return response;
    }
    public async Task<ResponseJoinGame> JoinGame(RequestJoinGame requestPacket)
    {
        string rpcKey = "join_game";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseJoinGame>(result);
        return response;
    }
    public async Task<ResponseLoadIngameShop> LoadIngameShop(RequestLoadIngameShop requestPacket)
    {
        string rpcKey = "load_ingame_shop";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseLoadIngameShop>(result);
        return response;
    }
    public async Task<ResponseBuyIngameItem> BuyIngameItem(RequestBuyIngameItem requestPacket)
    {
        string rpcKey = "buy_ingame_item";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<ResponseBuyIngameItem>(result);
        return response;
    }

    public async Task<Response> UserName(RequestUserName requestPacket)
    {
        string rpcKey = "user_name";
        string jsonData = JsonConvert.SerializeObject(requestPacket);

        string result = await SendRpcAsync(rpcKey, jsonData);

        var response = JsonConvert.DeserializeObject<Response>(result);
        return response;
    }
}

