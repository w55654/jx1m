using ProtoBuf;
using System.Collections.Generic;

namespace Server.Data
{
    [ProtoContract]
    public class TerritoryInfo
    {
        [ProtoMember(1)]
        public int TerritoryCount { get; set; }

        [ProtoMember(2)]
        public List<Territory> Territorys { get; set; }

    }

    [ProtoContract]
    public class Territory
    {
        /// <summary>
        /// ID
        /// </summary>
        ///
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary>
        /// MapID
        /// </summary>
        ///
        [ProtoMember(2)]
        public int MapID { get; set; }

        /// <summary>
        /// MapNAME
        /// </summary>
        ///
        [ProtoMember(3)]
        public string MapName { get; set; }

        /// <summary>
        /// ID BANG
        /// </summary>
        ///
        [ProtoMember(4)]
        public int GuildID { get; set; }

        /// <summary>
        /// 领土星级
        /// </summary>
        ///
        [ProtoMember(5)]
        public int Star { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        ///
        [ProtoMember(6)]
        public int Tax { get; set; }

        /// <summary>
        /// 区域 ID
        /// </summary>
        ///
        [ProtoMember(7)]
        public int ZoneID { get; set; }

        /// <summary>
        /// 是否主城
        /// </summary>
        ///
        [ProtoMember(8)]
        public int IsMainCity { get; set; }

        [ProtoMember(9)]
        public string GuildName { get; set; }
    }
}
