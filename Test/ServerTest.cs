

using System;
using ZNetSvc;

namespace Test
{
    class ServerTest
    {
        static Server server;
        static void Main(string[] args)
        {

            server = new Server("127.0.0.1", 6688,
            onEndReceive: (id, obj) =>
            {

                Console.WriteLine("用户 : " + id);
                Console.WriteLine(obj.ToString()); 
            }
            , onEndSend: (id) =>
            {

                Console.WriteLine("用户 : " + id + "  发送完毕");
            }
            , onConnected: (id) =>
            {

                Console.WriteLine("用户: " + id + "  连接成功");
            }
            , onDisConnected: (id) =>
            {

                Console.WriteLine("用户: " + id + "  断开连接");
            });

            server.Start();

            while (true) {


            }

        }
    }
}
