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

    public class RequestTest
    {
        public string id;
        public string password;
    }
    public class ResponseTest : Response
    {
        public string seconds;
    }
}

