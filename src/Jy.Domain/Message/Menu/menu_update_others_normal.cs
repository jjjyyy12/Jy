﻿using Jy.Domain.Dtos;
using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Domain.Message
{
    public class menu_update_others_normal : MessageBase
    {
        private string _currentDBStr;
        public string CurrentDBStr { get; set; }
        public menu_update_others_normal(string exchangeName, MenuDto dto, string currentDBStr)
        {
            base.MessageBodyByte = ByteConvertHelper.Object2Bytes(dto);
            base.exchangeName = exchangeName;
            base.MessageRouter = this.GetType().Name;
            _currentDBStr = currentDBStr;
        }
    }
}
