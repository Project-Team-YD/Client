using UnityEngine;
using Grpc.Core;
using MainGrpcClient;
using Server;
using Packet;
public class Test : MonoBehaviour
{

    private async void Start()
    {
        //ServerManager.GetInstance.ConnectToServer();

        RequestTest requestTest = new RequestTest();
        requestTest.id = "testId";
        requestTest.password = "testPassword";
        ResponseTest responseTest = await GrpcManager.GetInstance.RpcTest(requestTest);

        string result = $"MessageCode:{responseTest.code}/{responseTest.message}/{responseTest.seconds}";

        Debug.Log(result);

        ResponseTest responseTest2 = await GrpcManager.GetInstance.RpcStreamTest(requestTest);

        result = $"MessageCode:{responseTest.code}/{responseTest.message}/{responseTest.seconds}";

        Debug.Log(result);


    }

    private async void Update()
    {

        await GrpcManager.GetInstance.ReceiveRpcStreamAsync();
    }

}
