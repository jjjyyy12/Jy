﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.ConsumerAuth.Middleware
{
    public class ConsLogOptions
    {
        public string LogPath { get; set; } = @"D:\LogFiles_MQ";

        public string PathFormat { get; set; }

        public static void EnsurePreConditions(ConsLogOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrWhiteSpace(options.LogPath))
            {
                throw new ArgumentException("系统日志文件存储路径未配置", nameof(options.LogPath));
            }

            if (string.IsNullOrWhiteSpace(options.PathFormat))
            {
                throw new ArgumentException("系统日志文件名称未配置", nameof(options.PathFormat));
            }

            if (!Directory.Exists(options.LogPath))
            {
                Directory.CreateDirectory(options.LogPath);
            }
        }
    }
}
