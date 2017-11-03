using Google.Protobuf;
using ProtocData;
using UnityEngine;

public static class LoginNetMsg
{
    public static void Init()
    {
        //注册接受消息的处理
        NetworkManager.SetResponseCallback(MsgType.LoginResponse, LoginResponseCallback);
        NetworkManager.SetResponseCallback(MsgType.RegisterResponse, RegisterResponseCallBack);
    }

    public static void LoginRequest(string username, string password)
    {
        LoginRequest data = new LoginRequest
        {
            Username = username,
            Password = password
        };
        NetworkManager.Instance.SendMessage(MsgType.LoginRequest, data);
    }

    static void LoginResponseCallback(IMessage msg)
    {
        LoginResponse data = msg as LoginResponse;
        Debuger.Log("LoginResponseCallback ReturnCode=" + data.ReturnCode);
    }

    public static void RegisterRequest(string username, string password)
    {
        RegisterRequest data = new RegisterRequest
        {
            Username = username,
            Password = password
        };
        NetworkManager.Instance.SendMessage(MsgType.RegisterRequest, data);
    }

    static void RegisterResponseCallBack(IMessage msg)
    {
        RegisterResponse data = msg as RegisterResponse;
        Debuger.Log("RegisterResponseCallBack ReturnCode=" + data.ReturnCode);
    }
}