using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using ProtocData;
using SimpleChatServer.NetMessage;

namespace SimpleChatServer.Servers
{
    class Server
    {
        Socket mServerSocket;
        const int Port = 8848;
        const string Ip = "127.0.0.1";
        List<Client> mClients = new List<Client>();
        MsgMng mMsgMng;

        public void Start()
        {
            mMsgMng = new MsgMng(this);
            mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mServerSocket.Bind(new IPEndPoint(IPAddress.Parse(Ip), Port));
            mServerSocket.Listen(0);
            //开始死循环监听客户端请求，之后的代码不会执行
            ListenClientAccept();
        }

        void ListenClientAccept()
        {
            while (true)
            {
                try
                {
                    Socket clientSocket = mServerSocket.Accept();
                    Console.WriteLine("收到客户端连接请求");
                    BeginClientAccept(clientSocket);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        void BeginClientAccept(Socket clientSocket)
        {
            Client client = new Client(clientSocket, this);
            client.Start();
            mClients.Add(client);
        }

        /// <summary>
        /// 处理客户端发送过来的请求
        /// </summary>
        /// <param name="data">请求数据</param>
        /// <param name="client">客户端</param>
        public void HandleRequest(MsgType type, byte[] data, Client client)
        {
            //TODO：处理请求  暂时把消息本身返回客户端
            //SendResponse(data, client);
            mMsgMng.HandleRequest(type, data, client);
        }

        public void SendResponse(MsgType type, IMessage data, Client client)
        {
            client.Send(type, data);
        }
    }
}
