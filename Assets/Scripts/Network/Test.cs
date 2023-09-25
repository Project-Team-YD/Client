using UnityEngine;
using Grpc.Core;
using MainGrpcClient;

public class Test : MonoBehaviour
{
    private Channel channel;
    private GlobalRpcService.GlobalRpcServiceClient grpcClient;

    private void Start()
    {
        // gRPC 서버의 주소와 포트 설정
        string serverAddress = "localhost";
        int serverPort = 19001;

        // gRPC 채널 초기화
        channel = new Channel($"{serverAddress}:{serverPort}", ChannelCredentials.Insecure);

        // gRPC 클라이언트 초기화
        grpcClient = new GlobalRpcService.GlobalRpcServiceClient(channel);

        // gRPC 요청 보내기 예시
        SendGrpcRequest();
    }

    private async void SendGrpcRequest()
    {
        // gRPC 요청 생성
        GlobalGrpcRequest request = new GlobalGrpcRequest
        {
            RpcKey = "SampleRpcKey",
            Message = "Hello gRPC server!"
        };

        try
        {
            // gRPC 요청 보내기
            GlobalGrpcResponse response = await grpcClient.SayHelloAsync(request);

            // gRPC 응답 출력
            Debug.Log("gRPC Response - RpcKey: " + response.RpcKey + ", Message: " + response.Message);
        }
        catch (System.Exception e)
        {
            Debug.LogError("gRPC Error: " + e.Message);
        }
    }

    private void OnDestroy()
    {
        // gRPC 채널 종료
        channel.ShutdownAsync().Wait();
    }
}
