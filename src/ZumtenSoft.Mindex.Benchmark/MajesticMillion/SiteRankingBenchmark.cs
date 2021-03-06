﻿using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using ZumtenSoft.Mindex.Stubs.MajesticMillion;

namespace ZumtenSoft.Mindex.Benchmark.MajesticMillion
{
    //[ClrJob(isBaseline: true), CoreJob, MonoJob]
    //[RPlotExporter, RankColumn]
    //[DryJob]

    public class SiteRankingBenchmark
    {
        public SiteRanking[] Rankings { get; set; }
        public SiteRankingTable Table { get; set; }
        public ILookup<string, SiteRanking> LookupRankingsByTLD { get; set; }

        [Params(1000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            Rankings = MajesticMillionHelper.LoadSiteRankings();
            Table = new SiteRankingTable(Rankings);
            LookupRankingsByTLD = Rankings.ToLookup(r => r.TopLevelDomain, StringComparer.OrdinalIgnoreCase);
        }
    }
}
