﻿using BenchmarkDotNet.Running;
using System;
using ZumtenSoft.Mindex.Benchmark.Benchmarks;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Benchmark
{

    class Program
    {
        static void Main(string[] args)
        {
            // BenchmarkRunner.Run<SiteRankingSearchTopDomainByTLD>();
            // BenchmarkRunner.Run<SiteRankingSearchTopDomainByComOrgNet>();
            var rows = MajesticMillionCache.Instance;
            BenchmarkRunner.Run<SiteRankingSearchCanadianDomainInGlobalTop1000>();
            Console.ReadLine();
        }

  
    }
}
