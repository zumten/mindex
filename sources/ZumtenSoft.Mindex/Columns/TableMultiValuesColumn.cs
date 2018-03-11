﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Columns
{
    public class TableMultiValuesColumn<TRow, TSearch, TColumn> : ITableColumn<TRow, TSearch>
    {
        private readonly Func<TSearch, SearchCriteriaByValue<TColumn>> _getColumnValue;
        private readonly Expression<Func<TRow, TColumn, bool>> _predicate;

        public TableMultiValuesColumn(Func<TSearch, SearchCriteriaByValue<TColumn>> getColumnValue, Expression<Func<TRow, TColumn, bool>> predicate)
        {
            _getColumnValue = getColumnValue;
            _predicate = predicate;
        }

        public Tuple<float, bool> GetScore(TSearch search)
        {
            return Tuple.Create(0f, false);
        }

        public IEnumerable<TRow> Sort(IEnumerable<TRow> items)
        {
            throw new NotSupportedException();
        }

        public bool Reduce(TSearch search, ref BinarySearchResult<TRow> items)
        {
            return false;
        }

        public Expression BuildCondition(ParameterExpression paramExpr, TSearch criteria)
        {
            var value = _getColumnValue(criteria);
            if (value != null)
                return BuildConditionExpression(value, paramExpr, true);
            return null;
        }

        private Expression BuildConditionExpression(SearchCriteriaByValue<TColumn> criteria, ParameterExpression paramRow, bool isOr)
        {
            if (criteria.SearchValues.Length == 0)
                return Expression.Constant(false);

            List<Expression> expressions = new List<Expression>();
            foreach (var value in criteria.SearchValues)
            {
                expressions.Add(
                    Expression.Invoke(
                        _predicate,
                        paramRow,
                        Expression.Constant(value)));
            }

            return isOr
                ? expressions.Aggregate(Expression.OrElse)
                : expressions.Aggregate(Expression.AndAlso);
        }
    }

}