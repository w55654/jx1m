﻿namespace FS.GameEngine.Logic
{
    /// <summary>
    /// 常用常量
    /// </summary>
	public class Consts
    {
        #region 捆绑包配置文件列表

        /// <summary>
        /// 游戏配置文件
        /// </summary>
        public const string GAME_CONFIG_FILE = "Config.unity3d";

        public const string GAME_CONFIG_NAME = "Config";

        /// <summary>
        /// 音频文件
        /// </summary>
        public const string SKILLCASTSOUND_FILE = "Resources/Sound/SkillCastSound.unity3d";

        #endregion 捆绑包配置文件列表

        #region Bundle 中的 XML 文件列表

        /// <summary>
        /// XML 文件包含 Lua 脚本列表
        /// </summary>
        public const string XML_SCRIPTINDEX_FILE = "Assets/Extension/ScriptIndex.xml";

        /// <summary>
        /// XML文件包含怪物的移动信息
        /// </summary>
        public const string XML_MONSTERACTIONSET_FILE = "Assets/Config/MonsterActionSet.xml";

        /// <summary>
        /// XML文件包含角色头像信息
        /// </summary>
        public const string XML_ROLEAVARTA_FILE = "Assets/Config/RoleAvarta.xml";

        /// <summary>
        /// XML文件包含角色头像信息
        /// </summary>
        public const string XML_GUILDCONFIG_FILE = "Assets/Config/GuildConfig.xml";

        /// <summary>
        /// XML 文件包含效果信息
        /// </summary>
        public const string XML_EFFECT_FILE = "Assets/Config/StateEffect.xml";

        /// <summary>
        /// XML 文件包含 PropertyDictionary 信息
        /// </summary>
        public const string XML_PROPERTYDICTIONARY_FILE = "Assets/Config/PropertyDictionary.xml";

        /// <summary>
        /// XML文件包含技能信息
        /// </summary>
        public const string XML_SKILLDATA_FILE = "Assets/Config/SkillData.xml";

        /// <summary>
        /// XML 文件包含技能属性
        /// </summary>
        public const string XML_SKILLATTRIBUTE_FILE = "Assets/Config/SkillPropertiesLua.xml";

        /// <summary>
        /// XML 文件包含补充技能信息
        /// </summary>
        public const string XML_ENCHANTSKILL_FILE = "Assets/Config/EnchantSkill.xml";

        /// <summary>
        /// XML文件包含自激活技能信息
        /// </summary>
        public const string XML_AUTOSKILL_FILE = "Assets/Config/AutoSkill.xml";

        /// <summary>
        /// XML文件包含五个元素信息
        /// </summary>
        public const string XML_ELEMENT_FILE = "Assets/Config/Element.xml";

        /// <summary>
        /// XML文件包含教派信息
        /// </summary>
        public const string XML_FACTION_FILE = "Assets/Config/Faction.xml";

        /// <summary>
        /// XML文件包含教派介绍信息
        /// </summary>
        public const string XML_FACTIONINTRO_FILE = "Assets/Config/FactionIntro.xml";

        /// <summary>
        /// XML文件包含有关武器增强效果的信息
        /// </summary>
        public const string XML_WEAPONENHANCEEFFECT_FILE = "Assets/Config/WeaponEnhanceConfig.xml";

        #region Action Set

        /// <summary>
        /// 配置角色动作
        /// </summary>
        public const string XML_ACTIONSET_CONFIG = "Assets/Config/CharacterActionSet.xml";

        /// <summary>
        /// 马
        /// </summary>
        public const string XML_ACTIONSET_EFFECT = "Assets/Config/EffectActionSet/Effect.xml";

        /// <summary>
        /// 动作排列顺序
        /// </summary>
        public const string XML_ACTIONSET_LAYERSORT = "Assets/Config/CharacterActionSet/CharacterActionSetLayerSort.xml";

        /// <summary>
        /// 按武器名称配置移动
        /// </summary>
        public const string XML_ACTIONCONFIG = "Assets/Config/CharacterActionSet/ActionConfig.xml";

        /// <summary>
        /// 配置怪物动作
        /// </summary>
        public const string XML_ACTIONSET_NPC = "Assets/Config/MonsterActionSet/Npc.xml";

        /// <summary>
        /// 根据动作配置声音
        /// </summary>
        public const string XML_CHARACTERACTIONSETSOUND = "Assets/Config/CharacterActionSet/CharacterActionSetSound.xml";

        /// <summary>
        /// 根据动作配置声音
        /// </summary>
        public const string XML_MONSTERACTIONSETSOUND = "Assets/Config/MonsterActionSet/MonsterActionSetSound.xml";

        #endregion Action Set

        #region Bullet Action Set

        /// <summary>
        /// 配置项目符号逻辑
        /// </summary>
        public const string XML_BULLETCONFIG = "Assets/Config/BulletConfig.xml";

        /// <summary>
        /// 配置子弹运动和其他信息
        /// </summary>
        public const string XML_BULLETACTIONSET_CONFIG = "Assets/Config/BulletActionSet.xml";

        /// <summary>
        /// 配置子弹效果
        /// </summary>
        public const string XML_BULLETACTIONSET = "Assets/Config/SkillActionSet/BulletActionSet.xml";

        /// <summary>
        /// 配置子弹声音
        /// </summary>
        public const string XML_BULLETACTIONSETSOUND = "Assets/Config/SkillActionSet/BulletActionSetSound.xml";

        #endregion Bullet Action Set

        #region Items

        /// <summary>
        /// 配置装备宠物
        /// </summary>
        public const string Pet_Equip = "Assets/Config/Items/TotalItem/PetEquip.xml";

        /// <summary>
        /// 配置适合
        /// </summary>
        public const string BasicItem_amulet = "Assets/Config/Items/TotalItem/amulet.xml";

        /// <summary>
        /// 配置衬衫
        /// </summary>
        public const string BasicItem_armor = "Assets/Config/Items/TotalItem/armor.xml";

        /// <summary>
        /// 配置腰带
        /// </summary>
        public const string BasicItem_belt = "Assets/Config/Items/TotalItem/belt.xml";

        /// <summary>
        /// 鞋子配置
        /// </summary>
        public const string BasicItem_boots = "Assets/Config/Items/TotalItem/boot.xml";

        /// <summary>
        /// 家用配置
        /// </summary>
        public const string BasicItem_cuff = "Assets/Config/Items/TotalItem/cuff.xml";

        /// <summary>
        /// 配置帽子
        /// </summary>
        public const string BasicItem_helm = "Assets/Config/Items/TotalItem/helm.xml";

        /// <summary>
        /// 配置马
        /// </summary>
        public const string BasicItem_horse = "Assets/Config/Items/TotalItem/horse.xml";


        /// <summary>
        /// 成分
        /// </summary>

        public const string BasicItem_stuffitem = "Assets/Config/Items/TotalItem/stuffitem.xml";

        /// <summary>
        /// Config phi phong
        /// 暂时禁用此功能以供以后使用
        /// </summary>
        //public const string BasicItem_mantle = "Assets/Config/Items/BasicItem/mantle.xml";
        /// <summary>
        /// 配置近战武器
        /// </summary>
        public const string BasicItem_goldequip = "Assets/Config/Items/TotalItem/goldequip.xml";

        /// <summary>
        /// 配置近战武器
        /// </summary>
        public const string BasicItem_meleeweapon = "Assets/Config/Items/TotalItem/meleeweapon.xml";

        /// <summary>
        /// 配置近战武器
        /// </summary>
        public const string BasicItem_magicscript = "Assets/Config/Items/TotalItem/magicscript.xml";

        /// <summary>
        /// 配置近战武器
        /// </summary>
        public const string BasicItem_mask = "Assets/Config/Items/TotalItem/mask.xml";

        /// <summary>
        /// 配置近战武器
        /// </summary>
        public const string BasicItem_platinaequip = "Assets/Config/Items/TotalItem/platinaequip.xml";

        public const string BasicItem_metal = "Assets/Config/Items/TotalItem/mantle.xml";

        /// <summary>
        /// 配置环
        /// </summary>
        public const string BasicItem_potion = "Assets/Config/Items/TotalItem/potion.xml";

        /// <summary>
        /// 配置远程武器
        /// </summary>
        public const string BasicItem_rangeweapon = "Assets/Config/Items/TotalItem/rangeweapon.xml";

        /// <summary>
        /// 配置环
        /// </summary>
        public const string BasicItem_ring = "Assets/Config/Items/TotalItem/ring.xml";

        public const string BasicItem_pendant = "Assets/Config/Items/TotalItem/pendant.xml";

        public const string BasicItem_ShiPin = "Assets/Config/Items/TotalItem/ShiPin.xml";

        public const string BasicItem_YinJian = "Assets/Config/Items/TotalItem/YinJian.xml";

        /// <summary>
        /// 配置设备属性
        /// </summary>
        public const string MagicAttribLevel = "Assets/Config/Items/MagicAttribLevel.xml";

        /// <summary>
        /// 通过设置启用配置属性
        /// </summary>
        public const string SuiteActiveProp = "Assets/Config/Items/SuiteActiveProp.xml";

        /// <summary>
        /// 配置项索引
        /// </summary>
        public const string ItemValueCalculation = "Assets/Config/Items/ItemValueCaculation.xml";

        /// <summary>
        /// Config Exp 五要素密封
        /// </summary>
        public const string SignetExp = "Assets/Config/Items/SignnetExp.xml";

        /// <summary>
        /// 装备精炼配方配置列表
        /// </summary>
        public const string EquipRefineRecipe = "Assets/Config/Items/Refine.xml";

        #endregion Items

        #region Task

        /// <summary>
        /// 任务 XML 文件
        /// </summary>
        public const string XML_SYSTEMTASK = "Assets/Config/SystemTasks.xml";

        #endregion Task

        #region LifeSkill

        /// <summary>
        /// 生活技能 XML 文件
        /// </summary>
        public const string XML_LIFESKILL = "Assets/Config/Items/LifeSkill.xml";

        #endregion LifeSkill

        #region Repute

        /// <summary>
        /// 系统信誉 XML 文件
        /// </summary>
        public const string XML_REPUTE = "Assets/Config/repute.xml";

        #endregion Repute

        #region Animated Title

        /// <summary>
        /// 配置 Phi Phong 标题
        /// </summary>
        public const string XML_MANTLE_TITLE = "Assets/Config/AnimatedTitle/MantleTItle.xml";

        /// <summary>
        /// 配置官方标题
        /// </summary>
        public const string XML_OFFICE_TITLE = "Assets/Config/AnimatedTitle/OfficeTitle.xml";

        #endregion Animated Title

        #region 标题

        /// <summary>
        /// 配置标题
        /// </summary>
        public const string XML_ROLE_TITLE = "Assets/Config/Title.xml";

        /// <summary>
        /// 配置标题
        /// </summary>
        public const string XML_SPECIAL_TITLE = "Assets/Config/SpecialTitle.xml";

        #endregion 标题

        #region 巴赫宝荣

        /// <summary>
        /// 配置巴赫宝荣
        /// </summary>
        public const string XML_ACTIIVTY_SEASHELLCIRCLE = "Assets/Config/Activity/KTSeashellCircle.xml";

        #endregion 巴赫宝荣

        #region 祝福

        /// <summary>
        /// 配置巴赫宝荣
        /// </summary>
        public const string XML_ACTIIVTY_PLAYERPRAY = "Assets/Config/Activity/PlayerPray.xml";

        #endregion 祝福

        #region 工作

        /// <summary>
        /// 配置巴赫宝荣
        /// </summary>
        public const string XML_ACTIIVTY_LIST = "Assets/Config/Activity/ActivityList.xml";

        #endregion 工作

        #endregion Bundle 中的 XML 文件列表

        #region Resource

        /// <summary>
        /// 资源文件夹路径
        /// </summary>
        public const string RESOURCES_DIR = "Resources";

        /// <summary>
        /// 用户界面文件夹路径
        /// </summary>
        public const string UI_DIR = "Resources/UI";

        /// <summary>
        /// 音乐文件夹路径
        /// </summary>
        public const string SOUND_DIR = "Resources/Sound";

        #endregion Resource

        #region 游戏配置文件列表

        /// <summary>
        /// 未使用的套装列表
        /// </summary>
        public const string XML_MODELFILTER_FILE = "Assets/Config/ModelFilter.xml";

        /// <summary>
        /// 疯狂的配置文件
        /// </summary>
        public const string XML_MONSTER_FILE = "Assets/Config/Monster.xml";

        /// <summary>
        /// 宠物配置文件
        /// </summary>
        public const string XML_PETCONFIG_FILE = "Assets/Config/PetConfig.xml";

        /// <summary>
        /// 宠物清单文件
        /// </summary>
        public const string XML_PET_FILE = "Assets/Config/Pet.xml";

        /// <summary>
        /// 疯狂的配置文件
        /// </summary>
        public const string XML_NPC_FILE = "Assets/Config/Npc.xml";

        /// <summary>
        /// XML 文件包含地图信息
        /// </summary>
        public const string XML_MAP_FILE = "Assets/Config/MapConfig.xml";

        /// <summary>
        /// XML 文件包含世界地图信息
        /// </summary>
        public const string XML_WORLDMAP_FILE = "Assets/Config/WorldMap.xml";

        /// <summary>
        /// XML 文件包含服务器间映射信息
        /// </summary>
        public const string XML_CROSSSERVERMAP_FILE = "Assets/Config/CrossServerMap.xml";

        /// <summary>
        /// XML 文件包含领土地图信息
        /// </summary>
        public const string XML_COLONYMAP_FILE = "Assets/Config/ColonyMap.xml";

        /// <summary>
        /// XML 文件包含有关自动位移位置的信息
        /// </summary>
        public const string XML_AUTOPATH_FILE = "Assets/Config/AutoPath.xml";

        #endregion 游戏配置文件列表

#if UNITY_EDITOR

        /// <summary>
        /// Key Verify Account
        /// </summary>
        public const string HTTP_MD5_KEY = "tmsk_mu_06";

#elif UNITY_IPHONE && APPS
        /// <summary>
        /// Key Verify Account
        /// </summary>
		public const string HTTP_MD5_KEY = "HWjKO26fEJvZ27f8v0Qu9EGZ3k3phFO4NCt8A";
#else
        /// <summary>
        /// Key Verify Account
        /// </summary>
		public const string HTTP_MD5_KEY = "tmsk_mu_06";
#endif
    }
}