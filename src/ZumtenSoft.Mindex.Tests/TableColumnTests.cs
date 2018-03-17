﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Tests
{
    [TestClass]
    public class TableColumnTests
    {
        [TestMethod]
        public void GetScore_WhenNoSearch_ShouldReturnZero()
        {
            TableColumn<SiteRanking, SiteRankingSearch, string> column = new TableColumn<SiteRanking, SiteRankingSearch, string>(x => x.DomainName, x => x.DomainName, StringComparer.OrdinalIgnoreCase);
            var actual = column.GetScore(new SiteRankingSearch());
            var expected = Tuple.Create(0f, false);

            Assert.AreEqual(expected, actual);
        }
    }
}