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


    /* 전체적인 서버 연결 구조

     * 클라이언트에서 로그인서버에 로그인정보를 호출후 로그인 서버로부터
     * 접속 가능여부를 판별받게 된다. (중복 로그인)
     * 중복 로그인시 로그인 시도한 클라이언트는 접속 불가 메시지를 받게되고
     * 이미 접속중인 계정인 클라이언트에는 게임서버로부터 중복접속 알림 메시지를 받게된다.
     * 로그인 성공시 로그인서버로부터 만들어진 HeartBeat(이하 하트비트)의 값과 UUID를 할당받게 된다.
     * 이후 할당받은 UUID값을 게임서버에 연결하면서 Metadata의 값으로 넣고 게임서버와 연결한다.

     * 연결된 게임서버로부터 일정 시간마다 하트비트의 값을 요구하는 메시지가 들어오게 되며
     * 해당 메시지 수신시 현재 가지고있는 하트비트의 값을 게임서버에 보내게 되고 갱신된 하트비트의 값을 넘겨받아 기존의 값에서 새로 갱신한다.
     * 이렇게 하트비트의 구조는 완성이 되고 서버와 연결되있는 동안에는 반복적으로 비동기로 처리하여 이뤄진다.

     */

    #region 로그인
    
    public class RequestLogin
    {
        public string id;
    }

    public class ResponseLogin : Response
    {
        public string UUID;
        public string heartBeat;
    }
    #endregion

    public class RequestHeartBeat
    {
        public string heartBeat;
    }

    public class ResponseHeartBeat : Response
    {
        public string heartBeat;
    }
}

