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


        private void ConnectToGrpcServer()
        {
            // gRPC ������ �ּҿ� ��Ʈ ����
            string serverAddress = "localhost";
            int serverPort = 19001;

            // gRPC ä�� �ʱ�ȭ
            channel = new Channel($"{serverAddress}:{serverPort}", ChannelCredentials.Insecure);

            // gRPC Ŭ���̾�Ʈ �ʱ�ȭ
            grpcClient = new GlobalGRpcService.GlobalGRpcServiceClient(channel);
        }


        public string ip = "localhost";
        public int port = 19001;

        public void ConnectToServer()
        {
            ConnectToGrpcServer();
        }
    }
}

