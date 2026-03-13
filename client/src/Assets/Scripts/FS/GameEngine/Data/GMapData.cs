﻿//#define USE_AS3_COMPAITABLE_ASTAR

using System;
using System.Net;
using FS.Drawing;
using System.Collections.Generic;
using Server.Tools.AStarEx;
using FS.GameEngine.Logic;
using UnityEngine;
using FS.VLTK.Entities.Config;

namespace FS.GameEngine.Data
{
    /// <summary>
    /// 地图数据
    /// </summary>
    public class GMapData : IDisposable
    {
        /// <summary>
        /// 地图数据
        /// </summary>
        public GMapData()
        {
        }

        #region 地图的组成部分
        /// <summary>
        /// 当前地图大小（以正方形为单位，不是实际大小）
        /// </summary>
        public Vector2 MapSize
        {
            get
            {
                return new Vector2(this.MapWidth, this.MapHeight);
            }
        }

        /// <summary>
        /// 地图的实际尺寸
        /// </summary>
        public Vector2 RealMapSize { get; set; }

        /// <summary>
        /// NPC 列表将出现在小地图上
        /// </summary>
        public List<KeyValuePair<string, Point>> MinimapNPCList { get; set; }

        /// <summary>
        /// 怪物列表将出现在小地图上
        /// </summary>
        public List<KeyValuePair<string, Point>> MinimapMonsterList { get; set; }

        /// <summary>
        /// 收集点列表将出现在小地图上
        /// </summary>
        public List<KeyValuePair<string, Point>> MinimapGrowPointList { get; set; }

        /// <summary>
        /// 传输点列表
        /// </summary>
        public List<KeyValuePair<string, Point>> Teleport { get; set; }

        /// <summary>
        /// NPC 列表按名称显示在小地图上
        /// </summary>
        public List<KeyValuePair<string, Point>> NpcList { get; set; }

        /// <summary>
        /// NPC 列表按 ID 显示在小地图上
        /// </summary>
        public Dictionary<int, List<Point>> NpcListByID { get; set; }

        /// <summary>
        /// 小地图上出现怪物列表
        /// </summary>
        public List<KeyValuePair<string, Point>> MonsterList { get; set; }

        /// <summary>
        /// 怪物列表按 ID 显示在小地图上
        /// </summary>
        public Dictionary<int, List<Point>> MonsterListByID { get; set; }

        /// <summary>
        /// 小地图上会显示收集点列表
        /// </summary>
        public List<KeyValuePair<string, Point>> GrowPointList { get; set; }

        /// <summary>
        /// 收集点列表按 ID 显示在小地图上
        /// </summary>
        public Dictionary<int, List<Point>> GrowPointListByID { get; set; }

        /// <summary>
        /// 区域列表出现在小地图上
        /// </summary>
        public List<KeyValuePair<string, Point>> Zone { get; set; }

        /// <summary>
        /// 设置地图
        /// </summary>
        public MapSetting Setting { get; set; }

        /// <summary>
        /// 地图宽度
        /// </summary>
        public int MapWidth { get; set; }

        /// <summary>
        /// 地图高度
        /// </summary>
        public int MapHeight { get; set; }

        /// <summary>
        /// 网孔尺寸X（POT）
        /// </summary>
        public int GridSizeX { get; set; } = 10;

        /// <summary>
        /// Y网格尺寸（POT）
        /// </summary>
        public int GridSizeY { get; set; } = 10;

        /// <summary>
        /// 水平网格单元总数
        /// </summary>
        public int GridSizeXNum { get; set; } = 0;

        /// <summary>
        /// 垂直网格单元总数
        /// </summary>
        public int GridSizeYNum { get; set; } = 0;

        /// <summary>
        /// 原始X网格尺寸
        /// </summary>
        public int OriginGridSizeXNum { get; set; }

        /// <summary>
        /// 原始Y网格尺寸
        /// </summary>
        public int OriginGridSizeYNum { get; set; }

        /// <summary>
        /// 无法访问块区域
        /// </summary>
        public byte[,] Obstructions { get; set; }

        /// <summary>
        /// 模糊区域
        /// </summary>
        public byte[,] BlurPositions { get; set; }

        /// <summary>
        /// 动态块区域可任意打开和关闭
        /// </summary>
        public byte[,] DynamicObstructions { get; set; }

        /// <summary>
        /// 安全区
        /// </summary>
        public byte[,] SafeAreas { get; set; }

        /// <summary>
        /// 动态 Obs 标签列表已打开
        /// </summary>
        public HashSet<byte> OpenedDynamicObsLabels { get; } = new HashSet<byte>();

        #endregion

        /// <summary>
        /// 摧毁物体
        /// </summary>
        public void Dispose()
        {
            this.MinimapNPCList?.Clear();
            this.MinimapNPCList = null;
            this.MinimapMonsterList?.Clear();
            this.MinimapMonsterList = null;
            this.MinimapGrowPointList?.Clear();
            this.MinimapGrowPointList = null;
            this.Teleport?.Clear();
            this.Teleport = null;
            this.NpcList?.Clear();
            this.NpcList = null;
            this.NpcListByID?.Clear();
            this.NpcListByID = null;
            this.MonsterList?.Clear();
            this.MonsterList = null;
            this.MonsterListByID?.Clear();
            this.MonsterListByID = null;
            this.GrowPointList?.Clear();
            this.GrowPointList = null;
            this.GrowPointListByID?.Clear();
            this.GrowPointListByID = null;
            this.Zone?.Clear();
            this.Zone = null;
            this.Setting?.Dispose();
            this.Setting = null;
            this.Obstructions = null;
            this.BlurPositions = null;
            this.DynamicObstructions = null;
            this.OpenedDynamicObsLabels.Clear();
        }
    }
}
