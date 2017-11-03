using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using ProtocData;
using SimpleChatServer.Servers;

namespace SimpleChatServer.NetMessage
{
    class MsgMng
    {
        #region 静态方法

        public delegate void MsgRequestCallback(IMessage msg, Client client);
        static MsgRequestCallback[] sMsgRequestCallback = new MsgRequestCallback[(int)MsgType.Max];

        public static void SetRequestCallback(MsgType id, MsgRequestCallback callback)
        {
            sMsgRequestCallback[(int)id] = callback;
        }

        public static MsgRequestCallback GetMsgCallBack(MsgType id)
        {
            return sMsgRequestCallback[(int)id];
        }

        #endregion

        Server mServer;

        public MsgMng(Server server)
        {
            mServer = server;
            Init();
        }

        void Init()
        {
            LoginNetMsg.Init();
        }

        public void HandleRequest(MsgType type, byte[] data, Client client)
        {
            Type t = MsgTypeMap.GeType(type);
            IMessage message = Activator.CreateInstance(t) as IMessage;
            message.MergeFrom(data);
            Console.WriteLine("接收到客户端请求 " + type);
            GetMsgCallBack(type).Invoke(message, client);
        }
    }
}
