# ZNetWork
废稿，有待修改

## 服务器端

 
    class ServerTest
    {
        static Server server;
        static void Main(string[] args)
        {

            server = new Server("127.0.0.1", 6688,
                  onEndReceive: (id, obj) =>
                  {

                      Console.WriteLine("用户 : " + id);
                      Console.WriteLine(obj.ToString()); //处理接收信息
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

            server.Start(); //开始接收

            while (true) {


            }

        }
    }
 

## 客户端


    class ClientTest
    {
        static Client client;
        static void Main(string[] args)
        {

            client = new Client("127.0.0.1",6688,
                onEndReceive:(obj)=> {

                    Console.WriteLine("服务器 ： ");
                    Console.WriteLine(obj.ToString()); //处理接收信息

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
            client.Start();  //开始接收
            while (true) {

                Console.WriteLine("用户 : ");
                string str = Console.ReadLine();
                client.Send(str);
            }
        }
    }
