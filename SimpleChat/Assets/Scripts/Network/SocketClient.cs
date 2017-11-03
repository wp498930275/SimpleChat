using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Google.Protobuf;
using ProtocData;
using UnityEngine;

public class SocketClient
{
    const int MaxRead = 2048;
    byte[] mByteBuffer = new byte[MaxRead];
    MemoryStream mMemoryStream;
    BinaryReader mBinaryReader;
    NetworkStream mNetworkStream;
    TcpClient mTcpClient;

    public Action OnConnected;
    public Action OnDisconnected;
    public Action<MsgType, byte[]> OnReceiveMessage;

    public void Connect(string ip, int port)
    {
        this.Log($"Connect to {ip} port {port}");
        mTcpClient = new TcpClient
        {
            SendTimeout = 1000,
            ReceiveTimeout = 1000,
            NoDelay = true
        };
        try
        {
            mTcpClient.BeginConnect(ip, port, ConnectCalback, null);
        }
        catch (Exception e)
        {
            this.LogError("ConnectError " + e);
        }
    }

    public void Close()
    {
        mNetworkStream?.Close();
        if (mTcpClient != null)
        {
            if (mTcpClient.Connected)
                mTcpClient.Close();
            mTcpClient = null;
        }
        mMemoryStream?.Close();
        mBinaryReader?.Close();
    }

    void ConnectCalback(IAsyncResult ar)
    {
        if (!mTcpClient.Connected)
        {
            Disconnect("TcpClient 连接服务器失败");
            return;
        }

        mMemoryStream = new MemoryStream();
        mBinaryReader = new BinaryReader(mMemoryStream);
        mNetworkStream = mTcpClient.GetStream();
        mNetworkStream.BeginRead(mByteBuffer, 0, MaxRead, ReadCallback, null);

        OnConnected?.Invoke();
    }

    void Disconnect(string str)
    {
        this.Log("Disconnect " + str);
        Close();
        OnDisconnected?.Invoke();
    }

    //MemoryStream中剩下的字节数量
    int RemainingBytes()
    {
        return (int)(mMemoryStream.Length - mMemoryStream.Position);
    }

    #region 数据接收

    void ReadCallback(IAsyncResult ar)
    {
        try
        {
            int length = 0;
            //lock (mNetworkStream)
            {
                length = mNetworkStream.EndRead(ar);
            }
            if (length == 0)
            {
                Disconnect("接收到消息长度为0");
                return;
            }
            ReceiveMsg(length);
            //继续读取消息
            //lock (mNetworkStream)
            {
                mNetworkStream.BeginRead(mByteBuffer, 0, MaxRead, ReadCallback, null);
            }
        }
        catch (Exception)
        {
            Disconnect("接受数据异常，断开连接");
        }
    }

    void ReceiveMsg(int length)
    {
        //在mMemoryStream末尾添加新读取的数据
        mMemoryStream.Seek(0, SeekOrigin.End);
        mMemoryStream.Write(mByteBuffer, 0, length);
        mMemoryStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > 2)
        {
            short msgLen = mBinaryReader.ReadInt16();
            if (msgLen <= RemainingBytes())
            {
                int type = mBinaryReader.ReadInt16();
                byte[] data = mBinaryReader.ReadBytes(msgLen - 2);
                ReceiveResponse((MsgType)type, data);
            }
            else
            {
                mMemoryStream.Position = mMemoryStream.Position - 2;
                break;
            }
        }
        //剩下不完整的消息写入MemoryStream中
        byte[] leftover = mBinaryReader.ReadBytes(RemainingBytes());
        mMemoryStream.SetLength(0);
        mMemoryStream.Write(leftover, 0, leftover.Length);
    }

    void ReceiveResponse(MsgType type, byte[] data)
    {
        OnReceiveMessage?.Invoke(type, data);
    }

    #endregion

    #region 数据发送

    public void SendMessage(MsgType id, IMessage data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Write(BitConverter.GetBytes((short)id), 0, 2);
            data.WriteTo(ms);
            byte[] bytes = ms.ToArray();
            SendMessage(bytes);
        }
    }

    void SendMessage(byte[] data)
    {
        WriteMessage(data);
    }

    void WriteMessage(byte[] data)
    {
        if (mTcpClient != null && mTcpClient.Connected)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                ushort msglen = (ushort)data.Length;
                writer.Write(msglen);
                writer.Write(data);
                byte[] payload = ms.ToArray();
                lock (mNetworkStream)
                {
                    mNetworkStream.BeginWrite(payload, 0, payload.Length, OnWrite, null);
                }
            }
        }
        else
        {
            this.LogError("client.connected----->>false");
        }
    }

    void OnWrite(IAsyncResult ar)
    {
        try
        {
            mNetworkStream.EndWrite(ar);
        }
        catch (Exception ex)
        {
            this.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    #endregion
}