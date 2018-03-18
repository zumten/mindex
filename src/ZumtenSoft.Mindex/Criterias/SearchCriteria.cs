﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Criterias
{
    public abstract class SearchCriteria
    {
        public static SearchCriteriaByValue<TColumn> ByValues<TColumn>(params TColumn[] values)
        {
            return values == null || values.Length == 0 ? null : new SearchCriteriaByValue<TColumn>(values);
        }

        public static SearchCriteriaByRange<TColumn> ByRange<TColumn>(TColumn start, TColumn end)
        {
            return new SearchCriteriaByRange<TColumn>(start, end);
        }

        public static SearchCriteriaByPredicate<TColumn> ByPredicate<TColumn>(Func<TColumn, bool> predicate)
        {
            return new SearchCriteriaByPredicate<TColumn>(predicate);
        }
    }

    public abstract class SearchCriteria<TColumn> : SearchCriteria
    {
        public static implicit operator SearchCriteria<TColumn>(TColumn value)
        {
            return ByValues(value);
        }

        public static implicit operator SearchCriteria<TColumn>(TColumn[] values)
        {
            return ByValues(values);
        }

        public static implicit operator SearchCriteria<TColumn>(Func<TColumn, bool> predicate)
        {
            return ByPredicate(predicate);
        }

        public abstract BinarySearchResult<TRow> Reduce<TRow>(BinarySearchResult<TRow> rows, Func<TRow, TColumn> getValue, IComparer<TColumn> comparer);
        public abstract Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer);
        public abstract SearchCriteria<TColumn> Optimize(IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer);
        public abstract TableColumnScore GetScore(TColumn[] possibleValues, IComparer<TColumn> comparer);
    }
}