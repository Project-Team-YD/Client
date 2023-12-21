using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using MainGrpcClient;
using System.Threading;
namespace Server
{
    public class ServerManager
    {
        private static ServerManager instance = null;
        public Channel loginChannel;
        public Channel gameChannel;
        public GlobalGRpcService.GlobalGRpcServiceClient grpcLoginServerClient;
        public GlobalGRpcService.GlobalGRpcServiceClient grpcGameServerClient;
        public AsyncDuplexStreamingCall<GlobalGrpcRequest, GlobalGrpcBroadcast> call;
        public string UUID;
        public string heartBeat;
        private ServerManager()
        {
        }
        ~ServerManager()
        {
            // gRPC 연결해제
            if (loginChannel != null)
                loginChannel.ShutdownAsync().Wait();
            if (gameChannel != null)
                gameChannel.ShutdownAsync().Wait();
            if (instance != null)
                instance = null;
        }

        public static ServerManager GetInstance 
        { 
            get 
            {
                if (instance == null)
                {
                    instance = new ServerManager();
                }
                return instance; 
            }
        }

        public void ConnectToGrpcLoginServer()
        {
            // gRPC 채널 연결
            loginChannel = new Channel($"{loginServerIp}:{loginServerPort}", ChannelCredentials.Insecure);

            // gRPC 연결
            grpcLoginServerClient = new GlobalGRpcService.GlobalGRpcServiceClient(loginChannel);

        }

        public void ConnectToGrpcGameServer()
        {
            // gRPC 채널 연결 
            gameChannel = new Channel($"{gameServerIp}:{gameServerPort}", ChannelCredentials.Insecure);

            // gRPC 연결
            grpcGameServerClient = new GlobalGRpcService.GlobalGRpcServiceClient(gameChannel);
            cancellationTokenSource = new CancellationTokenSource();

            var metaData = new Metadata
            {
                { "uuid", UUID }
            };
#if UNITY_EDITOR
            Debug.Log("uuid::" + UUID);
#endif
            //CallOptions callOptions = new CallOptions(metaData);

            //var response = grpcGameServerClient.GlobalGrpcStreamBroadcast(metaData);
            call = grpcGameServerClient.GlobalGrpcStreamBroadcast(metaData);
        }
        public CancellationTokenSource cancellationTokenSource;
        public string loginServerIp = "13.125.254.231";
        public int loginServerPort = 8081;
        public string gameServerIp = "13.125.254.231";
        public int gameServerPort = 8080;

    }
}

