using System;
using System.Drawing;
using System.IO;

namespace Jy.IFile
{
    public interface IStorageService
    {
        FileSystemInfo CreateDirectory(string path);
        /// <summary>
        /// 删除文件夹路径
        /// </summary>
        /// <param name="path"></param>
        void DeleteDir(string path);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        void DeleteFile(string path);

        /// <summary>
        /// 确定给定路径是否引用磁盘上的现有目录。
        /// </summary>
        /// <param name="path">要测试的路径。</param>
        /// <returns>如果 path 引用现有目录，则为 true；否则为 false。</returns>
        bool ExistsDir(string path);

        /// <summary>
        /// 确定指定的文件是否存在。
        /// </summary>
        /// <param name="path">要检查的文件。</param>
        /// <returns>如果调用方具有要求的权限并且 path 包含现有文件的名称，则为 true；否则为 false。如果 path 为 null、无效路径或零长度字符串，则此方法也将返回
        /// false。如果调用方不具有读取指定文件所需的足够权限，则不引发异常并且该方法返回 false，这与 path 是否存在无关。</returns>
        bool ExistsFile(string path);


        /// <summary>
        /// copy
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destPath"></param>
        void CopyFile(string path, string destPath, bool overwrite);

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destPath"></param>
        void MoveFile(string path, string destPath);
        /// <summary>
        /// 读取image
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Image FromFile(string path);

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="path"></param>
        /// <param name="btArray"></param>
        void PutObjectFromBytes(string path, byte[] btArray, long psoition = 0);

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        byte[] GetObjectByte(string path, long position);

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        byte[] GetObjectByte(string path, long start, long end);

        /// <summary>
        /// 得到文件长度
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        long GetFileLength(string path);

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetFileName(string path);
        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        byte[] GetImageThumbnailBytes(string path, int width, int height);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        void UploadFile(string path, long startPosition, byte[] data);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="startPosition"></param>
        /// <param name="fileSize"></param>
        /// <param name="data"></param>
        void UploadFile(string path, long startPosition, long fileSize, byte[] data);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stream"></param>
        void UploadFile(string path, Stream stream);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="startPosition"></param>
        /// <param name="fileSize"></param>
        /// <param name="stream"></param>
        void UploadFile(string path, long startPosition, long fileSize, Stream stream);
    }
}
