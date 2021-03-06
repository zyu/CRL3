﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery
{
    /// <summary>
    /// 关联查询分支
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TJoin"></typeparam>
    public sealed class LambdaQueryJoin<T, TJoin>
        where T : IModel, new()
        where TJoin : IModel, new()
    {
        /// <summary>
        /// 关联查询分支
        /// </summary>
        /// <param name="query"></param>
        public LambdaQueryJoin(LambdaQueryBase query)
        {
            BaseQuery = query;
        }
        LambdaQueryBase BaseQuery;

        /// <summary>
        /// 按TJoin追加条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> Where(Expression<Func<TJoin, bool>> expression)
        {
            string condition = BaseQuery.FormatExpression(expression.Body);
            BaseQuery.AddInnerRelationCondition(typeof(TJoin), condition);
            return this;
        }
        /// <summary>
        /// 按关联对象选择查询字段
        /// 可多次调用,不要重复
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> Select(Expression<Func<T, TJoin, object>> resultSelector) 
        {
            //在关联两次以上,可调用以下方法指定关联对象获取对应的字段
            var resultFields = BaseQuery.GetSelectField(resultSelector.Body, false, typeof(T), typeof(TJoin));
            BaseQuery.__QueryFields.AddRange(resultFields);
            return this;
        }
        /// <summary>
        /// 选择TJoin关联值到对象内部索引
        /// 可调用多次,不要重复
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> SelectAppendValue(Expression<Func<TJoin, object>> resultSelector)
        {
            //var innerType = typeof(TJoin);
            if (BaseQuery.__QueryFields.Count == 0)
            {
                BaseQuery.SelectAll();
            }
            var resultFields = BaseQuery.GetSelectField(resultSelector.Body, true, typeof(T), typeof(TJoin));
            BaseQuery.__QueryFields.AddRange(resultFields);
            return this;
        }
        /// <summary>
        /// 按关联对象设置GROUP字段
        /// 可多次调用,不要重复
        /// </summary>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> GroupBy(Expression<Func<T, TJoin, object>> resultSelector)
        {
            //在关联两次以上,可调用以下方法指定关联对象获取对应的字段
            //var innerType = typeof(TJoin);
            var resultFields = BaseQuery.GetSelectField(resultSelector.Body, false, typeof(T), typeof(TJoin));
            BaseQuery.__GroupFields.AddRange(resultFields);
            return this;
        }
        /// <summary>
        /// 按TJoin排序
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public LambdaQueryJoin<T, TJoin> OrderBy<TResult>(Expression<Func<TJoin, TResult>> expression, bool desc = true) 
        {
            //var innerType = typeof(TJoin);
            var fields = BaseQuery.GetSelectField(expression.Body, false, typeof(T), typeof(TJoin));
            if (!string.IsNullOrEmpty(BaseQuery.__QueryOrderBy))
            {
                BaseQuery.__QueryOrderBy += ",";
            }
            BaseQuery.__QueryOrderBy += string.Format(" {0} {1}", fields.First().QueryField, desc ? "desc" : "asc");
            return this;
        }
        /// <summary>
        /// 在当前关联基础上再创建关联
        /// </summary>
        /// <typeparam name="TJoin2">再关联的类型</typeparam>
        /// <param name="expression">关联语法</param>
        /// <param name="joinType">关联类型</param>
        /// <returns></returns>
        public LambdaQueryJoin<TJoin, TJoin2> Join<TJoin2>(Expression<Func<TJoin, TJoin2, bool>> expression, JoinType joinType = JoinType.Inner) where TJoin2 : IModel, new()
        {
            //like
            //query.Join<Code.Member>((a, b) => a.UserId == b.Id)
            //    .Select((a, b) => new { a.BarCode, b.Name })
            //    .Join<Code.Order>((a, b) => a.Id == b.Id);
            var query2 = new LambdaQueryJoin<TJoin, TJoin2>(BaseQuery);
            var innerType = typeof(TJoin2);
            BaseQuery.__JoinTypes.Add(innerType, joinType);
            BaseQuery.GetPrefix(innerType);
            string condition = BaseQuery.FormatJoinExpression(expression.Body);
            BaseQuery.AddInnerRelation(innerType, condition);
            return query2;
        }
        /// <summary>
        /// LeftJoin
        /// 在当前关联基础上再创建关联
        /// </summary>
        /// <typeparam name="TJoin2"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQueryJoin<TJoin, TJoin2> LeftJoin<TJoin2>(Expression<Func<TJoin, TJoin2, bool>> expression) where TJoin2 : IModel, new()
        {
            return Join(expression, JoinType.Left);
        }
        /// <summary>
        /// RightJoin
        /// 在当前关联基础上再创建关联
        /// </summary>
        /// <typeparam name="TJoin2"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public LambdaQueryJoin<TJoin, TJoin2> RightJoin<TJoin2>(Expression<Func<TJoin, TJoin2, bool>> expression) where TJoin2 : IModel, new()
        {
            return Join(expression, JoinType.Right);
        }
    }
}
