using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataHandler
{
    public class Database
    {
        /// <summary>
        /// 数据库对象
        /// </summary>
        public IDbConnection? conn = null;

        private string defaultTable;

        public Database(string ConnectionString, string defaultTable)
        {
            CheckDBFile(ConnectionString);
            this.ConnectionString = "Data Source=" + ConnectionString;
            this.defaultTable = defaultTable;
        }

        ~Database()
        {
            if (conn == null) return;
            conn.Close();
        }

        /// <summary>
        /// 默认表名
        /// </summary>
        public string DefaultTable
        {
            get { return defaultTable; }
            set { defaultTable = value; }
        }

        private string ConnectionString { get; }

        /// <summary>
        /// 检查参数是否为a-z|A-Z|0-9
        /// </summary>
        /// <param name="sqlparam"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public string CheckSql(string sqlparam)
        {
            if (!Regex.IsMatch(sqlparam, "^[A-Za-z0-9]+$"))
            {
                throw new ArgumentException("Name must contain only letters.");
            }
            return sqlparam;
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public bool Connect(string tableName, object models, Action<bool>? callback = null)
        {
            conn = new SqliteConnection(ConnectionString);
            conn.Open();
            bool ret = conn.State == ConnectionState.Open;
            CheckTable(tableName, models); //检测表
            if (callback != null) callback(ret);
            return ret;
        }

        /// <summary>
        /// 删除数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(string sql, object? param = null)
        {
            return await conn.ExecuteAsync(sql, param);
        }

        /// <summary>
        /// 获取连接状态
        /// </summary>
        /// <returns></returns>
        public ConnectionState GetConnect_State()
        {
            if (conn == null) return ConnectionState.Closed;
            return conn.State;
        }

        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertAsync(string sql, object? param = null)
        {
            return await conn.ExecuteAsync(sql, param);
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<dynamic> SelectAsync(string sql, object? param = null)
        {
            if (conn == null) return false;
            return await conn.QueryAsync(sql, param);
        }

        /// <summary>
        /// 更新数据库
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync(string sql, object? param = null)
        {
            return await conn.ExecuteAsync(sql, param);
        }

        /// <summary>
        /// 检测数据库文件是否存在，若不存在新建数据库文件
        /// </summary>
        private void CheckDBFile(string DbFilePath)
        {
            if (File.Exists(DbFilePath))
                return;

            if (DbFilePath.Contains("\\"))
            {
                var Paths = DbFilePath.Split('\\');
                // 检查目录是否存在，如果不存在则创建目录
                if (!Directory.Exists(Paths[0]))
                    Directory.CreateDirectory(Paths[0]);
            }
            File.Create(DbFilePath).Close();
        }

        /// <summary>
        /// 检测表是否存在，若不存在新建表
        /// </summary>
        public bool CheckTable(string tableName, object models)
        {
            if (GetConnect_State() != ConnectionState.Open) return false;

            var checkTableCmd = new SqliteCommand($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'", (SqliteConnection?)conn);
            var reader = checkTableCmd.ExecuteReader();
            bool tableExists = reader.HasRows;
            reader.Close();

            if (!tableExists) //尝试创建表
            {
                var sql = GenerateCreateTableSql(tableName, models);
                conn.Execute(sql);
                return CheckTable(tableName, models);
            }

            return tableExists;
        }

        /// <summary>
        /// 对象转换为sql建表语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        private string GenerateCreateTableSql(string tableName, object models)
        {
            Type modelType = models.GetType();
            PropertyInfo[] properties = modelType.GetProperties();

            StringBuilder sqlcommand = new StringBuilder();
            sqlcommand.AppendLine($"CREATE TABLE [{tableName}] (");

            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                Type propertyType = property.PropertyType;

                string dataType = GetSqlDataType(propertyType);

                sqlcommand.AppendLine($"[{propertyName}] {dataType},");
            }
            var ls = sqlcommand.ToString().Split('\n');
            sqlcommand.Insert(sqlcommand.ToString().IndexOf('(') + 1, "[id] INTEGER PRIMARY KEY AUTOINCREMENT,");

            // 移除最后一个 ',' 号
            sqlcommand.Remove(sqlcommand.Length - 3, 1);

            sqlcommand.AppendLine(");");

            return sqlcommand.ToString();
        }

        /// <summary>
        /// 将Type转换为SQLITE数据类型
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string GetSqlDataType(Type propertyType)
        {
            switch (propertyType.Name)
            {
                case "Int32":
                case "Int16":
                case "UInt32":
                case "UInt16":
                    return "INTEGER";

                case "String":
                    // 设置 NVARCHAR 最大值为 255
                    return "TEXT";

                case "DateTime":
                    return "DATETIME";

                case "Double":
                case "Single":
                    return "FLOAT";

                case "Decimal":
                    return "DECIMAL";
                // 更多类型
                default:
                    throw new ArgumentException($"Unsupported data type: {propertyType.Name}");
            }
        }
    }
}