using Newtonsoft.Json;
using System; 
using System.Net.Sockets;

namespace ZNetSvc
{
    class ZSend
    {
        private Action<string> OnEndSend = null;
        private Action<string> OnDisConnected = null; 

        public ZSend( Action<string> onEndSend = null
            , Action<string> onDisConnected = null) {

            this.OnEndSend = onEndSend;
            this.OnDisConnected = onDisConnected;
        }

        public void SetOnDisConnected(Action<string> onDisConnected) {
            this.OnDisConnected = onDisConnected;
        }
        public void SetOnEndSend(Action<string> onEndSend ) { this.OnEndSend = onEndSend; }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        public void Send(Socket conn, System.Object obj, string connectId = null)  
        {
             

            string datastr = null;
            try
            {
                datastr = JsonConvert.SerializeObject(obj);
            }
            catch (System.Exception ex)
            {
                ZLogger.Debug("ZSend 001：传输数据无法被序列化为Json:" + ex.ToString(), LogType.Error);
                ZLogger.Debug("停止发送本条消息！");
                return;
            }
    
            byte[] data = ZNetConfig.encoding.GetBytes(datastr);

            if (datastr == "\"\"")
            {
                ZLogger.Debug("ZSend 002：无法传递空消息:", LogType.Error);
                ZLogger.Debug("停止发送本条消息！");
                return;
            } else if (data.Length > ZNetConfig.MaxMessageSize) {
                ZLogger.Debug("ZSend 003: 单次发送消息过长", LogType.Error);
                ZLogger.Debug("停止发送本条消息！");
                return;
            }
            data = ZTool.Pack(data, ZNetConfig.isLittleEndian);

            try
            {
                conn.BeginSend(data, 0, data.Length, SocketFlags.None,
                    new AsyncCallback((IAsyncResult ar) =>
                    {
                        conn.EndSend(ar);
                        OnEndSend?.Invoke(connectId);
                    }), null);
            }
            catch (System.Exception ex)
            { 
                OnDisConnected?.Invoke(connectId); //关闭连接
                conn?.Dispose();
                conn?.Close();
                conn = null;
                ZLogger.Debug("ZSend 003: 用户：" + connectId + " 连接已经断开", LogType.Warning);
            }
        } 
    }
}
