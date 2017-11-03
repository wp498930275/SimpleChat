using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using ProtocData;

namespace SimpleChatServer.Servers
{
    class Client
    {
        public Socket ClientSocket { get; private set; }
        public Server Server { get; private set; }

        Message mMsg = new Message();

        public Client(Socket clientSocket, Server server)
        {
            ClientSocket = clientSocket;
            Server = server;
        }

        //关闭Client
        void Close()
        {

        }

        public void Start()
        {
            try
            {
                ClientSocket.BeginReceive(mMsg.Buffer, mMsg.Offset, mMsg.RemainCount, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception)
            {
                Console.WriteLine("连接异常，断开客户端与服务器的连接");
                Close();
            }
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int count = ClientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                    return;
                }
                mMsg.ReceiveMessage(count, OnReceive);
                Start();
            }
            catch (Exception)
            {
                Console.WriteLine("连接异常，断开客户端与服务器的连接");
                Close();
            }
        }

        /// <summary>
        /// 接受到消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data">一条完整的消息</param>
        void OnReceive(MsgType type, byte[] data)
        {
            Server.HandleRequest(type, data, this);
        }

        public void Send(MsgType type, IMessage data)
        {
            Console.WriteLine("向客户端发送响应 " + type);
            if (ClientSocket == null || !ClientSocket.Connected)
            {
                Console.WriteLine("mClientSocket为空，或者mClientSocket已经被关闭");
                return;
            }
            ClientSocket.Send(Message.Encode(type, data));
        }
    }
}
