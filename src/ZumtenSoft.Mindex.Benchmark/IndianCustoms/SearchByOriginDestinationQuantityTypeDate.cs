﻿using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
    public class SearchByOriginDestinationQuantityTypeDate : IndianCustomsImportBenchmark
    {
        private static readonly string[] Origins = {"CANADA", "UNITED STATES", "MEXICO"};
        private static readonly string[] Destinations = {"Delhi"};
        private static readonly string[] QuantityTypes = {"W", "M"};
        private static readonly DateTime MinimumDate = new DateTime(2015, 5, 1);
        private static readonly DateTime MaximumDate = new DateTime(2015, 5, 14);

        private readonly Tuple<string, string, string>[] _lookupSearchCriterias =
            (from origin in Origins
                from destination in Destinations
                from quantityType in QuantityTypes
                select Tuple.Create(origin, destination, quantityType)).ToArray();

        public ILookup<Tuple<string, string, string>, CustomsImport> LookupTable { get; set; }
        public IDictionary<Tuple<string, string, string>, CustomsImport[]> LookupWithBinarySearchTable { get; set; }
        public CustomsImport[] OrderedListByOriginDestinationQuantityTypeDate { get; set; }

        public override void Setup()
        {
            base.Setup();
            LookupTable = Imports.OrderBy(i => i.Date).ToLookup(i => Tuple.Create(i.Origin, i.ImportState, i.QuantityType));
            LookupWithBinarySearchTable = LookupTable
                .ToDictionary(g => g.Key, g => g.ToArray());
            OrderedListByOriginDestinationQuantityTypeDate = Imports
                .OrderBy(i => i.Origin)
                .ThenBy(i => i.ImportState)
                .ThenBy(i => i.QuantityType)
                .ThenBy(i => i.Date)
                .ToArray();
        }

        [Benchmark]
        public List<CustomsImport> SearchLinq() => Imports
            .Where(i => Origins.Contains(i.Origin) && Destinations.Contains(i.ImportState) && QuantityTypes.Contains(i.QuantityType) && i.Date >= MinimumDate && i.Date <= MaximumDate)
            .ToList();

        [Benchmark]
        public List<CustomsImport> SearchLookup() =>
            _lookupSearchCriterias
                .SelectMany(x => LookupTable[x])
                .Where(x => x.Date >= MinimumDate && x.Date <= MaximumDate)
                .ToList();


        [Benchmark]
        public List<CustomsImport> SearchLookupWithBinarySearch() =>
            new BinarySearchResult<CustomsImport>(_lookupSearchCriterias.Select(x => LookupWithBinarySearchTable.TryGetValue(x, out var grp) ? new ArraySegment<CustomsImport>(grp) : ArraySegment<CustomsImport>.Empty).ToArray(), true)
                .ReduceRange(i => i.Date, MinimumDate, MaximumDate, Comparer<DateTime>.Default)
                .ToList();

        [Benchmark]
        public CustomsImport[] SearchOrderedListWithBinarySearch() =>
            new BinarySearchResult<CustomsImport>(OrderedListByOriginDestinationQuantityTypeDate)
                .ReduceIn(i => i.Origin, Origins, Comparer<string>.Default)
                .ReduceIn(i => i.ImportState, Destinations, Comparer<string>.Default)
                .ReduceIn(i => i.QuantityType, QuantityTypes, Comparer<string>.Default)
                .ReduceRange(i => i.Date, MinimumDate, MaximumDate, Comparer<DateTime>.Default)
                .Materialize();

        [Benchmark]
        public CustomsImport[] SearchMindex() => Table.Search(new CustomsImportSearch
        {
            Origin = Origins,
            ImportState = Destinations,
            QuantityType = QuantityTypes,
            Date = SearchCriteria.ByRange(MinimumDate, MaximumDate)
        });
    }
}