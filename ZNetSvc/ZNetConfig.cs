using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ZNetSvc
{
    public class ZNetConfig
    {
        protected Socket socket;
        
        public static int MaxMessageSize = 64 * 1024;//64KB
        public static bool isLittleEndian = true;
        public static Encoding encoding = Encoding.UTF8;


        protected bool IsConnected (Socket conn){ 
                return conn != null && conn.Connected; 
        }

        protected void Close(Socket conn) {

            conn?.Dispose();
            conn?.Close();
        } 
 

    }
}
