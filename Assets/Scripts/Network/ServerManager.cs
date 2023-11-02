using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using MainGrpcClient;

namespace Server
{
    public class ServerManager
    {
        private static ServerManager instance = null;
        private Channel channel;
        public GlobalGRpcService.GlobalGRpcServiceClient grpcLoginServerClient;
        public GlobalGRpcService.GlobalGRpcServiceClient grpcGameServerClient;
        public string UUID;

        private ServerManager()
        {
        }
        ~ServerManager()
        {
            // gRPC 연결해챛
            if (channel != null)
                channel.ShutdownAsync().Wait();
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
            channel = new Channel($"{loginServerIp}:{loginServerPort}", ChannelCredentials.Insecure);

            // gRPC 연결
            grpcLoginServerClient = new GlobalGRpcService.GlobalGRpcServiceClient(channel);

        }
        
        public void ConnectToGrpcGameServer()
        {

            // gRPC 채널 연결 
            channel = new Channel($"{gameServerIp}:{gameServerPort}", ChannelCredentials.Insecure, new List<ChannelOption>
            {
                new ChannelOption("UUID", UUID)
            });

            // gRPC 연
            grpcGameServerClient = new GlobalGRpcService.GlobalGRpcServiceClient(channel);

        }

        public string loginServerIp = "13.125.254.231";
        public int loginServerPort = 8081;
        public string gameServerIp = "13.125.254.231";
        public int gameServerPort = 8080;

        
    }
}

