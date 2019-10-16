using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ZNetSvc
{
    public class Server :ZNetConfig
    {
        private string m_IP;
        private int m_Port;
        ZSend zSend;


        /// <summary>
        /// 客户端管理
        /// </summary>
        private ConcurrentDictionary<string, Socket> clients = new ConcurrentDictionary<string, Socket>();


        #region CallBack

        /// <summary>
        /// 当接收完信息时
        /// </summary>
        protected Action<string, System.Object> OnEndReceive = null;
        /// <summary>
        /// 开始连接时
        /// </summary>
        protected Action<string> OnConnected = null;
        /// <summary>
        /// 断开连接时
        /// </summary>
        protected Action<string> OnDisConnected = null;
        /// <summary>
        /// 当发送结束的时候
        /// </summary>
        protected Action<string> OnEndSend = null;

        #endregion

        public Server(
            string ip ,
            int port,
            bool misLittleEndian = true
            , Action<string,System.Object> onEndReceive = null
            ,Action<string> onEndSend = null
            ,Action<string> onConnected = null
            ,Action<string> onDisConnected = null)
        {
            this.m_IP = ip;
            this.m_Port = port;
            this.OnEndReceive = onEndReceive;
            this.OnEndSend = onEndSend;
            this.OnConnected = onConnected;
            this.OnDisConnected = onDisConnected;

            isLittleEndian = misLittleEndian;

            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            zSend = new ZSend(onEndSend:onEndSend);
        }

        public void SetEncoding(Encoding mencoding)
        {
            encoding = mencoding;
        }
        /// <summary>
        /// 开始一个服务器
        /// </summary>
        /// <param name="backlog"></param>
        public void Start(int backlog = 10)
        {
            socket.Bind(new IPEndPoint(IPAddress.Parse(m_IP), m_Port));
            socket.Listen(backlog);
            socket.BeginAccept(AcceptCallBack, null);
            ZLogger.Debug("     服务器初始化成功！   ");
        }
        
        private void AcceptCallBack(IAsyncResult ar)
        {

            Socket client = socket.EndAccept(ar); 
            string connId = Guid.NewGuid().ToString();

            OnConnected.Invoke(connId); //连接提示

            clients.TryAdd(connId,client); //添加客户

            ZReceive zr = new ZReceive(
                connSocket: client
                , connectId: connId
                , onEndReceive: OnEndReceive
                , onDisConnected: (str) =>
                {
                    clients.TryRemove(connId, out Socket _);
                    OnDisConnected?.Invoke(str);
                });

            zr.StartReceive(); //开始接收

            socket.BeginAccept(AcceptCallBack, null);
        }

        public void Send(string connectedId,object obj) {
            Socket clientSocket = null;

            if (!clients.TryGetValue(connectedId, out clientSocket)) {
                ZLogger.Debug("ZServer 001 : 发送对象无效，停止发送！", LogType.Error);
                return;
            }  

            zSend.SetOnDisConnected((str) =>
            {
                clients.TryRemove(connectedId, out Socket _);
                OnDisConnected?.Invoke(str);
            });
            zSend.Send(clientSocket,obj,connectedId);
        }

        public void BorderCast(string srcid,object obj)
        {

        }
        public void BorderCast(object obj) {


        }
    }
}
