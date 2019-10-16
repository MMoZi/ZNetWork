using System;
using System.Net;
using System.Net.Sockets; 

namespace ZNetSvc
{
    public class Client : ZNetConfig
    {

        private string m_IP;
        private int m_Port; 
        ZSend zSend;


        #region CallBack

        /// <summary>
        /// 当接收完信息时
        /// </summary>
        protected Action<System.Object> OnEndReceive = null;
        /// <summary>
        /// 开始连接时
        /// </summary>
        protected Action OnConnected = null;
        /// <summary>
        /// 断开连接时
        /// </summary>
        protected Action OnDisConnected = null;
        /// <summary>
        /// 当发送结束的时候
        /// </summary>
        protected Action OnEndSend = null;

        #endregion

        public Client(
            string ip,
            int port,
            bool misLittleEndian = true
            , Action< System.Object> onEndReceive = null
            , Action onEndSend = null
            , Action onConnected = null
            , Action onDisConnected = null
            ) {

            this.m_IP = ip;
            this.m_Port = port;
            this.OnEndReceive = onEndReceive;
            this.OnEndSend = onEndSend;
            this.OnConnected = onConnected;
            this.OnDisConnected = onDisConnected;

            isLittleEndian = misLittleEndian;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            zSend = new ZSend(onEndSend: (_) => onEndSend?.Invoke()
                        , onDisConnected: (_) => onDisConnected?.Invoke());
        }

        public void Start() {

            try
            {
                socket.BeginConnect(IPAddress.Parse(m_IP), m_Port, (ar) =>
                {

                    try
                    {
                        socket.EndConnect(ar);
                    }
                    catch (System.ObjectDisposedException)
                    {
                        ZLogger.Debug(" ZClient 001: 连接已断开", LogType.Warning);
                        return;
                    }
                    catch (System.Net.Sockets.SocketException) {
                        ZLogger.Debug(" ZClient 002: 服务器未启动", LogType.Warning);
                        return;
                    }
                    OnConnected?.Invoke();
                    ZReceive zr = new ZReceive(
                        connSocket: socket
                        , onDisConnected: (_) => OnDisConnected?.Invoke()
                        , onEndReceive: (_, obj) => OnEndReceive?.Invoke(obj)
                        );
                    zr.StartReceive();

                }, null);
            } 
            catch (System.Exception)
            {
                ZLogger.Debug(" ZClient 003: 连接已断开", LogType.Warning);
                return;
            }

            ZLogger.Debug("     客户端初始化成功！   ");
        }

        public void Send(object obj) {

            zSend.Send(socket,obj);

        }
 

    }
}
