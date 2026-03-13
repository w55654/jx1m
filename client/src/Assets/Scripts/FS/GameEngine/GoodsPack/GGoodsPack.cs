﻿using FS.Drawing;
using UnityEngine;
using FS.GameEngine.Logic;
using FS.GameEngine.Interface;
using FS.VLTK.Control.Component;
using FS.VLTK;
using static FS.VLTK.Entities.Enum;
using FS.VLTK.Factory;

namespace FS.GameEngine.GoodsPack
{
    /// <summary>
    /// 地图上掉落物品的对象
    /// </summary>
    public class GGoodsPack : IObject
    {
        /// <summary>
        /// 最大生存时间
        /// </summary>
        public const int GoodsPackKeepTimes = 60000;

        #region 初始化对象

        /// <summary>
        /// 初始化对象
        /// </summary>
        public GGoodsPack()
        {
        }

        #endregion

        #region 2D Objects
        /// <summary>
        /// Component Item
        /// </summary>
        public Item ComponentItem { get; private set; } = null;
        #endregion

        #region 继承IObject
        /// <summary>
        /// BaseID
        /// </summary>
        public int BaseID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 初始状态
        /// </summary>
        private bool _InitStatus = false;
        /// <summary>
        /// 初始状态
        /// </summary>
        public bool InitStatus
        {
            get { return _InitStatus; }
        }

        /// <summary>
        /// 2D 游戏对象对象
        /// </summary>
        public GameObject Role2D { get; set; }

        /// <summary>
        /// 当前位置（真实坐标）
        /// </summary>
        public Point Coordinate
        {
            get { return new Point(this.PosX, this.PosY); }
            set
            {
                this.PosX = value.X;
                this.PosY = value.Y;

                this.ApplyXYPos();
            }
        }

        private int _PosX = 0;

        /// <summary>
        /// 实际坐标X
        /// </summary>
        public int PosX
        {
            get { return this._PosX; }
            set
            {
                this._PosX = value;
                this.ApplyXYPos();
            }
        }

        private int _PosY = 0;

        /// <summary>
        /// 实际Y坐标
        /// </summary>
        public int PosY
        {
            get { return this._PosY; }
            set
            {
                this._PosY = value;
                this.ApplyXYPos();
            }
        }

        /// <summary>
        /// 更新 XY 坐标
        /// </summary>
        private void ApplyXYPos()
        {
            if (null != this.Role2D)
            {
                this.Role2D.transform.localPosition = new Vector3(this._PosX, this._PosY);
            }
        }

        /// <summary>
        /// 以 UnityEngine.Vector2 形式返回对象的坐标
        /// </summary>
        public Vector2 PositionInVector2
        {
            get
            {
                return new Vector2(this.PosX, this.PosY);
            }
        }

        #endregion

        #region 继承ISprite

        /// <summary>
        /// 对象类型
        /// </summary>
        public GSpriteTypes SpriteType
        {
            get;
            set;
        }

        /// <summary>
        /// 移动
        /// </summary>
        public KE_NPC_DOING CurrentAction { get; set; } 

        /// <summary>
        /// 旋转方向（8方向）
        /// </summary>
        public Direction Direction { get; set; }

        /// <summary>
        /// 运行速度
        /// </summary>
        public int MoveSpeed { get; set; }

        #endregion


        #region 继承IObject-显示

        /// <summary>
        /// 已经开始了吗？
        /// </summary>
        private bool _Started = false;

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            /// 如果已经开始
            if (this._Started)
            {
                return;
            }

            /// 标记已开始
            this._Started = true;
            /// 标记已初始化
            this._InitStatus = true;

            /// 添加成分
            this.ComponentItem = this.Role2D.GetComponent<Item>();
            /// 标记开始时间
            this.StartTick = KTGlobal.GetCurrentTimeMilis();

            /// 将对象添加到管理列表
            KTObjectsManager.Instance.AddObject(this);
        }

        /// <summary>
        /// 摧毁物体
        /// </summary>
        public void Destroy()
        {
            /// 删除对象
            KTObjectsManager.Instance.RemoveObject(this);

            /// 如果对象存在
            if (null != Role2D)
            {
                /// 执行取消
                this.ComponentItem.Destroy();
                this.Role2D = null;
            }
        }

        #endregion 

        #region 继承IObject - 渲染

        /// <summary>
        /// 该函数连续调用每个Frame，类似于Update函数
        /// </summary>
        public void OnFrameRender()
        {
            /// 如果你还没有开始
            if (!this._Started)
            {
                return;
            }

            /// 如果时间耗尽，该物品将被销毁
            if (this.StartTick > 0 && KTGlobal.GetCurrentTimeMilis() - this.StartTick >= GGoodsPack.GoodsPackKeepTimes - this.LifeTimeTicks)
            {
                /// 摧毁物体
                KTGlobal.RemoveObject(this, true);
                return;
            }
        }

        #endregion

        #region 属性
        /// <summary>
        /// 这一刻被创造了
        /// </summary>
        private long StartTick = 0;

        /// <summary>
        /// 时间存在
        /// </summary>
        public long LifeTimeTicks { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int GoodsID { get; set; }

        /// <summary>
        /// 星星数量
        /// </summary>
        public int Stars { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int LinesCount { get; set; }

        /// <summary>
        /// 强化等级
        /// </summary>
        public int EnhanceLevel { get; set; }
        #endregion
    }
}
