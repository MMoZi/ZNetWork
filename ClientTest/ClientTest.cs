using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNetSvc;
namespace ClientTest
{
    class ClientTest
    {
        static Client client;
        static void Main(string[] args)
        {

            client = new Client("127.0.0.1",6688,
                onEndReceive:(obj)=> {

                    Console.WriteLine("服务器 ： ");
                    Console.WriteLine(obj.ToString());

                }
                ,onEndSend:()=> {
                    Console.WriteLine("发送完毕");
                }
                ,onConnected:()=> {
                    Console.WriteLine("连接成功");
                }
                ,onDisConnected:()=> {

                    Console.WriteLine("断开连接");
                });
            client.Start(); 
            while (true) {

                Console.WriteLine("用户 : ");
                string str = Console.ReadLine();
                client.Send(str);
            }
        }
    }
}
