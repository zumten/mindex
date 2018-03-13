﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ZumtenSoft.Mindex.Criterias
{
    public class SearchCriteriaByRange<TColumn> : SearchCriteria<TColumn>
    {
        public TColumn Start { get; }
        public TColumn End { get; }

        public SearchCriteriaByRange(TColumn start, TColumn end)
        {
            Start = start;
            End = end;
        }

        public override BinarySearchResult<TRow> Search<TRow>(BinarySearchResult<TRow> rows, Func<TRow, TColumn> getValue, IComparer<TColumn> comparer)
        {
            return rows.ReduceRange(getValue, Start, End, comparer);
        }

        public override Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer)
        {
            var compareMethod = typeof(IComparer<TColumn>).GetMethod("Compare");
            var exprVariable = Expression.Variable(typeof(TColumn), "columnValue");
            return Expression.Block(
                new[] { exprVariable },
                Expression.Assign(exprVariable,
                    Expression.Invoke(getColumnValue, paramRow)),
                Expression.AndAlso(
                    Expression.GreaterThanOrEqual(
                        Expression.Call(Expression.Constant(comparer), compareMethod, exprVariable, Expression.Constant(Start)),
                        Expression.Constant(0)),
                    Expression.LessThanOrEqual(
                        Expression.Call(Expression.Constant(comparer), compareMethod, exprVariable, Expression.Constant(End)),
                        Expression.Constant(0))));
        }

    }


}