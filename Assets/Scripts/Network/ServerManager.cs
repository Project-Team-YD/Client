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
            // gRPC 채널 종료
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
            // gRPC 서버의 주소와 포트 설정
            string serverAddress = "localhost";
            int serverPort = 19001;

            // gRPC 채널 초기화
            channel = new Channel($"{serverAddress}:{serverPort}", ChannelCredentials.Insecure);

            // gRPC 클라이언트 초기화
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

