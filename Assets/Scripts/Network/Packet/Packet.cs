using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Packet
{
    public class Request
    {

    }
    public class Response
    {
        public uint code;
        public string message; 
    }


    public class RequestLogin
    {
        public string id;
    }

    public class ResponseLogin : Response
    {
        public string UUID;
    }


    public class RequestHeartBeat
    {
        public string heartBeat;
    }

    public class ResponseHeartBeat : Response
    {
        public string heartBeat;
    }
}

