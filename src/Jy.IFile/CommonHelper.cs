using System;
using System.Linq;
using System.Collections.Generic;

using System.IO;
using System.Text;
using System.Drawing;
using Jy.Utility;

namespace Jy.IFile
{
    /// <summary>
    /// 
    /// </summary>
    public static class CommonHelper
    {
        #region 生成随机数
        public static int CreateRandomCode(int codeCount)
        {
            var allCharArray = "0,1,2,3,4,5,6,7,8,9".SpanSplit(",");
            StringBuilder randomCode = new StringBuilder("0");
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(allCharArray.Count);
                if (temp == t)
                {
                    return CreateRandomCode(codeCount);
                }
                temp = t;
                randomCode.Append(allCharArray[t]);
            }
            return Convert.ToInt32(randomCode);
        }

        #endregion

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        public static DateTime? ToNewTime(long unixTime)
        {
             return FromUnixTime(unixTime).ToLocalTime();
        }

        /// <summary>
        /// 1、将 Stream 转成 byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ObjectStreamToBytes(Stream stream)
        {
            using (MemoryStream memstream = new MemoryStream())
            {
                var bys = new byte[1024];
                int len = 0;
                while ((len = stream.Read(bys, 0, 1024)) != 0)
                {
                    memstream.Write(bys, 0, len);
                }
                return memstream.ToArray();
            }
        }
        // <summary> 
        /// 3、字节流转换成图片 
        /// </summary> 
        /// <param name="byt">要转换的字节流</param> 
        /// <returns>转换得到的Image对象</returns> 
        public static Image BytToImg(byte[] byt)
        {
            using (MemoryStream ms = new MemoryStream(byt))
            {
                Image img = Image.FromStream(ms);
                return img;
            }
        }
        private static Dictionary<string, string> contentTypeDescs;
        private static void InitContentTypeDescs()
        {
            contentTypeDescs = new Dictionary<string, string>() {
            { "gif", "image/gif" },{ "jpg", "image/jpeg" },{ "png", "image/png" },{ "avi", "video/x-msvideo" }
            ,{ "au", "audio/basic" },{ "mp3", "audio/mpeg" },{ "mp4", "video/mp4" },{ "mpg", "video/mpeg" }
            ,{ "mpeg", "video/mpeg" },{ "rm", "audio/x-pn-realaudio" },{ "ra", "audio/x-pn-realaudio" },{ "ram", "audio/x-pn-realaudio" }
            ,{ "mid", "audio/x-midi" },{ "midi", "audio/x-midi" },{ "gz", "application/x-gzip" },{ "tar", "application/x-tar" }
            ,{ "rar", "application/x-rar" } ,{ "apk", "application/octet-stream" }
            };
        }
        public static string GetContentType(string ContentType)
        {
            if (contentTypeDescs == null)
            {
                InitContentTypeDescs();
            }
            string fType;
            if (contentTypeDescs.ContainsKey(ContentType))
                fType = contentTypeDescs[ContentType];
            else
                fType = "application/octet-stream";
            return fType;
        }

        public static string GetIMGContentType(string ContentType)
        {
            string fType = "";
            switch (ContentType.ToLower())
            {
                case "gif":
                    fType = "image/gif";
                    break;
                case "jpg":
                    fType = "image/jpeg";
                    break;
                case "jpeg":
                    fType = "image/jpeg";
                    break;
                case "png":
                    fType = "image/png";
                    break;
                default:
                    fType = "image/jpeg";
                    break;
            }
            return fType;
        }

        public static string GetFileNameExt(ReadOnlySpan<char> uploadFileName)
        {
            string contentType = "jpg";
            if (!uploadFileName.IsEmpty && uploadFileName.LastIndexOf(".") != -1)
            {
                contentType = uploadFileName.Slice(uploadFileName.LastIndexOf(".") + 1).ToString();
            }
            return contentType;
        }
        /**
        * 获取byte的实际长度
        * @param bytes
        * @return
        */
        public static int GetValidLength(byte[] bytes)
        {
            int i = 0;
            if (null == bytes || 0 == bytes.Length)
                return i;
            for (; i < bytes.Length; i++)
            {
                if ((bytes[i] == '\0') && (i + 1 < bytes.Length && bytes[i + 1] == '\0'))
                    break;
            }
            return i + 1;
        }
        /**
         * 获取byte的实际值
         * @param bytes
         * @return 实际长度的byte[]
         */
        public static byte[] GetCopyByte(byte[] bytes)
        {
            if (null == bytes || 0 == bytes.Length)
                return new byte[1];
            int length = GetValidLength(bytes);
            if (length == bytes.Length)
                return bytes;
            byte[] res = new byte[length];
            Array.Copy(bytes, 0, res, 0, length);
            return res;
        }

    }
}