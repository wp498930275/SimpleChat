using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtocData;

namespace GenerateMsgTypeMap
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "MsgTypeTypeMap";
            //foreach (var value in Enum.GetValues(typeof(MsgType)))
            //{
            //    Console.WriteLine(value);
            //}
            //Console.ReadKey();
            using (FileStream file = File.Open($"{str}.cs", FileMode.OpenOrCreate))
            using (StreamWriter sw = new StreamWriter(file))
            {
                file.SetLength(0);
                sw.Write(@"using System;
using System.Collections.Generic;
namespace ProtocData
{
    public static class MsgTypeMap
    {
        public static Type GeType(MsgType id)
        {
            return sMap[id];
        }
        static Dictionary<MsgType, Type> sMap = new Dictionary<MsgType, Type>
        {
");
                //{ MsgType.LoginRequest, typeof(LoginRequest)},
                //{ MsgType.LoginResponse, typeof(LoginResponse)},
                //{ MsgType.RegisterRequest, typeof(RegisterRequest)},
                //{ MsgType.RegisterResponse, typeof(RegisterResponse)}

                //写入中间部分
                foreach (var value in Enum.GetValues(typeof(MsgType)))
                {
                    if ((MsgType)value == MsgType.Max)
                    {
                        continue;
                    }
                    sw.WriteLine($"            {{ MsgType.{value},typeof({value})}},");
                }
                sw.Write(@"        };
    }
}");
            };
        }

    }
}