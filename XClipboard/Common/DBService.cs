using Dapper;
using DataHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace XClipboard.Common
{
    public class DBService : Database
    {
        /// <summary>
        /// 构造数据库操作对象
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        /// <param name="defaultTable">默认表名</param>
        public DBService(string ConnectionString, string defaultTable) : base(ConnectionString, defaultTable) { }

        /// <summary>
        /// 获取表所有数据
        /// </summary>
        /// <param name="defaultTable">默认表名 可空</param>
        /// <returns></returns>
        public async Task<List<T>> GetAllData<T>(string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT * FROM [{CheckSql(tableName)}]";
            IEnumerable<T> lst = await conn.QueryAsync<T>(sql);

            return lst.ToList();
        }

        /// <summary>
        /// 返回最新的几条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<List<T>> GetData<T>(int count, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT * FROM [{tableName}] ORDER BY Id DESC LIMIT @Count";
            var parameters = new { Count = count };

            IEnumerable<T> result = await conn.QueryAsync<T>(sql, parameters);
            return result.ToList();
        }

        /// <summary>
        /// 获取所有数据总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> GetDataNumber(string defaultTable = null)
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT COUNT(*) FROM [{CheckSql(tableName)}]";
            int count = await conn.ExecuteScalarAsync<int>(sql);

            return count;
        }

        /// <summary>
        /// 获取字段中不重复数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="by">字段名</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<List<string>> GetDistinctList(string by, string defaultTable = null)
        {
            string tableName = defaultTable ?? DefaultTable;
            string sql = $"SELECT DISTINCT {CheckSql(by)} FROM [{CheckSql(tableName)}] WHERE {CheckSql(by)} IS NOT NULL ORDER BY {CheckSql(by)} DESC";
            IEnumerable<string> result = await conn.QueryAsync<string>(sql);
            return result.ToList();
        }

        /// <summary>
        /// 获取字段中不重复数据的个数
        /// </summary>
        /// <param name="by">字段名</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> GetDistinctNumber(string by, string defaultTable = null)
        {
            List<string> ret = await GetDistinctList(by, defaultTable);
            return ret.Count;
        }

        /// <summary>
        /// 搜索指定字段中符合条件的数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="by">字段 使用"|"分割多字段搜索</param>
        /// <param name="searchData">条件数据 %searchData%</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<List<T>> GetDtaByParams<T>(string by, string searchData, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            string[] vs;
            List<T> lst = new();
            if (by.Contains('|'))
            {
                vs = by.Split('|');
                foreach (var item in vs)
                {
                    var sql_All = $"SELECT * FROM [{CheckSql(tableName)}] WHERE {CheckSql(item)} LIKE @_searchData";
                    var values = await conn.QueryAsync<T>(sql_All, new
                    {
                        _searchData = $"%{searchData}%"
                    });
                    values.ToList().ForEach(x =>
                    {
                        lst.Add(x);
                    });
                }
                return lst;
            }

            var sql = $"SELECT * FROM [{CheckSql(tableName)}] WHERE {CheckSql(by)} LIKE @_searchData";
            var ret = await conn.QueryAsync<T>(sql, new
            {
                _searchData = $"%{searchData}%"
            });

            return ret.ToList();
        }

        /// <summary>
        /// 搜索指定字段中符合条件的数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues">字段，内容的字典</param>
        /// <param name="param">new{ 字段 = %内容% } 的sql参数</param>
        /// <param name="defaultTable">默认表</param>
        /// <returns></returns>
        public async Task<List<T>> GetDtaByParams<T>(Dictionary<string, string> keyValues, object param, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;

            int findcount = keyValues.Count;
            string sql = $"SELECT * FROM [{CheckSql(tableName)}] WHERE ";
            int i = 0;
            foreach (var item in keyValues)
            {
                if (i == findcount - 1)
                    sql += $"{CheckSql(item.Key)} LIKE @{item.Key}";
                else
                    sql += $"{CheckSql(item.Key)} LIKE @{item.Key} AND ";
                i++;
            }
            IEnumerable<T> lst = await conn.QueryAsync<T>(sql, param);
            if (lst.ToList().Count != 0) return lst.ToList();
            else
                return null;
        }

        public async Task<T> GetDtaByParam<T>(string by, string searchData, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            string sql = $"SELECT * FROM [{CheckSql(tableName)}] WHERE {by} == @_searchData";

            IEnumerable<T> lst = await conn.QueryAsync<T>(sql, new
            {
                _searchData = searchData
            });
            if (lst.ToList().Count == 0) return default;
            else return lst.ToList()[0];
        }

        /// <summary>
        /// 搜索指定字段中符合条件的数据列表 搜索指定id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="by">字段</param>
        /// <param name="searchData">条件数据 %searchData%</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<List<T>> GetDtaByParam<T>(string by, int searchData, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;

            var sql = $"SELECT * FROM [{CheckSql(tableName)}] WHERE {CheckSql(by)} LIKE @_searchData";
            IEnumerable<T> lst = await conn.QueryAsync<T>(sql, new
            {
                _searchData = searchData
            });

            return lst.ToList();
        }

        /// <summary>
        /// 根据ID获取数据库信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<T> GetDataByID<T>(int id, string defaultTable = null)
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT * FROM [{CheckSql(tableName)}] WHERE id == @_searchData";
            IEnumerable<T> lst = await conn.QueryAsync<T>(sql, new
            {
                _searchData = id
            });
            if (lst.ToList().Count == 0) return default(T);
            else return lst.ToList()[0];
        }

        /// <summary>
        /// 获取字段中数据相同的数据数量
        /// </summary>
        /// <param name="by">字段</param>
        /// <param name="content">内容</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> GetNumberByContent(string by, string content, string defaultTable = null)
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT COUNT({CheckSql(by)}) FROM [{CheckSql(tableName)}] WHERE {CheckSql(by)} = @contenter";
            int count = await conn.ExecuteScalarAsync<int>(sql, new { contenter = content });
            return count;
        }

        /// <summary>
        /// 获取字段中数据相同的数据数量 以天为单位的条数
        /// </summary>
        /// <param name="by"></param>
        /// <param name="content"></param>
        /// <param name="recentCount">里以天为单位</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> GetNumberByContent(string by, string content, int recentCount, string defaultTable = null)
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT COUNT({CheckSql(by)}) FROM [{CheckSql(tableName)}] WHERE {CheckSql(by)} = @contenter AND CreateTime >= datetime('now', '-{recentCount} day')";
            int count = await conn.ExecuteScalarAsync<int>(sql, new { contenter = content });
            return count;
        }

        /// <summary>
        /// 获取字段中数据相同的数据数量int
        /// </summary>
        /// <param name="by">字段</param>
        /// <param name="content">内容</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> GetNumberByContent(string by, int content, string defaultTable = null)
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT COUNT({CheckSql(by)}) FROM [{CheckSql(tableName)}] WHERE {CheckSql(by)} = {CheckSql(content.ToString())}";
            int count = await conn.ExecuteScalarAsync<int>(sql);
            return count;
        }

        /// <summary>
        /// 返回前一天的所有数据总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> GetNumberByCreateTime(DateTime createTime, string defaultTable = null)
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT COUNT(*) FROM [{CheckSql(tableName)}] WHERE createtime > '{createTime.AddDays(-1):yyyy-MM-dd HH:mm:ss}'";
            int count = await conn.ExecuteScalarAsync<int>(sql);
            return count;
        }

        /// <summary>
        /// 获取某一页的数据列表，倒叙输出
        /// 注意：当页数为1时，显示的是第二页
        /// /summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="number"></param>
        /// <param name="reverse"></param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<List<T>> GetPageData<T>(int page, int number, bool reverse, string defaultTable = null) where T : new()
        {
            List<T> ret = await GetPageData<T>(page, number, defaultTable);
            if (reverse) ret.Reverse();
            return ret;
        }

        /// <summary>
        /// 获取某一页的数据列表
        /// 注意：当页数为1时，显示的是第二页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page">页数</param>
        /// <param name="number">每页个数</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<List<T>> GetPageData<T>(int page, int number, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            var sql = $"SELECT * FROM [{CheckSql(tableName)}] LIMIT (@_page * @_number), @_number";
            IEnumerable<T> lst = await conn.QueryAsync<T>(sql, new
            {
                _page = page,
                _number = number,
            });
            return lst.ToList();
        }

        /// <summary>
        /// 类模型数据全部插入
        /// </summary>
        /// <typeparam name="T">匿名类</typeparam>
        /// <param name="param">参数类</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> InsertDataAsync<T>(T param, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            string columns = string.Join(",", param.GetType().GetProperties().Select(p => CheckSql(p.Name)));
            string values = string.Join(",", param.GetType().GetProperties().Select(p => $"@{p.Name}"));
            string sql = $"INSERT INTO [{CheckSql(tableName)}] ({columns}) VALUES ({values}); SELECT last_insert_rowid()";

            var id = await conn.ExecuteScalarAsync<int>(sql, param);
            return id;
        }

        /// <summary>
        /// 类模型数据全部插入
        /// </summary>
        /// <typeparam name="T">匿名类</typeparam>
        /// <param name="param">参数类</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public int InsertData<T>(T param, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            string columns = string.Join(",", param.GetType().GetProperties().Select(p => CheckSql(p.Name)));
            string values = string.Join(",", param.GetType().GetProperties().Select(p => $"@{p.Name}"));
            string sql = $"INSERT INTO [{CheckSql(tableName)}] ({columns}) VALUES ({values}); SELECT last_insert_rowid()";
            return conn.ExecuteScalar<int>(sql, param);
        }

        /// <summary>
        /// 类模型数据全部插入，可排除空参数
        /// </summary>
        /// <typeparam name="T">匿名类</typeparam>
        /// <param name="param">参数类</param>
        /// <param name="delParam">不进行插入的字段</param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> InsertDataAsync<T>(T param, List<string> delParam, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            string[] propertyNames = param.GetType().GetProperties().Select(p => p.Name).Except(delParam).ToArray();
            string columns = string.Join(",", propertyNames.Select(p => CheckSql(p)));
            string values = string.Join(",", propertyNames.Select(p => $"@{p}"));
            string sql = $"INSERT INTO [{CheckSql(tableName)}] ({columns}) VALUES ({values}); SELECT last_insert_rowid()";

            var id = await conn.ExecuteScalarAsync<int>(sql, param);
            return id;
        }

        /// <summary>
        /// 去重插入，有重复则不插入返回-1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="delParam"></param>
        /// <param name="defaultTable"></param>
        /// <returns></returns>
        public async Task<int> InsertDistinct<T>(T param, string property, List<string> delParam = null, string defaultTable = null) where T : new()
        {
            string tableName = defaultTable ?? DefaultTable;
            Type type = param.GetType();
            PropertyInfo propInfo = type.GetProperty(property);
            string value = propInfo.GetValue(param, null).ToString();
            int ret;
            if (value != "" || value != null)
            {
                ret = await GetNumberByContent(property, value, 1, tableName);
                if (ret > 0) return -1;
            }
            ret = await InsertDataAsync<T>(param, delParam, defaultTable);
            return ret;
        }

        //修改数据
        public async Task<T> ChangeData<T>(string by, string content, object obj, string defaultTable = null) where T : new()
        {
            T t = default;
            string tableName = defaultTable ?? DefaultTable;
            // 获取对象的类型
            PropertyInfo[] properties = obj.GetType().GetProperties();
            // 创建一个字典，用于存储属性名称和值
            Dictionary<string, object> values = new Dictionary<string, object>();
            // 遍历属性列表，将属性名称和值添加到字典中
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj);
                values.Add(property.Name, value);
            }
            // 构建 SQL 语句
            string sql = $"UPDATE [{CheckSql(tableName)}] SET ";
            foreach (KeyValuePair<string, object> pair in values)
            {
                sql += $"{pair.Key} = '{pair.Value}' ,";
            }
            sql = sql.TrimEnd(',', ' ');
            sql += $" WHERE {by} = '{content}'";
            // 执行 SQL 语句
            try
            {
                t = await conn.ExecuteScalarAsync<T>(sql);
                sql = $"SELECT * FROM [{CheckSql(tableName)}] WHERE {by} == @_searchData";
                IEnumerable<T> lst = await conn.QueryAsync<T>(sql, new
                {
                    _searchData = content
                });
                if (lst.ToList().Count == 0) return default(T);
                else return lst.ToList()[0];
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}