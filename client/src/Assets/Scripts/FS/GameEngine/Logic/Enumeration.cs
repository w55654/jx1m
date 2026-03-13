﻿namespace FS.GameEngine.Logic
{
    /// <summary>
    /// 游戏中的事件
    /// </summary>
    public enum GameEvent
    {
        /// <summary>
        /// 移动收音机
        /// </summary>
        DynamicArena = 0,
        /// <summary>
        /// 秘密场景
        /// </summary>
        MiJing = 1,
        /// <summary>
        /// 军营
        /// </summary>
        MilitaryCamp = 2,
        /// <summary>
        /// 神秘的宝库
        /// </summary>
        ShenMiBaoKu = 3,
        /// <summary>
        /// 杜龙卡
        /// </summary>
        YouLong = 4,
        /// <summary>
        /// 武术连斗
        /// </summary>
        TeamBattle = 5,

        /// <summary>
        /// 金东
        /// </summary>
        SongJin = 20,
        /// <summary>
        /// 门派比赛
        /// </summary>
        FactionBattle = 21,
        /// <summary>
        /// 风火连青事件
        /// </summary>
        FengHuoLianCheng = 22,
        /// <summary>
        /// 巴赫胡阳
        /// </summary>
        BaiHuTang = 23,
        /// <summary>
        /// 贪狼
        /// </summary>
        EmperorTomb = 24,
    }

    /// <summary>
    /// 对象类型
    /// </summary>
    public enum GSpriteTypes
    {
        /// <summary>
        /// 没有
        /// </summary>
        None = -1,

        /// <summary>
        /// Leader
        /// </summary>
        Leader = 0,

        /// <summary>
        /// 其他玩家
        /// </summary>
        Other,

        /// <summary>
        /// 他妈的
        /// </summary>
        Monster,

        /// <summary>
        /// NPC
        /// </summary>
        NPC,

        /// <summary>
        /// 收集点
        /// </summary>
        GrowPoint,

        /// <summary>
        /// 动态区域
        /// </summary>
        DynamicArea,

        /// <summary>
        /// 传送门
        /// </summary>
        Teleport,

        /// <summary>
        /// 掉落物品
        /// </summary>
        GoodsPack,

        /// <summary>
        /// Bot
        /// </summary>
        Bot,

        /// <summary>
        /// Pet
        /// </summary>
        Pet,

        /// <summary>
        /// 辣椒车
        /// </summary>
        TraderCarriage,

        /// <summary>
        /// 销售机器人
        /// </summary>
        StallBot,
    }

    /// <summary>
    /// 项目操作类型
    /// </summary>
    public enum ModGoodsTypes
    {
        /// <summary>
        /// 丢弃
        /// </summary>
        Abandon = 0,

        /// <summary>
        /// 装备自己
        /// </summary>
        EquipLoad = 1,

        /// <summary>
        /// 移除设备
        /// </summary>
        EquipUnload = 2,

        /// <summary>
        /// 更改数量
        /// </summary>
        ModValue = 3,

        /// <summary>
        /// 删除项目
        /// </summary>
        Destroy = 4,

        /// <summary>
        /// 卖给NPC
        /// </summary>
        SaleToNpc = 5,

        /// <summary>
        /// 杯子
        /// </summary>
        SplitItem = 6,
    }

    /// <summary>
    /// 任务类型
    /// </summary>
    public enum TaskTypes
    {
        /// <summary>
        /// 没有
        /// </summary>
        None = -1,

        /// <summary>
        /// 与 NPC 交谈
        /// </summary>
        Talk = 0,

        /// <summary>
        /// 杀死怪物
        /// </summary>
        KillMonster = 1,

        /// <summary>
        /// 杀死怪物并拾取物品
        /// </summary>
        MonsterSomething = 2,

        /// <summary>
        /// 从商店购买商品
        /// </summary>
        BuySomething = 3,

        /// <summary>
        /// 使用物品
        /// </summary>
        UseSomething = 4,

        /// <summary>
        /// 向 NPC 运送物品
        /// </summary>
        TransferSomething = 5,

        /// <summary>
        /// 从 NPC 处获取物品
        /// </summary>
        GetSomething = 6,

        /// <summary>
        /// 从 NPC 处获取金钱
        /// </summary>
        Collect = 7,

        /// <summary>
        /// 回答问题
        /// </summary>
        AnswerQuest = 8,

        /// <summary>
        /// 进入宗门
        /// </summary>
        JoinFaction = 9,

        /// <summary>
        /// 做东西
        /// </summary>
        Crafting = 10,

        /// <summary>
        /// 强化
        /// </summary>
        Enhance = 11,

        /// <summary>
        /// 总共增强了多少次？
        /// </summary>
        EnhanceTime = 12,

        /// <summary>
        /// 参加某活动X次
        /// </summary>
        JoinActivity = 13,

        /// <summary>
        /// 参加活动
        /// </summary>
        JoinBattleSongJinEvent = 14,

        /// <summary>
        /// 杀戮
        /// </summary>
        KillOtherGuildRole = 15,

        /// <summary>
        /// 买东西
        /// </summary>
        BuyItemInShopGuild = 16,

        /// <summary>
        /// 搜索指定行的项目
        /// </summary>
        GetItemWithSpcecialLine = 17,

        /// <summary>
        /// 在特定地图上杀死其他人
        /// </summary>
        KillOtherGuildRoleTargetMapcode = 18,

        /// <summary>
        /// 花费多少次？
        /// </summary>
        CarriageTotalCount = 19,
    };

    /// <summary>
    /// NPC任务状态
    /// </summary>
    public enum NPCTaskStates
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None,

        /// <summary>
        /// 有主要任务需要接受
        /// </summary>
        ToReceive_MainQuest,

        /// <summary>
        /// 有主要任务需要付费
        /// </summary>
        ToReturn_MainQuest,

        /// <summary>
        /// 有支线任务可以接
        /// </summary>
        ToReceive_SubQuest,

        /// <summary>
        /// 有支线任务需要付费
        /// </summary>
        ToReturn_SubQuest,

        /// <summary>
        /// 有重复的任务可以接收
        /// </summary>
        ToReceive_DailyQuest,

        /// <summary>
        /// 有经常性的义务要支付
        /// </summary>
        ToReturn_DailyQuest,
    };

    /// <summary>
    /// 交易
    /// </summary>
    public enum GoodsExchangeCmds
    {
        /// <summary>
        /// 没有
        /// </summary>
        None = 0,

        /// <summary>
        /// 请求交易
        /// </summary>
        Request,

        /// <summary>
        /// 拒绝
        /// </summary>
        Refuse,

        /// <summary>
        /// 同意
        /// </summary>
        Agree,

        /// <summary>
        /// 取消交易
        /// </summary>
        Cancel,

        /// <summary>
        /// 将项目添加到会话
        /// </summary>
        AddGoods,

        /// <summary>
        /// 从会话中删除项目
        /// </summary>
        RemoveGoods,

        /// <summary>
        /// 加银
        /// </summary>
        UpdateMoney,

        /// <summary>
        /// 添加铜
        /// </summary>
        UpdateYuanBao,

        /// <summary>
        /// 宠物交易
        /// </summary>
        AddPet,

        /// <summary>
        /// 从会话中删除宠物
        /// </summary>
        RemovePet,

        /// <summary>
        /// 锁
        /// </summary>
        Lock,

        /// <summary>
        /// 开锁
        /// </summary>
        Unlock,

        /// <summary>
        /// 完成交易
        /// </summary>
        Done,
    }

    /// <summary>
    /// 采购实体
    /// </summary>
    public enum GoodsStallCmds
    {
        /// <summary>
        /// 没有
        /// </summary>
        None = 0,

        /// <summary>
        /// 要求开店
        /// </summary>
        Request,

        /// <summary>
        /// 开始销售
        /// </summary>
        Start,

        /// <summary>
        /// 取消销售
        /// </summary>
        Cancel,

        /// <summary>
        /// 将商品添加到商店
        /// </summary>
        AddGoods,

        /// <summary>
        /// 从库存中移除物品
        /// </summary>
        RemoveGoods,

        /// <summary>
        /// 更新店铺名称
        /// </summary>
        UpdateMessage,

        /// <summary>
        /// 查看某人的商店
        /// </summary>
        ShowStall,

        /// <summary>
        /// 从特定商店购买商品
        /// </summary>
        BuyGoods,
    }

    /// <summary>
    /// 怪物的种类
    /// </summary>
    public enum MonsterTypes
    {
        /// <summary>
        /// 通常，只有在受到伤害时，它们才会追逐
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 老板你真是个聪明人，每次看到人就追
        /// </summary>
        Elite = 1,

        /// <summary>
        /// 领导者
        /// </summary>
        Leader = 2,

        /// <summary>
        /// Boss
        /// </summary>
        Boss = 3,

        /// <summary>
        /// 海盗
        /// </summary>
        Pirate = 4,

        /// <summary>
        /// 吓坏了红字，自我追逐的人
        /// </summary>
        Hater = 5,

        /// <summary>
        /// 特殊怪物，自我追逐，无人时AI持续运行
        /// </summary>
        Special_Normal = 6,

        /// <summary>
        /// 特殊boss，无人时AI持续运行
        /// </summary>
        Special_Boss = 7,

        /// <summary>
        /// 安静的怪物，没有人工智能
        /// </summary>
        Static = 8,

        /// <summary>
        /// 安静的怪物，完全免疫，无AI
        /// </summary>
        Static_ImmuneAll = 9,

        /// <summary>
        /// NPC 移动
        /// </summary>
        DynamicNPC = 10,

        /// <summary>
        /// 全部的
        /// </summary>
        Total,
    }

    /// <summary>
    /// 福利类型
    /// </summary>
    public enum ActivityTypes
    {
        /// <summary>
        /// 没有
        /// </summary>
        None = 0,

        /// <summary>
        /// 顶部装载
        /// </summary>
        InputFirst = 1,

        /// <summary>
        /// 每天充电
        /// </summary>
        MeiRiChongZhiHaoLi = 27,

        /// <summary>
        /// 充电
        /// </summary>
        TotalCharge = 38,

        /// <summary>
        /// 累积消费
        /// </summary>
        TotalConsume = 39,

        /// <summary>
        /// 全部的
        /// </summary>
        MaxVal,
    }

    /// <summary>
    /// 重新连接型
    /// </summary>
    public enum ReConnectType
    {
        /// <summary>
        /// 从登录屏幕
        /// </summary>
        LoginWindow = 0,

        /// <summary>
        /// 从角色选择画面
        /// </summary>
        SelectRoleWindow = 1,

        /// <summary>
        /// 与当前角色重新连接
        /// </summary>
        ReconnectCurrentRole = 3,

        /// <summary>
        /// 从屏幕中选择服务器
        /// </summary>
        ServerListWindow = 4,

        /// <summary>
        /// 更改服务器
        /// </summary>
        ChangeServer = 5,
    }
}