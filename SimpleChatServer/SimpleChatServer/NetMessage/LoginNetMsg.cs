using System;
using ProtocData;
using Google.Protobuf;
using SimpleChatServer.Servers;

namespace SimpleChatServer.NetMessage
{
    class LoginNetMsg
    {
        public static void Init()
        {
            //注册接受消息的处理
            MsgMng.SetRequestCallback(MsgType.LoginRequest, LoginRequestCallback);
            MsgMng.SetRequestCallback(MsgType.RegisterRequest, RegisterRequestCallBack);
        }

        static void LoginRequestCallback(IMessage msg, Client client)
        {
            LoginResponse(client, int.Parse((msg as LoginRequest).Username));
        }

        public static void LoginResponse(Client client, int returnCode)
        {
            LoginResponse msg = new LoginResponse
            {
                ReturnCode = returnCode
            };
            client.Send(MsgType.LoginResponse, msg);
        }

        static void RegisterRequestCallBack(IMessage msg, Client client)
        {
            RegisterResponse(client, 1);
        }

        public static void RegisterResponse(Client client, int returnCode)
        {
            RegisterResponse msg = new RegisterResponse
            {
                ReturnCode = returnCode
            };
            client.Send(MsgType.RegisterResponse, msg);
        }
    }
}
