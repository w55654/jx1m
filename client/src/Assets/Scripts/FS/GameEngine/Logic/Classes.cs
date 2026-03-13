﻿using System.Collections.Generic;
using Server.Data;

namespace FS.GameEngine.Logic
{
    /// <summary>
    /// 服务器数据
    /// </summary>
    public class XuanFuServerData
    {
        /// <summary>
        /// 提名服务器
        /// </summary>
        public BuffServerInfo RecommendServer { get; set; }
        /// <summary>
        /// 最后一个服务器
        /// </summary>
        public BuffServerInfo LastServer { get; set; }
        /// <summary>
        /// 输入的服务器列表
        /// </summary>
        public List<BuffServerInfo> RecordServerInfos { get; set; }
        /// <summary>
        /// 推荐服务器列表
        /// </summary>
        public List<BuffServerInfo> RecommendServerInfos { get; set; }
        /// <summary>
        /// 服务器列表
        /// </summary>
        public List<BuffServerInfo> ServerInfos { get; set; }
    }
}
