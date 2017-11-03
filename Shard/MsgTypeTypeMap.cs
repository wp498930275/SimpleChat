using System;
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
            { MsgType.LoginRequest,typeof(LoginRequest)},
            { MsgType.LoginResponse,typeof(LoginResponse)},
            { MsgType.RegisterRequest,typeof(RegisterRequest)},
            { MsgType.RegisterResponse,typeof(RegisterResponse)},
        };
    }
}