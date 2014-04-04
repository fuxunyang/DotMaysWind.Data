﻿using System;
using System.Collections.Generic;
using System.Data;

namespace DotMaysWind.Data.Command
{
    /// <summary>
    /// Sql插入语句类
    /// </summary>
    public class InsertCommand : AbstractSqlCommand
    {
        #region 属性
        /// <summary>
        /// 获取语句类型
        /// </summary>
        public override SqlCommandType CommandType
        {
            get { return SqlCommandType.Insert; }
        }

        /// <summary>
        /// 获取或设置要插入的参数组
        /// </summary>
        public List<SqlParameter> InsertParameters
        {
            get { return this._parameters; }
            set { this._parameters = value; }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 初始化Sql插入语句类
        /// </summary>
        /// <param name="database">数据库</param>
        /// <param name="tableName">数据表名称</param>
        internal InsertCommand(AbstractDatabase database, String tableName)
            : base(database, tableName) { }
        #endregion

        #region 方法
        #region Add
        /// <summary>
        /// 插入指定参数组并返回当前语句
        /// </summary>
        /// <param name="insertParams">要插入的参数组</param>
        /// <returns>当前语句</returns>
        public InsertCommand Add(params SqlParameter[] insertParams)
        {
            if (insertParams != null)
            {
                this._parameters.AddRange(insertParams);
            }

            return this;
        }

        /// <summary>
        /// 插入指定参数并返回当前语句
        /// </summary>
        /// <param name="columnName">字段名</param>
        /// <param name="value">内容</param>
        /// <returns>当前语句</returns>
        /// <example>
        /// <code lang="C#">
        /// <![CDATA[
        /// IDatabase db = DatabaseFactory.CreateDatabase();
        /// InsertCommand cmd = db.CreateInsertCommand("tbl_Users")
        ///     .Add("UserID", 1)
        ///     .Add("UserName", "admin");
        /// 
        /// //INSERT INTO tbl_Users (UserID, UserName) VALUES (@UserID, @UserName)
        /// //@UserID = 1
        /// //@UserName = "admin"
        /// 
        /// Boolean success = cmd.Result() > 0;
        /// ]]>
        /// </code>
        /// </example>
        public InsertCommand Add(String columnName, Object value)
        {
            this._parameters.Add(this.CreateSqlParameter(columnName, value));
            return this;
        }

        /// <summary>
        /// 插入指定参数并返回当前语句
        /// </summary>
        /// <param name="columnName">字段名</param>
        /// <param name="dbType">数据类型</param>
        /// <param name="value">内容</param>
        /// <returns>当前语句</returns>
        /// <example>
        /// <code lang="C#">
        /// <![CDATA[
        /// IDatabase db = DatabaseFactory.CreateDatabase();
        /// InsertCommand cmd = db.CreateInsertCommand("tbl_Users")
        ///     .Add("UserID", DbType.Int32, 1)
        ///     .Add("UserName", DbType.String, "admin");
        /// 
        /// //INSERT INTO tbl_Users (UserID, UserName) VALUES (@UserID, @UserName)
        /// //@UserID = 1
        /// //@UserName = "admin"
        /// 
        /// Boolean success = cmd.Result() > 0;
        /// ]]>
        /// </code>
        /// </example>
        public InsertCommand Add(String columnName, DbType dbType, Object value)
        {
            this._parameters.Add(this.CreateSqlParameter(columnName, dbType, value));
            return this;
        }

        /// <summary>
        /// 插入指定参数并返回当前语句
        /// </summary>
        /// <param name="columnName">字段名</param>
        /// <param name="function">函数</param>
        /// <exception cref="ArgumentNullException">函数不能为空</exception>
        /// <returns>当前语句</returns>
        /// <example>
        /// <code lang="C#">
        /// <![CDATA[
        /// IDatabase db = DatabaseFactory.CreateDatabase();
        /// InsertCommand cmd = db.CreateInsertCommand("tbl_Users")
        ///     .Add("UserID", 1)
        ///     .Add("UserName", "admin")
        ///     .Add("CreateTime", db.Functions.Now());
        /// 
        /// //INSERT INTO tbl_Users (UserID, UserName, CreateTime) VALUES (@UserID, @UserName, GETDATE())
        /// //@UserID = 1
        /// //@UserName = "admin"
        /// //"GETDATE()" will be changed into "NOW()" in Access or MySQL, "SYSDATE" in Oracle, or "DATETIME('NOW')" in SQLite.
        /// 
        /// Boolean success = cmd.Result() > 0;
        /// ]]>
        /// </code>
        /// </example>
        public InsertCommand Add(String columnName, ISqlFunction function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }

            this._parameters.Add(this.CreateSqlParameterCustomAction(columnName, function.GetCommandText()));

            if (function.HasParameters)
            {
                this._parameters.AddRange(function.GetAllParameters());
            }

            return this;
        }

        /// <summary>
        /// 插入指定参数并返回当前语句
        /// </summary>
        /// <param name="columnName">字段名</param>
        /// <param name="command">选择语句</param>
        /// <exception cref="ArgumentNullException">选择语句不能为空</exception>
        /// <returns>当前语句</returns>
        /// <example>
        /// <code lang="C#">
        /// <![CDATA[
        /// IDatabase db = DatabaseFactory.CreateDatabase();
        /// SelectCommand countCmd = db.CreateSelectCommand("tbl_Uploads")
        ///     .Query(SqlAggregateFunction.Count)
        ///     .Where(c => c.Equal("UserID", 1));
        /// 
        /// InsertCommand cmd = db.CreateInsertCommand("tbl_Users")
        ///     .Add("UserID", 1)
        ///     .Add("UserName", "admin")
        ///     .Add("UploadCount", countCmd);
        /// 
        /// //INSERT INTO tbl_Users (UserID, UserName, UploadCount) VALUES (@UserID_Insert, @UserName, (SELECT COUNT(*) FROM tbl_Uploads WHERE UserID = @UserID_Select))
        /// //@UserID_Insert = 1
        /// //@UserName = "admin"
        /// //@UserID_Select = 1
        /// 
        /// Boolean success = cmd.Result() > 0;
        /// ]]>
        /// </code>
        /// </example>
        public InsertCommand Add(String columnName, SelectCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            this._parameters.Add(this.CreateSqlParameterCustomAction(columnName, command.GetCommandText()));

            SqlParameter[] parameters = command.GetAllParameters();

            if (parameters != null)
            {
                this._parameters.AddRange(command.GetAllParameters());
            }

            return this;
        }
        #endregion

        /// <summary>
        /// 获取Sql语句内容
        /// </summary>
        /// <returns>Sql语句内容</returns>
        public override String GetCommandText()
        {
            SqlCommandBuilder sb = new SqlCommandBuilder(this.Database);
            sb.AppendInsertPrefix().AppendTableName(this._tableName);

            if (this._parameters.Count > 0)
            {
                sb.AppendAllColumnNamesWithParentheses(this._parameters).AppendInsertValues().AppendAllParameterNamesWithParentheses(this._parameters);
            }

            return sb.ToString();
        }
        #endregion
    }
}