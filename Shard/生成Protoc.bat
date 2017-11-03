echo off
echo .protoc 生成 .cs

rmdir tmp /s/q
mkdir tmp

::描述文件生成.cs文件
protoc --csharp_out=./ MsgProto.proto


::生成dll文件
echo .cs  生成 .dll
csc /target:library /reference:Google.Protobuf.dll /out:ProtoData.dll MsgProto.cs 


::依赖ProtoData.dll生成MsgTypeTypeMap.cs文件
GenerateMsgTypeTypeMap
::cs文件生成dll文件
csc /target:library /reference:Google.Protobuf.dll /out:ProtoData.dll *.cs 


::拷贝生成的文件到客户端服务器目录
echo copy ProtoData.dll to client
xcopy ProtoData.dll ..\SimpleChat\Assets\Plugins /y/d/q
echo copy ProtoData.dll to server
xcopy ProtoData.dll ..\SimpleChatServer\SimpleChatServer\Reference /y/d/q


::移动生成的文件到tmp目录，tmp目录不需要放到git上
move /y *.cs tmp
move /y ProtoData.dll tmp

pause