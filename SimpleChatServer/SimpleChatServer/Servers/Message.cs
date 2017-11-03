using System;
using System.IO;
using Google.Protobuf;
using ProtocData;

namespace SimpleChatServer.Servers
{
    class Message
    {
        const int BufferMaxCount = 2048;
        public byte[] Buffer { get; set; } = new byte[BufferMaxCount];
        public int Offset { get; set; }
        public int RemainCount => BufferMaxCount - Offset;

        public void ReceiveMessage(int count, Action<MsgType, byte[]> onReceiveMessage)
        {
            Offset += count;
            while (true)
            {
                if (Offset < 2) break;
                int type = 0;
                byte[] result = Decode(ref type);
                if (result == null)
                    break;
                //收到消息 result
                onReceiveMessage?.Invoke((MsgType)type, result);
            }
        }

        public static byte[] Encode(MsgType type, IMessage data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                data.WriteTo(ms);
                return Encode(type, ms.ToArray());
            }
        }

        /// <summary>
        /// 编码
        /// </summary>
        public static byte[] Encode(MsgType type, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((short)(data.Length + 2));
                bw.Write((short)type);
                bw.Write(data);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 解码 从消息中读取一条完整的消息
        /// </summary>
        public byte[] Decode(ref int type)
        {
            using (MemoryStream ms = new MemoryStream(Buffer, 0, Offset))
            using (BinaryReader br = new BinaryReader(ms))
            {
                short length = br.ReadInt16();
                if (length > ms.Length - ms.Position)
                {
                    return null;
                }
                type = br.ReadInt16();
                byte[] result = br.ReadBytes(length - 2);
                Offset = (int)ms.Length - (int)ms.Position;
                br.ReadBytes(Offset).CopyTo(Buffer, 0);
                return result;
            }
        }
    }
}
