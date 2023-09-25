using UnityEngine;
using Grpc.Core;
using MainGrpcClient;

public class Test : MonoBehaviour
{
    private Channel channel;
    private GlobalRpcService.GlobalRpcServiceClient grpcClient;

    private void Start()
    {
        // gRPC ������ �ּҿ� ��Ʈ ����
        string serverAddress = "localhost";
        int serverPort = 19001;

        // gRPC ä�� �ʱ�ȭ
        channel = new Channel($"{serverAddress}:{serverPort}", ChannelCredentials.Insecure);

        // gRPC Ŭ���̾�Ʈ �ʱ�ȭ
        grpcClient = new GlobalRpcService.GlobalRpcServiceClient(channel);

        // gRPC ��û ������ ����
        SendGrpcRequest();
    }

    private async void SendGrpcRequest()
    {
        // gRPC ��û ����
        GlobalGrpcRequest request = new GlobalGrpcRequest
        {
            RpcKey = "SampleRpcKey",
            Message = "Hello gRPC server!"
        };

        try
        {
            // gRPC ��û ������
            GlobalGrpcResponse response = await grpcClient.SayHelloAsync(request);

            // gRPC ���� ���
            Debug.Log("gRPC Response - RpcKey: " + response.RpcKey + ", Message: " + response.Message);
        }
        catch (System.Exception e)
        {
            Debug.LogError("gRPC Error: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        // gRPC ä�� ����
        channel.ShutdownAsync().Wait();
    }
}
