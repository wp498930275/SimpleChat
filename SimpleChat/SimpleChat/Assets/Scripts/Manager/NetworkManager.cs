using System;
using System.Collections.Generic;
using Google.Protobuf;
using ProtocData;
using UnityEngine;

public class NetworkManager : BaseManager
{
    Queue<KeyValuePair<MsgType, IMessage>> mMessages = new Queue<KeyValuePair<MsgType, IMessage>>();


    SocketClient mSocketClient;

    public NetworkManager(GameFacade facade) : base(facade)
    {
        Instance = this;
        LoginNetMsg.Init();
    }

    public static NetworkManager Instance { get; private set; }

    public void Connect(string ip, int port)
    {
        mSocketClient = new SocketClient();
        mSocketClient.OnConnected += OnConnected;
        mSocketClient.OnDisconnected += OnDisconnected;
        mSocketClient.OnReceiveMessage += OnReceiveMessage;
        mSocketClient.Connect(ip, port);
    }

    public override void Update()
    {
        base.Update();
        //TODO: 处理接受到的消息,在主线程处理这些消息，网络在别的线程里面，不能调用很多Unity方法
        while (mMessages.Count != 0)
        {
            //byte[] msg = mMessages.Dequeue();
            var tmp = mMessages.Dequeue();
            GetMsgCallBack(tmp.Key).Invoke(tmp.Value);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Disconnect();
    }

    void OnConnected()
    {
        this.Log("OnConnected");
    }

    void OnDisconnected()
    {
        this.Log("OnDisconnected");
    }

    void OnReceiveMessage(MsgType type, byte[] data)
    {
        this.Log($"receive response : {type}");
        Type t = MsgTypeMap.GeType(type);
        IMessage message = Activator.CreateInstance(t) as IMessage;
        message.MergeFrom(data);
        mMessages.Enqueue(new KeyValuePair<MsgType, IMessage>(type, message));
    }

    public void Disconnect()
    {
        mSocketClient.OnConnected -= OnConnected;
        mSocketClient.OnDisconnected -= OnDisconnected;
        mSocketClient.OnReceiveMessage -= OnReceiveMessage;
        mSocketClient.Close();
    }

    public void SendMessage(MsgType id, IMessage data)
    {
        mSocketClient.SendMessage(id, data);
    }

    #region 静态方法

    public delegate void MsgResponseCallback(IMessage msg);

    static MsgResponseCallback[] sMsgResponseCallback = new MsgResponseCallback[(int) MsgType.Max];

    public static void SetResponseCallback(MsgType id, MsgResponseCallback callback)
    {
        sMsgResponseCallback[(int) id] = callback;
    }

    public static MsgResponseCallback GetMsgCallBack(MsgType id)
    {
        return sMsgResponseCallback[(int) id];
    }

    #endregion
}