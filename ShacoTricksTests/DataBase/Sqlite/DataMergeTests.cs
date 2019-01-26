using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShacoTricks.DataBase.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShacoTricks.DataBase.Sqlite.Tests
{
    /// <summary>
    /// 测试前需要将
    /// G:\MyGitHub\ShacoTricks\ShacoTricks\packages\System.Data.SQLite.Core.1.0.109.2\runtimes\win-x86\native\netstandard2.0\SQLite.Interop.dll
    /// 放入debug中
    /// </summary>
    [TestClass()]
    public class DataMergeTests
    {
        [TestMethod()]
        public void DataMergeRunTest()
        {
            string source = @"C:\Users\zr644\Desktop\Additel LogII\DataV2.db3";
            string target = @"C:\Users\zr644\Desktop\Additel LogII Wireless\DataV2.db3";
            SqliteDataMerge dm = new SqliteDataMerge(source, target);
            bool a=dm.DataMergeRun();
        }
    }
}