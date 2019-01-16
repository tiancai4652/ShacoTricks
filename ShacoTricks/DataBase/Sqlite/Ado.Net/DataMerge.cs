using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ShacoTricks.DataBase.Sqlite
{
    //Target:将SqliteA的数据迁移到Sqlite.B上

    //思路转自：http://www.cnblogs.com/welhzh/p/4232107.html

    //SQL:
    //ATTACH "C:\Users\zr644\Desktop\III - 副本.db" AS SecondaryDB;
    //INSERT OR IGNORE INTO ADT760ChannelData SELECT * FROM SecondaryDB.ADT760ChannelData; 
    //DETACH DATABASE SecondaryDB

    //思路:表A附加到表B，插入表B的数据到A(获取ab公共的表，然后每个表去Insert数据)，去除表A的附加

    /// <summary>
    /// 两个Sqlite数据库的数据合并到一起
    /// </summary>
    public class DataMerge
    {

        public DataMerge(string source, string target)
        {
            SourceDBFile = source;
            TargetDBFile = target;
        }

        /// <summary>
        /// 临时附加表名字
        /// </summary>
        private static string TemporaryAttachTableName = "SecondaryDB";

        /// <summary>
        /// 源数据库文件
        /// </summary>
        private string SourceDBFile { get; set; }

        /// <summary>
        /// 目标数据库文件
        /// </summary>
        private string TargetDBFile { get; set; }

        /// <summary>
        /// 验证数据库是否能连接
        /// </summary>
        /// <param name="dbConn">数据库文件路径</param>
        /// <returns></returns>
        private bool IsValid(string dbFile)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", dbFile)))
                {
                    conn.Open();
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取当前数据库的所有表名
        /// </summary>
        /// <param name="dbFile">数据库文件路径</param>
        /// <returns></returns>
        private List<string> GetAllTableNames(string dbFile)
        {
            List<string> result = new List<string>();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", dbFile)))
                {
                    conn.Open();
                    string sql = "  select name from sqlite_master where type = 'table' order by name;";
                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                    using (SQLiteDataReader dr = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        while (dr.Read())
                        {
                            result.Add(dr["Name"].ToString());
                        }
                    }
                    conn.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        /// <summary>
        /// 从SourceFile迁移数据到TargetFile
        /// 只迁移相同表名的数据，默认表明相同的表数据结构也一样，
        /// 请先将数据结构升级到相同版本在进行合并
        /// </summary>
        public bool DataMergeRun()
        {
            if (!(IsValid(SourceDBFile) && IsValid(TargetDBFile)))
            {
                return false;
            }
            var nameListSource = GetAllTableNames(SourceDBFile);
            var nameListTarget = GetAllTableNames(TargetDBFile);
            var nameList = nameListSource.Intersect(nameListTarget).ToList();
            SQLiteTransaction trans = null;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", TargetDBFile)))
                {
                    conn.Open();

                    string sql1 = string.Format("ATTACH '{0}' AS {1}; ", SourceDBFile, TemporaryAttachTableName);
                    SQLiteCommand cmd = new SQLiteCommand(sql1, conn);
                    int result = cmd.ExecuteNonQuery();

                    trans = conn.BeginTransaction();
                    foreach (var name in nameList)
                    {
                        sql1 = string.Format("INSERT OR IGNORE INTO {0} SELECT * FROM {1}.{0};", name, TemporaryAttachTableName);
                        cmd = new SQLiteCommand(sql1, conn);
                        result = cmd.ExecuteNonQuery();
                    }
                    trans.Commit();

                    sql1 = string.Format("DETACH DATABASE {0}", TemporaryAttachTableName);
                    cmd = new SQLiteCommand(sql1, conn);
                    result = cmd.ExecuteNonQuery();
                    conn.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", TargetDBFile)))
                    {
                        string sql1 = string.Format("DETACH DATABASE {0}", TemporaryAttachTableName);
                        SQLiteCommand cmd = new SQLiteCommand(sql1, conn);
                        int result = cmd.ExecuteNonQuery();
                    }
                }
                catch
                { }
                trans.Rollback();
                return false;
            }
        }
    }
}
