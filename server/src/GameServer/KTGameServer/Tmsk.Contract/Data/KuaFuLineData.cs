using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Tmsk.Contract
{
    /// <summary>
    /// 跨服线路信息
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class KuaFuLineData
    {
        /// <summary>
        /// Line ID
        /// </summary>
        [ProtoMember(1)]
        public int Line;

        /// <summary>
        /// 状态
        /// </summary>
        [ProtoMember(2)]
        public int State;

        /// <summary>
        /// 在线人数
        /// </summary>
        [ProtoMember(3)]
        public int OnlineCount;
        
        /// <summary>
        /// 最大在线人数
        /// </summary>
        [ProtoMember(4)]
        public int MaxOnlineCount;

        /// <summary>
        /// 服务器 ID
        /// </summary>
        [ProtoMember(5)]
        public int ServerId;

        /// <summary>
        /// Map code
        /// </summary>
        [ProtoMember(6)]
        public int MapCode;
    }
}
