﻿using FS.Drawing;
using UnityEngine;
using FS.GameEngine.Logic;
using static FS.VLTK.Entities.Enum;

namespace FS.GameEngine.Interface
{
    /// <summary>
    /// 地图中的对象
    /// </summary>
	public interface IObject
    {
        /// <summary>
        /// 对象的BaseID，用于区别于其他对象
        /// </summary>
        int BaseID { get; set; }

        /// <summary>
        /// 对象名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 已经初始化了吗？
        /// </summary>
        bool InitStatus { get; }

        /// <summary>
        /// 控制 2D 角色的脚本
        /// </summary>
        GameObject Role2D { get; set; }

        /// <summary>
        /// 坐标
        /// </summary>
        Point Coordinate { set; get; }

        /// <summary>
        /// 实际坐标X
        /// </summary>
        int PosX { get; set; }

        /// <summary>
        /// 实际Y坐标
        /// </summary>
        int PosY { get; set; }

        /// <summary>
        /// 初始化对象
        /// </summary>
        void Start();

        /// <summary>
        /// 删除对象
        /// </summary>
        void Destroy();

        /// <summary>
        /// 该函数与Update函数类似，不断调用
        /// </summary>
        /// <param name="time"></param>
        void OnFrameRender();

        /// <summary>
        /// 对象类型
        /// </summary>
        GSpriteTypes SpriteType { get; set; }

        /// <summary>
        /// 当前动向
        /// </summary>
        KE_NPC_DOING CurrentAction { get; }

        /// <summary>
        /// 当前旋转方向
        /// </summary>
        Direction Direction { get; set; }
    }
}
