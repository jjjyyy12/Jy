using Jy.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.IFile
{
    public static class PathHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            StringBuilder path = new StringBuilder();
            foreach (var item in paths)
            {
                path.Append(item).Append("/");
            }
            var resPath = path.ToString();
            var sresPath = resPath.AsSpan();
            if (path.Length > 0)
                return sresPath.Slice(0, sresPath.LastIndexOf('/')).ToString();
            return resPath;
        }

        public static string GetSpliter(StorageTypeEnum storageType)
        {
            string spliti = "";
            switch (storageType)
            {
                case StorageTypeEnum.DISK:
                    spliti = @"\";
                    break;
                case StorageTypeEnum.OSS:
                    spliti = "/";
                    break;
                case StorageTypeEnum.LINUXDISK:
                    spliti = "/";
                    break;
                default:
                    spliti = "/";
                    break;
            }
            return spliti;
        }
        public static string GetUploadFilePath(string rootPath, string folderName, string fileName, StorageTypeEnum storageType)
        {
            string spliter = GetSpliter(storageType);
            return rootPath + spliter + folderName + spliter + fileName;
        }
        public static string ReWriteFileName(string fileName, string appId)
        {
            string result;
            if (string.IsNullOrEmpty(appId))
            {
                result = Guid.NewGuid().ToString() + "_" + fileName;
            }
            else
            {
                result = string.Format("{0}_{1}_{2}", appId, CommonHelper.GetTimeStamp(), fileName);
            }
            return result;
        }
        public static string ReplaceUnUseChar(string origional)
        {
            return origional.Trim('"').SpanReplace("\\/", "/").SpanReplace( "\\", "/");
        }
        
    }
}
