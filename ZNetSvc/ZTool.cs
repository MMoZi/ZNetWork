using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNetSvc
{
    public static class ZTool
    {
        public static byte[] ConvertBytesBigEndian(int value) {
            return new byte[] {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value
            };
        }
        public static int ConvertToIntBigEndian(byte[] bytes)
        {
            return
                (bytes[0] << 24) |
                (bytes[1] << 16) |
                (bytes[2] << 8) |
                bytes[3]; 
        }
        public static byte[] ConvertBytesLittleEndian(int value) {
            return new byte[] {
                (byte)value,
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24)
            };
        }
        public static int ConvertToIntLittleEndian(byte[] bytes)
        {
            return
                (bytes[3] << 24) |
                (bytes[2] << 16) |
                (bytes[1] << 8) |
                bytes[0];
        }


        /// <summary>
        /// 头部长度加在数据前面
        /// </summary>
        /// <param name="data"></param>
        /// <param name="IsLittleEndian"></param>
        /// <returns></returns>
        public static byte[] Pack(byte[] data, bool IsLittleEndian = true)
        {
            int length = data.Length;
            byte[] head = IsLittleEndian ? ZTool.ConvertBytesLittleEndian(length) : ZTool.ConvertBytesBigEndian(length);
            byte[] temp = new byte[data.Length + 4];
            head.CopyTo(temp, 0);
            data.CopyTo(temp, 4);
            return temp;
        }



    }
}
