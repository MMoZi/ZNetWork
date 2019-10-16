using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ZNetSvc
{
    class ZReceive
    {
        private byte[] headCache = null;
        private byte[] bodyCache = null;

        private int headLength = 4;
        private int bodyLength;
        private int currHeadIndex = 0;
        private int currBodyIndex = 0;

        public Socket conn = null;
        public string connectId = null;  
        private Action<string, System.Object> OnEndReceive = null;
        private Action<string> OnDisConnected = null;

        public ZReceive(Socket connSocket 
            , string connectId = null 
            , Action<string, System.Object> onEndReceive = null
            ,Action<string> onDisConnected = null)
        { 
            this.conn = connSocket;
            this.connectId = connectId; 
            this.OnEndReceive = onEndReceive;
            this.OnDisConnected = onDisConnected;
            initHeadCache();
        }

        public void SetOnEndReceive(Action<string, System.Object> onEndReceive) { this.OnEndReceive = onEndReceive; }
        public void SetOnDisConnected(Action<string> onDisConnected) { this.OnDisConnected = onDisConnected; }
        private void initHeadCache()
        {
            headCache = new byte[headLength];
        }
        private void initBodyCache()
        {
            bodyCache = new byte[bodyLength];
        }
        private void clearCache()
        {
            headCache = null;
            bodyCache = null;
            currHeadIndex = 0;
            currBodyIndex = 0;
        }
        private void Close()
        { 
            OnDisConnected?.Invoke(connectId);
            conn?.Dispose();
            conn?.Close();
            conn = null;
        }
        public void StartReceive( )
        { 
            try
            {
                conn.BeginReceive(headCache, 0, headLength, SocketFlags.None
                                , new AsyncCallback(ReceiveHead),null);
            }
            catch (System.Exception )
            {
                ZLogger.Debug(" ZReceive 001:用户" + connectId + " 连接已断开", LogType.Warning);
                Close();
            }
        }

        private void ReceiveHead(IAsyncResult ar)
        {
            try
            { 
                int len = conn.EndReceive(ar);
                if (len > 0)
                {
                    currHeadIndex += len;
                    if (currHeadIndex < headLength)
                    {
                        conn.BeginReceive(headCache, currHeadIndex, headLength - currHeadIndex, SocketFlags.None
                        , new AsyncCallback(ReceiveHead), null);
                    }
                    else
                    {
                        bodyLength = ZNetConfig.isLittleEndian ? ZTool.ConvertToIntLittleEndian(headCache)
                            : ZTool.ConvertToIntBigEndian(headCache);
                        initBodyCache();
                        conn.BeginReceive(bodyCache, 0,bodyLength, SocketFlags.None
                                , new AsyncCallback(ReceiveBody), null);
                    }
                }
                else
                {
                    ZLogger.Debug(" ZReceive 002:用户" + connectId + " 连接已断开", LogType.Warning);
                    Close();
                }
            }
            catch (System.Exception ex)
            {
                ZLogger.Debug(" ZReceive 003:用户" + connectId + " 连接已断开", LogType.Warning);
                Close();
            }
        }
        private void ReceiveBody(IAsyncResult ar)
        {

            try
            { 
                int len = conn.EndReceive(ar);
                if (len > 0)
                {
                    currBodyIndex += len;
                    if (currBodyIndex < headLength)
                    {
                        conn.BeginReceive(bodyCache, currBodyIndex, bodyLength - currBodyIndex, SocketFlags.None
                        , new AsyncCallback(ReceiveBody), null);
                    }
                    else
                    {
                        string str = ZNetConfig.encoding.GetString(bodyCache);
                        System.Object obj = JsonConvert.DeserializeObject(str);

                        OnEndReceive?.Invoke(connectId, obj);

                        clearCache();
                        initHeadCache();
                        conn.BeginReceive(headCache, 0, headLength, SocketFlags.None
                                , new AsyncCallback(ReceiveHead), null);
                    }
                }
                else
                {
                    ZLogger.Debug(" ZReceive 003:用户" + connectId + " 连接已断开", LogType.Warning);
                    Close();
                }
            }
            catch (System.Exception ex)
            {
                ZLogger.Debug(" ZReceive 004:用户" + connectId + " 连接已断开", LogType.Warning);
                Close();
            }
        }
    }
}
