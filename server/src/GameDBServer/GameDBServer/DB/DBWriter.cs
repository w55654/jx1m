using GameDBServer.Core;
using GameDBServer.Logic;
using GameDBServer.Logic.KT_ItemManager;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameDBServer.DB
{
    /// <summary>
    /// 该类用于将数据写入 DB
    /// </summary>
    public class DBWriter
    {
        /// <summary>
        /// Game DB 正常运行所必需的表
        /// </summary>
        private static readonly string[][] ValidateDatabaseTables = new string[][]{
            new string[]{"t_login", "userid"},
            new string[]{"t_usemoney_log", "Id"},
            new string[]{"t_goods_bak", "id"},
            new string[]{"t_goods_bak_1", "id"},
        };

        #region 自动为 ROLEID 设置 INDEXID

        private const string RoleExtIdKey = "role_ext_auto_increment";
        private const int RoleExtIdValidStart = 1500000000;

        #endregion 自动为 ROLEID 设置 INDEXID

        #region ，FormatUpdateSQL

        private static readonly string[] _UpdateGoods_fieldNames = { "isusing", "forge_level", "starttime", "endtime", "site", "Props", "gcount", "binding", "bagindex", "strong", "series", "otherpramer", "goodsid" };
        private static readonly byte[] _UpdateGoods_fieldTypes = { 0, 0, 3, 3, 0, 1, 0, 0, 0, 0, 0, 1, 0 };

        private static readonly string[] _UpdateTask_fieldNames = { "focus", "value1", "value2" };
        private static readonly byte[] _UpdateTask_fieldTypes = { 0, 0, 0 };

        private static readonly string[] _UpdateActivity_fieldNames = { "loginweekid", "logindayid", "loginnum", "newstep", "steptime", "lastmtime", "curmid", "curmtime", "songliid", "logingiftstate", "onlinegiftstate", "lastlimittimehuodongid", "lastlimittimedayid", "limittimeloginnum", "limittimegiftstate", "everydayonlineawardstep", "geteverydayonlineawarddayid", "serieslogingetawardstep", "seriesloginawarddayid", "seriesloginawardgoodsid", "everydayonlineawardgoodsid" };
        private static readonly byte[] _UpdateActivity_fieldTypes = { 1, 1, 0, 0, 3, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 };

        #endregion ，FormatUpdateSQL

        #region 用于背包备份的表

        private static object GoodsBakTableMutex = new object();

        private static int GoodsBakTableIndex = -1;

        private static readonly string[] GoodsBakTableNames = { "t_goods_bak", "t_goods_bak_1" };

        #endregion 用于背包备份的表

        #region

        /// <summary>
        /// 执行一条返回 INT 的 MySQL 语句
        /// 通常用于新增、更新或删除
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="sqlText"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int ExecuteSQLNoQuery(DBManager dbMgr, string sqlText, MySQLConnection conn = null)
        {
            int result = 0;
            bool keepConn = true;

            MySQLCommand cmd = null;
            try
            {
                if (conn == null)
                {
                    keepConn = false;
                    conn = dbMgr.DBConns.PopDBConnection();
                }

                using (cmd = new MySQLCommand(sqlText, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteException(ex.ToString());
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteException(ex.ToString());
                return -2;
            }
            finally
            {
                if (!keepConn && null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 检查是否已满
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static bool CheckRoleCountFull(DBManager dbMgr)
        {
            bool bFull = true;
            MySQLConnection conn = null;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();
                MySQLCommand cmd = new MySQLCommand("SELECT max(rid) AS LastID from t_roles", conn);
                MySQLDataReader reader = cmd.ExecuteReaderEx();

                int nCount = 0;
                while (reader.Read())
                {
                    nCount = Global.SafeConvertToInt32(reader[0].ToString());
                    // nCount = int.Parse(reader[0].ToString());
                }

                if (null != cmd)
                {
                    cmd.Dispose();
                    cmd = null;
                }

                if ((nCount % GameDBManager.DBAutoIncreaseStepValue) >= GameDBManager.DBAutoIncreaseStepValue - 500)
                {
                    int extId = GameDBManager.GameConfigMgr.GetGameConfigItemInt(RoleExtIdKey, 0);
                    if (extId >= RoleExtIdValidStart && nCount < extId)
                    {
                        if (0 == DBWriter.ChangeTablesAutoIncrementValue(dbMgr, "t_roles", extId))
                        {
                            bFull = false;
                        }
                        else bFull = true;
                    }
                    else bFull = true;
                }
                else
                {
                    bFull = false;
                }
            }
            catch (MySQLException ex)
            {
                bFull = true;
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return bFull;
        }

        /// <summary>
        /// 创建新角色
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int CreateRole(DBManager dbMgr, string userID, string userName, int sex, int factionID, int subID, string roleName, int serverID, int bagnum, string positionInfo, int isflashplayer)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int roleID = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = "INSERT INTO t_roles (userid, rname, sex, occupation, sub_id, position, regtime, lasttime, zoneid, username, bagnum, isflashplayer) VALUES('" + userID + "', '" + roleName + "', " + sex + ", " + factionID + "," + subID + ", '" + positionInfo + "', '" + today + "', '" + today + "', " + serverID + ", '" + userName + "'," + bagnum + "," + isflashplayer + ")";
                if (!conn.ExecuteNonQueryBool(cmdText)) return roleID;

                try
                {
                    roleID = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID");
                }
                catch (MySQLException)
                {
                    roleID = -2;
                }
            }

            return roleID;
        }

        public static bool ExecuteSqlScript(string Sql, bool WriterLogs = false)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                if (WriterLogs)
                {
                    GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", Sql), EventLevels.Important);
                }
                int Execute = conn.ExecuteNonQuery(Sql);
                if (Execute > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 玩家下线时更新
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleLogOff(DBManager dbMgr, int roleID, string userid, int zoneid, string ip, int onlineSecs)
        {
            bool ret = false;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET logofftime='{0}' WHERE rid={1}", today, roleID);
                conn.ExecuteNonQuery(cmdText);

                DateTime now = DateTime.Now;
                cmdText = string.Format("INSERT INTO t_login (userid,dayid,rid,logintime,logouttime,ip,mac,zoneid,onlinesecs,loginnum) " +
                                            "VALUES('{0}',{1},{2},'{3}','{4}','{5}','{6}',{7},{8},1) ON DUPLICATE KEY UPDATE logouttime='{4}',onlinesecs=LEAST(onlineSecs+{8},86400);"
                                            , userid, Global.GetOffsetDay(now), roleID, Global.GetDayStartTime(now), today, ip, null, zoneid, onlineSecs);
                conn.ExecuteNonQuery(cmdText);
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// 更新玩家在线时长
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleOnlineSecs(DBManager dbMgr, int roleID, int totalOnlineSecs, int antiAddictionSecs)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET totalonlinesecs={0}, antiaddictionsecs={1} WHERE rid={2}", totalOnlineSecs, antiAddictionSecs, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新玩家任务
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleCZTaskID(DBManager dbMgr, int roleID, int czTaskID)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET cztaskid={0} WHERE rid={1}", czTaskID, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 角色接取新任务时
        /// </summary>
        public static int NewTask(DBManager dbMgr, int roleID, int npcID, int taskID, string addtime, int focus, int nStarLevel)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_tasks (taskid, rid, value1, value2, focus, isdel, addtime, starlevel) VALUES({0}, {1}, {2}, {3}, {4}, {5}, '{6}', {7})", taskID, roleID, 0, 0, focus, 0, addtime, nStarLevel);
                ret = conn.ExecuteNonQuery(cmdText);
                if (ret < 0) return ret;
                ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID");
            }

            return ret;
        }

        /// <summary>
        /// 更新玩家位置
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRolePosition(DBManager dbMgr, int roleID, string position)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET position='{0}' WHERE rid={1}", position, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新角色等级、声望与经验信息
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleInfo(DBManager dbMgr, int roleID, int level, long experience, int Prestige)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET level={0}, experience={1},roleprestige={2} WHERE rid={3}", level, experience, Prestige, roleID);

                Console.WriteLine("EXECUTE : " + cmdText);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新角色头像信息
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="avartaID"></param>
        public static bool UpdateRoleAvarta(DBManager dbMgr, int roleID, int avartaID)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET pic={0} WHERE rid={1}", avartaID, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新角色绑定银两
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleMoney1(DBManager dbMgr, int roleID, int money)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET money1={0} WHERE rid={1}", money, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新绑定铜钱
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleYinLiang(DBManager dbMgr, int roleID, int yinLiang)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET yinliang={0} WHERE rid={1}", yinLiang, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新绑定元宝
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleGold(DBManager dbMgr, int roleID, int gold)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET money2={0} WHERE rid={1}", gold, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新仓库绑定铜钱
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleStoreYinLiang(DBManager dbMgr, int roleID, long yinLiang)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET store_yinliang={0} WHERE rid={1}", yinLiang, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新仓库绑定银两
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleStoreMoney(DBManager dbMgr, int roleID, long money)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET store_money={0} WHERE rid={1}", money, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新角色铜钱
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateUserMoney(DBManager dbMgr, string userID, int userMoney)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = "";
                cmdText = string.Format("INSERT INTO t_money (userid, money) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE money={2}", userID, userMoney, userMoney);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新角色充值金额
        /// 同时更新 roleID 以按角色做累计统计
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="UserID"></param>
        /// <param name="realmoney"></param>
        /// <param name="ZONEID"></param>
        /// <returns></returns>
        public static bool UpdatetInputMoney(DBManager dbMgr, string UserID, int realmoney, int ZONEID, int RoleID)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                DateTime Now = DateTime.Now;
                string TimeInsert = Now.ToString("yyyy-MM-dd H:mm:ss");
                long time = 1111;
                string cmdText = "";
                cmdText = "INSERT INTO t_inputlog (amount, u,order_no,cporder_no,time,sign,inputtime,result,zoneid,role_action) VALUES (" + realmoney + ",'" + UserID + "','" + UserID + "','" + UserID + "'," + time + ",'" + UserID + "','" + TimeInsert + "','OK'," + ZONEID + "," + RoleID + ")";

                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新充值卡信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="userID"></param>
        /// <param name="realmoney"></param>
        /// <returns></returns>
        public static bool UpdateRechageData(DBManager dbMgr, string userID, int realmoney)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = "";
                cmdText = string.Format("INSERT INTO t_money (userid, realmoney) VALUES('{0}', {1}) ON DUPLICATE KEY UPDATE realmoney= realmoney + {2}", userID, realmoney, realmoney);

                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 在角色之间转移物品
        /// </summary>
        public static int MoveGoods(DBManager dbMgr, int roleID, int goodsDbID)
        {
            int ret = -10;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_goods SET rid={0}, site=0 WHERE Id={1}", roleID, goodsDbID);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 将物品从旧主人转移到新主人
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="goodsDbID"></param>
        /// <returns></returns>
        public static int MoveGoodsByDropTake(DBManager dbMgr, int roleID, int goodsDbID)
        {
            int ret = -10;
            using (MySqlUnity conn = new MySqlUnity())
            {

                string Fist = "INSERT INTO t_goods SELECT * FROM t_goods_bak WHERE Id=" + goodsDbID;
                ret = conn.ExecuteNonQuery(Fist);
                if(ret>=0)
                {
                    string cmdText = string.Format("UPDATE t_goods SET rid={0},gcount = ABS(site),site=0 WHERE Id={1}", roleID, goodsDbID);
                    ret = conn.ExecuteNonQuery(cmdText);

                   string Sec = "Delete from t_goods_bak WHERE Id=" + goodsDbID;
                    ret = conn.ExecuteNonQuery(Sec);
                }    
               
            }

            return ret;
        }

        /// <summary>
        /// 创建一个新物品
        /// </summary>
        public static int NewGoods(DBManager dbMgr, int roleID, int goodsID, int goodsNum, string props, int forgeLevel, int binding, int site, int bagindex, string startTime, string endTime, int strong, int series, string otherpramer)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                try
                {
                    string cmdText = string.Format("INSERT INTO t_goods (rid, goodsid, Props, gcount, forge_level, binding, site, bagindex, starttime, endtime, strong, otherpramer,series) VALUES({0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7}, '{8}', '{9}', {10}, '{11}', {12})",
                    roleID, goodsID, props, goodsNum, forgeLevel, binding, site, bagindex, startTime, endTime, strong, otherpramer, series);
                    ret = conn.ExecuteNonQuery(cmdText);
                    if (ret < 0) return ret;
                    ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID");
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
            }

            return ret;
        }

        /// <summary>
        /// 根据传入参数构建 SQL 查询
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="startIndex"></param>
        /// <param name="fieldNames"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldTypes"></param>
        /// <returns></returns>
        public static string FormatUpdateSQL(int id, string[] fields, int startIndex, string[] fieldNames, string tableName, byte[] fieldTypes, string idName = "Id")
        {
            //bool first = true;
            StringBuilder sb = new StringBuilder(256);
            //string sql = "UPDATE " + tableName + " SET ";
            sb.Append("UPDATE ").Append(tableName).Append(" SET ");
            for (int i = 0; i < fieldNames.Count(); i++)
            {
                if (fields[startIndex + i] == "*")
                {
                    continue;
                }

                if (fieldTypes[i] == 0) // 数值类型
                {
                    sb.AppendFormat("{0}={1}", fieldNames[i], fields[startIndex + i]).Append(',');
                    //sql += string.Format("{0}={1}", fieldNames[i], fields[startIndex + i]);
                }
                else if (fieldTypes[i] == 1)// 字符串类型
                {
                    if (fieldNames[i] == "otherpramer" && fields[startIndex + i].Length < 10)
                    {
                        continue;
                    }
                    else
                    {
                        sb.AppendFormat("{0}='{1}'", fieldNames[i], fields[startIndex + i]).Append(',');
                    }

                    //sql += string.Format("{0}='{1}'", fieldNames[i], fields[startIndex + i]);
                }
                else if (fieldTypes[i] == 2)// 在原值基础上增加
                {
                    sb.AppendFormat("{0}={1}+{2}", fieldNames[i], fieldNames[i], fields[startIndex + i]).Append(',');
                    //sql += string.Format("{0}={1}+{2}", fieldNames[i], fieldNames[i], fields[startIndex + i]);
                }
                else if (fieldTypes[i] == 3)// 时间类型
                {
                    sb.AppendFormat("{0}='{1}'", fieldNames[i], fields[startIndex + i].Replace('$', ':')).Append(',');
                    //sql += string.Format("{0}='{1}'", fieldNames[i], fields[startIndex + i].Replace('$', ':'));
                }

                // first = false;
            }
            sb[sb.Length - 1] = ' '; // remove the last character ','
            sb.AppendFormat(" WHERE {0}={1}", idName, id);
            //System.Console.WriteLine(sb.Length);
            if (sb.Length > 100)
            {
                //System.Console.WriteLine(sb.ToString());
            }
            return sb.ToString();
            //sql += string.Format(" WHERE {0}={1}", idName, id);
            //return sql;
        }

        /// <summary>
        /// 更新物品信息
        /// </summary>
        public static int UpdateGoods(DBManager dbMgr, int id, string[] fields, int startIndex)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = FormatUpdateSQL(id, fields, startIndex, _UpdateGoods_fieldNames, "t_goods", _UpdateGoods_fieldTypes);

                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 将 1 个物品移动到备份栏
        /// </summary>
        public static int DeleteItemInDb(DBManager dbMgr, int id)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                //string cmdText = string.Format("INSERT INTO {1} SELECT *,0,NOW(),0 FROM t_goods WHERE Id={0}", id, CurrentGoodsBakTableName);
                //conn.ExecuteNonQuery(cmdText);
                string cmdText = string.Format("DELETE FROM t_goods WHERE Id={0}", id);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 备份该物品
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int BackUpAndDelete(DBManager dbMgr, int id)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = "INSERT INTO t_goods_bak SELECT * FROM t_goods WHERE Id=" + id;

                ret = conn.ExecuteNonQuery(cmdText);

                string Clear = string.Format("DELETE FROM t_goods WHERE Id={0}", id);

                ret = conn.ExecuteNonQuery(Clear);
            }

            return ret;
        }

        /// <summary>
        /// 将备份物品转回角色主背包
        /// 供 GM 恢复已备份物品
        /// </summary>
        public static int SwitchGoodsBackupTable(DBManager dbMgr)
        {
            MySQLConnection conn = null;

            try
            {
                lock (GoodsBakTableMutex)
                {
                    conn = dbMgr.DBConns.PopDBConnection();

                    DateTime now = TimeUtil.NowDateTime();
                    int needIndex = now.Month % GoodsBakTableNames.Length;
                    if (GoodsBakTableIndex < 0)
                    {
                        GoodsBakTableIndex = needIndex;
                    }

                    int currentIndex = (GoodsBakTableIndex) % GoodsBakTableNames.Length;
                    if (needIndex != currentIndex)
                    {
                        string needTableName = GoodsBakTableNames[needIndex];
                        string sqlText = string.Format("SELECT id FROM {0} limit 1;", needTableName);
                        int result = ExecuteSQLReadInt(dbMgr, sqlText, conn);
                        if (result > 0)
                        {
                            sqlText = string.Format("TRUNCATE TABLE {0};", needTableName);
                            result = ExecuteSQLNoQuery(dbMgr, sqlText, conn);
                            if (result < 0)
                            {
                                LogManager.WriteLog(LogTypes.Error, "阶段物品备份表失败，不切换数据库");
                            }
                        }
                        else
                        {
                            GoodsBakTableIndex = needIndex;
                        }
                    }
                }
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return 0;
        }

        /// <summary>
        /// 更新任务信息
        /// </summary>
        public static int UpdateTask(DBManager dbMgr, int dbID, string[] fields, int startIndex)
        {
            string cmdText = FormatUpdateSQL(dbID, fields, startIndex, _UpdateTask_fieldNames, "t_tasks", _UpdateTask_fieldTypes);

            DelayUpdateManager.getInstance().AddItemProsecc(cmdText);

            return 1;
        }

        /// <summary>
        /// 标记任务已完成
        /// </summary>
        public static bool CompleteTask(DBManager dbMgr, int roleID, int npcID, int taskID, int dbID, int TaskClass)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_taskslog (taskid, rid, count,taskclass) VALUES({0}, {1}, 1, {2}) ON DUPLICATE KEY UPDATE count=count+1", taskID, roleID, TaskClass);
                conn.ExecuteNonQueryBool(cmdText);

                cmdText = string.Format("UPDATE t_tasks SET isdel=1 WHERE Id={0}", dbID);
                conn.ExecuteNonQueryBool(cmdText);

                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// 删除任务 --> 放弃任务
        /// </summary>
        public static bool DeleteTask(DBManager dbMgr, int roleID, int taskID, int dbID)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_tasks SET isdel=1 WHERE Id={0}", dbID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 添加一位好友
        /// </summary>
        public static int AddFriend(DBManager dbMgr, int dbID, int roleID, int otherID, int friendType, int relationship)
        {
            int ret = dbID;
            using (MySqlUnity conn = new MySqlUnity())
            {
                try
                {
                    bool error = false;
                    string cmdText = string.Format("INSERT INTO t_friends (myid, otherid, friendType, relationship) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE friendType={4}", roleID, otherID, friendType, relationship, friendType);
                    error = !conn.ExecuteNonQueryBool(cmdText);
                    if (!error && dbID < 0)
                    {
                        ret = conn.GetSingleInt(string.Format("SELECT Id FROM t_friends where myid={0} and otherid={1}", roleID, otherID));
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
            }

            return ret;
        }

        /// <summary>
        /// 删除好友
        /// </summary>
        public static bool RemoveFriend(DBManager dbMgr, int dbID, int roleID)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("DELETE FROM t_friends WHERE Id={0}", dbID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新 PK 模式
        /// </summary>
        public static bool UpdatePKMode(DBManager dbMgr, int roleID, int pkMode)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET pkmode={0} WHERE rid={1}", pkMode, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新角色 PK 值
        /// </summary>
        public static bool UpdatePKValues(DBManager dbMgr, int roleID, int pkValue, int pkPoint)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET pkvalue={0}, pkpoint={1} WHERE rid={2}", pkValue, pkPoint, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 将角色二级密码保存到 DB
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleSecondPassword(DBManager dbMgr, int roleID, string secondPassword)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET second_password='{0}' WHERE rid={1}", secondPassword, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 将角色快捷键保存到 DB
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleKeys(DBManager dbMgr, int roleID, int type, string keys)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = null;
                if (type == 0)
                {
                    cmdText = string.Format("UPDATE t_roles SET main_quick_keys='{0}' WHERE rid={1}", keys, roleID);
                }
                else if (type == 1)
                {
                    cmdText = string.Format("UPDATE t_roles SET other_quick_keys='{0}' WHERE rid={1}", keys, roleID);
                }
                else if (type == 2)
                {
                    cmdText = string.Format("UPDATE t_roles SET quick_items='{0}' WHERE rid={1}", keys, roleID);
                }
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新游戏配置
        /// 通常由 GS 的 GM 指令设置
        /// </summary>
        public static int UpdateGameConfig(DBManager dbMgr, string paramName, string paramValue)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_config (paramname, paramvalue) VALUES('{0}', '{1}') ON DUPLICATE KEY UPDATE paramvalue='{2}'", paramName, paramValue, paramValue);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 将技能保存到 DB
        /// </summary>
        public static int AddSkill(DBManager dbMgr, int roleID, int skillID)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                try
                {
                    string cmdText = string.Format("INSERT INTO t_skills (rid, skillid, skilllevel, lastusedtick, cooldowntick, exp) VALUES({0}, {1}, 0, 0, 0, 0)", roleID, skillID);
                    ret = conn.ExecuteNonQuery(cmdText);
                    if (ret >= 0)
                    {
                        ret = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID");
                    }
                }
                catch (MySQLException)
                {
                    ret = -2;
                }
            }

            return ret;
        }

        /// <summary>
        /// 从 DB 删除技能
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public static bool DeleteSkill(DBManager dbMgr, int roleID, int skillID)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                try
                {
                    string cmdText = string.Format("DELETE FROM t_skills WHERE rid = {0} AND skillid = {1}", roleID, skillID);
                    return conn.ExecuteNonQueryBool(cmdText);
                }
                catch (MySQLException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 刷新 DB 中的技能信息
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="skillID"></param>
        /// <param name="skillLevel"></param>
        /// <param name="lastUsedTick"></param>
        public static bool UpdateSkillInfo(DBManager dbMgr, int roleID, int skillID, int skillLevel, long lastUsedTick, int cooldownTick, int exp)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_skills SET skilllevel={0}, lastusedtick={1}, cooldowntick={2}, exp={3} WHERE skillid={4} AND rid={5}", skillLevel, lastUsedTick, cooldownTick, exp, skillID, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新角色 Buff 列表
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int UpdateRoleBufferItem(DBManager dbMgr, int roleID, int bufferID, long startTime, long bufferSecs, long bufferVal, string customProperty)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_buffer (rid, bufferid, starttime, buffersecs, bufferval, custom_property) VALUES({0}, {1}, {2}, {3}, {4}, '{5}') ON DUPLICATE KEY UPDATE starttime={6}, buffersecs={7}, bufferval={8}",
                        roleID, bufferID, startTime, bufferSecs, bufferVal, customProperty, startTime, bufferSecs, bufferVal);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 删除角色 Buff
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="bufferID"></param>
        /// <returns></returns>
        public static int DeleteRoleBufferItem(DBManager dbMgr, int roleID, int bufferID)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("DELETE FROM t_buffer WHERE rid={0} AND bufferid={1}", roleID, bufferID);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新角色主线任务
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleMainTaskID(DBManager dbMgr, int roleID, int mainTaskID)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET maintaskid={0} WHERE rid={1}", mainTaskID, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 记录角色累计信息
        /// </summary>
        public static int CreateWelfare(DBManager dbMgr, RoleWelfare CmdData)
        {
            // 获取一年中的天数

            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText =
                    "INSERT INTO t_welfare (rid, lastdaylogin, logincontinus, sevenday_continus_step, sevenday_continus_note, sevendaylogin_note, sevendaylogin_step, createdayid, logindayid, loginweekid, online_step, levelup_step,monthid,checkpoint,fist_recharge_step,totarechage_step,totalconsume_step,day_rechage_step) " +
                    "VALUES(" + CmdData.RoleID + "," + CmdData.lastdaylogin + "," + CmdData.logincontinus + "," + CmdData.sevenday_continus_step + ",'" + CmdData.sevenday_continus_note + "','" + CmdData.sevendaylogin_note + "','" + CmdData.sevendaylogin_step + "'," + CmdData.createdayid + "," + CmdData.logindayid + "," + CmdData.loginweekid + ",'" + CmdData.online_step + "','" + CmdData.levelup_step + "'," + CmdData.monthid + ",'" + CmdData.checkpoint + "'," + CmdData.fist_recharge_step + ",'" + CmdData.totarechage_step + "','" + CmdData.totalconsume_step + "','" + CmdData.day_rechage_step + "')";
                return conn.ExecuteNonQuery(cmdText);
            }
        }

        /// <summary>
        /// 更新角色累计信息
        /// </summary>
        public static int UpdateWelfare(DBManager dbMgr, RoleWelfare CmdData)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string CmdText = "UPDATE t_welfare set lastdaylogin = " + CmdData.lastdaylogin + ",logincontinus = " +
                                 CmdData.logincontinus + ",sevenday_continus_step = " + CmdData.sevenday_continus_step +
                                 ",sevenday_continus_note = '" + CmdData.sevenday_continus_note +
                                 "',sevendaylogin_note = '" + CmdData.sevendaylogin_note + "',sevendaylogin_step = '" +
                                 CmdData.sevendaylogin_step + "',createdayid = " + CmdData.createdayid +
                                 ",logindayid = " + CmdData.logindayid + ",loginweekid = " + CmdData.loginweekid +
                                 ",online_step = '" + CmdData.online_step + "',levelup_step = '" +
                                 CmdData.levelup_step + "',monthid = " + CmdData.monthid + ",checkpoint = '" +
                                 CmdData.checkpoint + "',fist_recharge_step = " + CmdData.fist_recharge_step +
                                 ",totarechage_step = '" + CmdData.totarechage_step + "',totalconsume_step = '" +
                                 CmdData.totalconsume_step + "',day_rechage_step = '" + CmdData.day_rechage_step + "' WHERE rid = " + CmdData.RoleID + "";

                return conn.ExecuteNonQuery(CmdText);
            }
        }

        /// <summary>
        /// 更新玩家排行榜
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="rname"></param>
        /// <param name="level"></param>
        /// <param name="occupation"></param>
        /// <param name="sub_id"></param>
        /// <param name="monphai"></param>
        /// <param name="taiphu"></param>
        /// <param name="volam"></param>
        /// <param name="liendau"></param>
        /// <param name="uydanh"></param>
        /// <returns></returns>
        public static int UpdateRoleRanking(DBManager dbMgr, int roleID, string rname, int level, int occupation, int sub_id, int monphai, Int64 taiphu, int volam, int liendau, int uydanh, Int64 experience)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = "INSERT INTO t_ranking(rid, rname, level, occupation, sub_id, experience, monphai, taiphu, volam, liendau, uydanh,RankingEventType0Value,RankingEventType0Status,RankingEventType1Value,RankingEventType1Status)" +
                    " VALUES(" + roleID + ", '" + rname + "', " + level + ", " + occupation + ", " + sub_id + ", " + experience + "," + monphai + "," + taiphu + ", " + volam + "," + liendau + "," + uydanh + ",-1,-1,-1,-1) " +
                    "ON DUPLICATE KEY UPDATE level = " + level + ",rname = '"+ rname + "', monphai = " + monphai + ", taiphu = " + taiphu + ", volam = " + volam + ", liendau = " + liendau + ", uydanh = " + uydanh + ",experience = " + experience + ",occupation = " + occupation + "";
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        #region 处理邮件相关逻辑

        /// <summary>
        /// 更新邮件已读状态
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateMailHasReadFlag(DBManager dbMgr, int mailID, int rid)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_mail SET isread=1, readtime=now() where mailid={0} and receiverrid={1}", mailID, rid);
                conn.ExecuteNonQuery(cmdText);
            }
        }

        /// <summary>
        /// 更新是否可领取附件状态
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateMailHasFetchGoodsFlag(DBManager dbMgr, int mailID, int rid, int hasFetchAttachment)
        {
            bool ret = true;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText;
                /// 若为领取锁定操作
                if (hasFetchAttachment == 0)
                {
                    cmdText = string.Format("UPDATE t_mail SET hasfetchattachment={0}, bound_money={1}, bound_token={2} where mailid={3} and receiverrid={4}", hasFetchAttachment, 0, 0, mailID, rid);
                }
                else
                {
                    cmdText = string.Format("UPDATE t_mail SET hasfetchattachment={0} where mailid={1} and receiverrid={2}", hasFetchAttachment, mailID, rid);
                }
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 删除无附件邮件
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool DeleteMailDataItemExcludeGoodsList(DBManager dbMgr, int mailID, int rid)
        {
            bool ret = true;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("DELETE from t_mail where mailid={0} and receiverrid={1}", mailID, rid);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 删除邮件中的物品
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void DeleteMailGoodsList(DBManager dbMgr, int mailID)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("DELETE from t_mailgoods where mailid={0}", mailID);
                conn.ExecuteNonQuery(cmdText);
            }
        }

        /// <summary>
        /// 创建新邮件
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddMailBody(DBManager dbMgr, int senderrid, string senderrname, int receiverrid, string reveiverrname, string subject, string content, int hasFetchAttachment, int boundMoney, int boundToken,int MailType)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            int mailID = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
               
                string cmdText = string.Format("INSERT INTO t_mail (senderrid, senderrname, sendtime, receiverrid, reveiverrname, readtime, " +
                    "isread, mailtype, hasfetchattachment, subject,content, bound_money, bound_token) VALUES (" +
                    "{0},'{1}','{2}', {3}, '{4}','{5}',{6},{7},{8},'{9}','{10}', {11}, {12})",
                    senderrid, senderrname, today, receiverrid, reveiverrname, "2000-11-11 11:11:11",
                    0, MailType, hasFetchAttachment, subject, content, boundMoney, 0);
                int ret = conn.ExecuteNonQuery(cmdText);
                if (ret < 0)
                {
                    mailID = -2; 
                    return mailID;
                }

                try
                {
                    mailID = conn.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID");
                }
                catch (MySQLException)
                {
                    mailID = -3; //添加新邮件失败
                }
            }

            return mailID;
        }

        /// <summary>
        /// 向邮件添加物品
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool AddMailGoodsDataItem(DBManager dbMgr, int mailID, int goodsid, int forge_level, string Props, int gcount, int binding, int series, string otherParams, int strong)
        {
            bool ret = true;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = "";
                cmdText = string.Format("INSERT INTO t_mailgoods (mailid, goodsid, forge_level, Props, gcount, binding, series, otherpramer, strong) VALUES ({0}, {1}, {2}, '{3}', {4}, {5}, {6}, '{7}', {8})",
                        mailID, goodsid, forge_level, Props, gcount, binding, series, otherParams, strong);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 更新最后扫描邮件 ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void UpdateLastScanMailID(DBManager dbMgr, int roleID, int mailID)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_mailtemp (receiverrid, mailid) " +
                                                "VALUES ({0}, {1})", roleID, mailID);
                conn.ExecuteNonQuery(cmdText);
            }
        }

        /// <summary>
        /// 删除全部过期邮件
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void ClearOverdueMails(DBManager dbMgr, DateTime overdueTime)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("DELETE FROM t_mailgoods WHERE mailid IN (SELECT mailid FROM t_mail WHERE sendtime < '{0}');", overdueTime.ToString("yyyy-MM-dd HH:mm:ss"));
                if (conn.ExecuteNonQuery(cmdText) < 0) return;

                cmdText = string.Format("DELETE from t_mail where sendtime < '{0}'", overdueTime.ToString("yyyy-MM-dd HH:mm:ss"));
                conn.ExecuteNonQuery(cmdText);
            }
        }

        /// <summary>
        /// 清除上次邮件更新记录
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void DeleteLastScanMailIDs(DBManager dbMgr, Dictionary<int, int> lastMailDict)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                string sWhere = "";
                //生成where语句
                foreach (var item in lastMailDict)
                {
                    if (sWhere.Length > 0)
                    {
                        sWhere += " or ";
                    }
                    else
                    {
                        sWhere = " where ";
                    }

                    sWhere += string.Format(" (mailid<={0} and receiverrid={1}) ", item.Value, item.Key);
                }
                string cmdText = string.Format("DELETE from t_mailtemp {0}", sWhere);
                conn.ExecuteNonQuery(cmdText);
            }
        }

        /// <summary>
        /// 从临时表删除邮件
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static void DeleteMailIDInMailTemp(DBManager dbMgr, int mailID)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("DELETE from t_mailtemp where mailid={0}", mailID);
                conn.ExecuteNonQuery(cmdText);
            }
        }

        /// <summary>
        /// 更新最后邮件 ID
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleLastMail(DBManager dbMgr, int roleID, int mailID)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET lastmailid={0} WHERE rid={1}", mailID, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        #endregion

        /// <summary>
        /// 记录物品按天购买次数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static int AddLimitGoodsBuyItem(DBManager dbMgr, int roleID, int goodsID, int dayID, int usedNum)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_limitgoodsbuy (rid, goodsid, dayid, usednum) VALUES({0}, {1}, {2}, {3}) ON DUPLICATE KEY UPDATE dayID={2}, usedNum={3}",
                    roleID, goodsID, dayID, usedNum);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        #region 从 DB 提升 ID 上限

        public static int ChangeTablesAutoIncrementValue(DBManager dbMgr, string sTableName, int nAutoIncrementValue)
        {
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("alter table {0} auto_increment={1}", sTableName, nAutoIncrementValue);
                conn.ExecuteNonQuery(cmdText);
                return 0; //调用方要求必须返回0
            }
        }

        #endregion

        /// <summary>
        /// 更新角色参数
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="roleName"></param>
        public static bool UpdateRoleParams(DBManager dbMgr, int roleID, string name, string value, RoleParamType roleParamType = null)
        {
            if (roleParamType == null)
            {
                roleParamType = RoleParamNameInfo.GetRoleParamType(name, value);
            }

            string cmdText = string.Format("INSERT INTO `{3}` (`rid`, `{4}`, `{5}`) VALUES({0}, {1}, '{2}') ON DUPLICATE KEY UPDATE `{5}`='{2}'",
                                                 roleID, roleParamType.KeyString, value, roleParamType.TableName, roleParamType.IdxName, roleParamType.ColumnName);

            // DelayUpdateManager.getInstance().AddItemProsecc(cmdText);

            try
            {
                using (MySqlUnity conn = new MySqlUnity())
                {
                    // Console.WriteLine("DEALY MYSQL UDPATE  ITEM ==>TOTALCOUNT111111");
                    conn.ExecuteNonQueryBool(cmdText);
                    return true;
                    //Console.WriteLine("DEALY MYSQL UDPATE  ITEM ==>TOTALCOUNT3333333");
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(LogTypes.SQL, "[BUG]Exception=" + ex.ToString());
                return false;
            }

            return true;
        }

        public static bool GMSetTask(DBManager dbMgr, int roleID, int taskID, List<int> taskIDList)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                // 把日志清掉
                string DelteFirst = "Delete from t_taskslog where rid = " + roleID;
                string DelteSecon = "Delete from t_tasks where rid = " + roleID;
                conn.ExecuteNonQuery(DelteFirst);
                conn.ExecuteNonQuery(DelteSecon);
                for (int i = 1; i < taskIDList.Count - 1; i++)
                {
                    string addTask = string.Format("INSERT INTO t_tasks (taskid, rid, value1, value2, focus, isdel, addtime, starlevel) VALUES({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})", taskIDList[i], roleID, 100, 0, 0, 1, "now()", 0);

                    conn.ExecuteNonQuery(addTask);

                    string cmdText = string.Format("INSERT INTO t_taskslog (taskid, rid, count,taskclass) VALUES({0}, {1}, 1,0) ON DUPLICATE KEY UPDATE count=count+1", taskIDList[i], roleID);
                    conn.ExecuteNonQuery(cmdText);
                }
                ret = true;
            }

            return ret;
        }

        #region 记录交易历史

        /// <summary>
        /// 记录交易历史
        /// 可能不需要，服务器日志已覆盖该功能
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="rid"></param>
        /// <param name="goodsid"></param>
        /// <param name="goodsnum"></param>
        /// <param name="leftgoodsnum"></param>
        /// <param name="otherroleid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int AddExchange1Item(DBManager dbMgr, int rid, int goodsid, int goodsnum, int leftgoodsnum, int otherroleid, string result)
        {
            int ret = -1;
            string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_exchange1 (rid, goodsid, goodsnum, leftgoodsnum, otherroleid, result, rectime)" +
                    " VALUES ({0},{1},{2},{3},{4},'{5}','{6}')",
                    rid, goodsid, goodsnum, leftgoodsnum, otherroleid, result, today);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        #endregion

        /// <summary>
        /// 保存玩家门派与路线信息
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="roleID"></param>
        /// <param name="factionID"></param>
        /// <param name="routeID"></param>
        /// <returns></returns>
        public static bool UpdateRoleFactionAndRoute(DBManager dbMgr, int roleID, int factionID, int routeID)
        {
            bool ret = false;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("UPDATE t_roles SET occupation={0}, sub_id={1} WHERE rid={2}", factionID, routeID, roleID);
                ret = conn.ExecuteNonQueryBool(cmdText);
            }

            return ret;
        }

        /// <summary>
        /// 按 mark ID 记录某个事件
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="nRoleID"></param>
        /// <param name="TimeRager"></param>
        /// <param name="MarkValue"></param>
        /// <param name="MarkType"></param>
        /// <returns></returns>
        public static bool UpdateMarkData(DBManager dbMgr, int nRoleID, string TimeRager, int MarkValue, int MarkType)
        {
            bool bRet = false;

            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = "SELECT count(*) from t_mark where RoleID= " + nRoleID + " and TimeRanger = '" + TimeRager + "' and MarkType= " + MarkType + "";
                int Value = conn.GetSingleInt(cmdText);
                if (Value > 0)
                {
                    // 更新 MarkValue 的值
                    cmdText = "Update t_mark set MarkValue = " + MarkValue + " where TimeRanger= '" + TimeRager + "' and RoleID= " + nRoleID + " and MarkType = " + MarkType + "";
                }
                else
                {
                    cmdText = "Insert into t_mark(RoleID,TimeRanger,MarkValue,MarkType) VALUES (" + nRoleID + ",'" + TimeRager + "'," + MarkValue + "," + MarkType + ")";
                }

                bRet = conn.ExecuteNonQueryBool(cmdText);
            }

            return bRet;
        }

        /// <summary>
        /// 写入记录
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="nRoleID"></param>
        /// <param name="RoleName"></param>
        /// <param name="ValueRecore"></param>
        /// <param name="RecoryType"></param>
        /// <returns></returns>
        public static bool AddRecore(DBManager dbMgr, int nRoleID, string RoleName, int ValueRecore, int RecoryType)
        {
            bool bRet = false;

            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = "Insert into t_recore(RoleID,RoleName,RecoryType,DateRecore,ValueRecore,ZoneID) VALUES (" + nRoleID + ",'" + RoleName + "'," + RecoryType + ",now()," + ValueRecore + "," + GameDBManager.ZoneID + ")";

                bRet = conn.ExecuteNonQueryBool(cmdText);
            }

            return bRet;
        }

        #region 记录消费累计

        public static int SaveConsumeLog(DBManager dbMgr, int roleid, string cdate, int ctype, int amount)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_consumelog (rid, amount, cdate, ctype) VALUES({0}, {1}, '{2}', {3})",
                    roleid, amount, cdate, ctype);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        #endregion

        #region

        /// <summary>
        /// 读取记录中的 INT 值
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <param name="sqlText"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int ExecuteSQLReadInt(DBManager dbMgr, string sqlText, MySQLConnection conn = null)
        {
            int result = 0;
            bool keepConn = true;

            MySQLCommand cmd = null;
            try
            {
                if (conn == null)
                {
                    keepConn = false;
                    conn = dbMgr.DBConns.PopDBConnection();
                }

                using (cmd = new MySQLCommand(sqlText, conn))
                {
                    try
                    {
                        MySQLDataReader reader = cmd.ExecuteReaderEx();
                        if (reader.Read())
                        {
                            result = Convert.ToInt32(reader[0].ToString());
                        }
                        reader.Close();
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.WriteException(ex.ToString());
                        return -1;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.WriteException(ex.ToString());
                return -2;
            }
            finally
            {
                if (!keepConn && null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return result;
        }

        /// <summary>
        /// 执行数据库数据校验
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static int ValidateDatabase(DBManager dbMgr, string dbName)
        {
            MySQLConnection conn = null;
            string sqlText;
            int result;

            try
            {
                conn = dbMgr.DBConns.PopDBConnection();

                #region 自动增长

                int flag_t_roles_auto_increment = GameDBManager.GameConfigMgr.GetGameConfigItemInt("flag_t_roles_auto_increment", 0);

                if (flag_t_roles_auto_increment < 200000 || (flag_t_roles_auto_increment % 100000) != 0)
                {
                    Global.LogAndExitProcess("Not yet set flag_t_roles_auto_increment");
                }

                int t_roles_auto_increment = 0;
                string[] ips = Global.GetLocalAddressIPs().Split('_');
                foreach (var ip in ips)
                {
                    int idx = ip.IndexOf('.');
                    if (idx > 0)
                    {
                        idx = ip.IndexOf('.', idx + 1);
                        if (idx > 0)
                        {
                            string ipPrefix = ip.Substring(0, idx);
                            if (GameDBManager.IPRange2AutoIncreaseStepDict.TryGetValue(ipPrefix, out t_roles_auto_increment))
                            {
                                break;
                            }
                        }
                    }
                }

                if (t_roles_auto_increment > 0)
                {
                    if (t_roles_auto_increment != flag_t_roles_auto_increment)
                    {
                        Global.LogAndExitProcess("flag_t_roles_auto_increment invalid format");
                    }
                }
                else if (t_roles_auto_increment != 0 && 200000 != flag_t_roles_auto_increment)
                {
                    Global.LogAndExitProcess("flag_t_roles_auto_increment invalid format");
                }

                GameDBManager.DBAutoIncreaseStepValue = flag_t_roles_auto_increment;

                #endregion

                foreach (var item in ValidateDatabaseTables)
                {
                    sqlText = string.Format("SELECT COUNT(*) FROM information_schema.columns WHERE table_schema='{0}' AND table_name = '{1}' AND column_name='{2}' limit 1;", dbName, item[0], item[1]);
                    result = ExecuteSQLReadInt(dbMgr, sqlText, conn);
                    if (result <= 0)
                    {
                        Global.LogAndExitProcess(string.Format("Table'{0}' does not exist or has missing columns: '{1}'", item[0], item[1]));
                    }
                }

                SwitchGoodsBackupTable(dbMgr);

                DBWriter.ClearBigTable_NameCheck(dbMgr);
            }
            catch (MySQLException ex)
            {
                LogManager.WriteException(string.Format("Mysql Ex: {0}", ex.ToString()));
                throw new Exception(string.Format("Mysql Ex: {0}", ex.ToString()));
            }
            finally
            {
                if (null != conn)
                {
                    dbMgr.DBConns.PushDBConnection(conn);
                }
            }

            return 1;
        }

        #endregion

        #region Clear Name Check

        /// <summary>
        /// 清空 name check 列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int ClearBigTable_NameCheck(DBManager dbMgr)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("DELETE FROM t_name_check;");
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        #endregion

        #region

        /// <summary>
        /// 记录跨服登录历史
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int UpdateRoleKuaFuDayLog(DBManager dbMgr, RoleKuaFuDayLogData data)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                string cmdText = string.Format("INSERT INTO t_kf_day_role_log (gametype,day,rid,zoneid,signup_count, start_game_count, success_count, faild_count) " +
                                                "VALUES({0},'{1}',{2},{3},{4},{5},{6},{7}) " +
                                                "on duplicate key update zoneid={3},signup_count=signup_count+{4},start_game_count=start_game_count+{5},success_count=success_count+{6},faild_count=faild_count+{7}",
                                                data.GameType, data.Day, data.RoleID, data.ZoneId, data.SignupCount, data.StartGameCount, data.SuccessCount, data.FaildCount);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        #endregion

        #region 修复玩家邮件

        public static int ModifyGMailRecord(DBManager dbMgr, int roleID, int gmailID, int mailID)
        {
            int ret = -1;
            using (MySqlUnity conn = new MySqlUnity())
            {
                String strTableName = "t_rolegmail_record";
                string cmdText = string.Format("REPLACE INTO {0} (roleid, gmailid, mailid) VALUES({1}, {2}, {3})",
                                                strTableName, roleID, gmailID, mailID);
                ret = conn.ExecuteNonQuery(cmdText);
            }

            return ret;
        }

        #endregion
    }
}