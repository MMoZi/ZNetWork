using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNetSvc
{
    enum LogType {
        Log,
        Warning,
        Error
    }
    class ZLogger
    {
        public static bool IsOutput = true;

        public static void Debug(string log, LogType logType = LogType.Log) {
            if (!IsOutput) return;
            switch (logType)
            {
                case LogType.Log:
                    Console.Write("<-- Log --> :");
                    Console.WriteLine(log);
                    break;
                case LogType.Warning:
                    Console.WriteLine("<-- Warning --> :");
                    Console.WriteLine(log);
                    break;
                case LogType.Error:
                    Console.Write("<-- Error --> :");
                    Console.WriteLine(log);
                    break;
                default:
                    break;
            }
        }
    }
}
