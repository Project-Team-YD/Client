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
        public GlobalGRpcService.GlobalGRpcServiceClient grpcClient;
        public string uuid;

        private ServerManager()
        {
        }
        ~ServerManager()
        {
            // gRPC ä�� ����
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
            // gRPC ä�� �ʱ�ȭ
            channel = new Channel($"{loginServerIp}:{loginServerPort}", ChannelCredentials.Insecure);

            // gRPC Ŭ���̾�Ʈ �ʱ�ȭ
            grpcClient = new GlobalGRpcService.GlobalGRpcServiceClient(channel);

        }
        
        public void ConnectToGrpcGameServer()
        {

            // gRPC ä�� �ʱ�ȭ
            channel = new Channel($"{gameServerIp}:{gameServerPort}", ChannelCredentials.Insecure, new List<ChannelOption>
            {
                new ChannelOption("uuid", uuid)
            });

            // gRPC Ŭ���̾�Ʈ �ʱ�ȭ
            grpcClient = new GlobalGRpcService.GlobalGRpcServiceClient(channel);

        }

        public string loginServerIp = "localhost";
        public int loginServerPort = 19001;
        public string gameServerIp = "localhost";
        public int gameServerPort = 19001;

        
    }
}

