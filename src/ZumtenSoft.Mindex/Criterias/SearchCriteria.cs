﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.MappingCriterias;
using ZumtenSoft.Mindex.Mappings;

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
            return new SearchCriteriaByRange<TColumn>(start, end, false);
        }

        public static SearchCriteriaByPredicate<TColumn> ByPredicate<TColumn>(Expression<Func<TColumn, bool>> predicate)
        {
            return new SearchCriteriaByPredicate<TColumn>(predicate);
        }
    }

    public abstract class SearchCriteria<TColumn> : SearchCriteria
    {
        public static implicit operator SearchCriteria<TColumn>(TColumn value)
        {
            if (value == null)
                return null;
            return ByValues(value);
        }

        public static implicit operator SearchCriteria<TColumn>(TColumn[] values)
        {
            return ByValues(values);
        }

        public abstract string Name { get; }
        public abstract BinarySearchTable<TRow> Reduce<TRow>(BinarySearchTable<TRow> rows, TableMappingMetaData<TRow, TColumn> metaData);
        public abstract Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer);
        public abstract SearchCriteria<TColumn> Optimize<TRow>(TableMappingMetaData<TRow, TColumn> metaData);
        public abstract TableCriteriaScore GetScore<TRow>(TableMappingMetaData<TRow, TColumn> metaData);
    }
}