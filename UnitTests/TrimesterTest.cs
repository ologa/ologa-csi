using System;
using System.Collections.Generic;
using Domain;
using EFDataAccess.Services;
using EFDataAccess.Services.TrimesterServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VPPS.CSI.Domain;
using FluentAssertions;
using EFDataAccess.UOW;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class TrimesterTest
    {
        [TestMethod]
        public void TestGenerateTrimestersFor4Years()
        {
            const int NumberOfYears = 4;
            Mock<ITrimesterQueryService> mockTrimesterQueryService = new Mock<ITrimesterQueryService>();
            Mock<IUnitOfWork> mockUnitOfWork = new Mock<IUnitOfWork>();
            var listOfTrimesterDefinitions = new List<TrimesterDefinition>()
            {
                new TrimesterDefinition(){ FirstDay = 16, LastDay = 15, FirstMonth = 9, LastMonth = 12, TrimesterSequence = 1},
                new TrimesterDefinition(){ FirstDay = 16, LastDay = 15, FirstMonth = 12, LastMonth = 3, TrimesterSequence = 2},
                new TrimesterDefinition(){ FirstDay = 16, LastDay = 15, FirstMonth = 3, LastMonth = 6, TrimesterSequence = 3},
                new TrimesterDefinition(){ FirstDay = 16, LastDay = 15, FirstMonth = 6, LastMonth = 9, TrimesterSequence = 4},
            };

            mockTrimesterQueryService.Setup(x => x.GetAllTrimesterDefinitions()).Returns(listOfTrimesterDefinitions);
            TrimesterService trimesterService = new TrimesterService(mockTrimesterQueryService.Object);
            List<Trimester> trimesters = trimesterService.GenerateTrimestersForTheLastPastYears(NumberOfYears);
           
            var expectedTrimesterSize = listOfTrimesterDefinitions.Count * (NumberOfYears + 1);
            trimesters.Count.Should().Equals(expectedTrimesterSize);
            Trimester lastTrimester = trimesters[trimesters.Count - 1];
            Trimester firstTrimester = trimesters[0];

            // Assertions
            trimesters.Should().NotBeEmpty();
            Assert.AreEqual(DateTime.Now.Year - NumberOfYears, firstTrimester.EndDate.Year);
            firstTrimester.EndDate.Year.Should().Equals(DateTime.Now.Year - NumberOfYears);
            lastTrimester.Seq.Should().Equals(expectedTrimesterSize);
            lastTrimester.EndDate.Year.Should().Equals(DateTime.Now.Year);
            lastTrimester.EndDate.Month.Should().Equals(9);
            lastTrimester.EndDate.Month.Should().Equals(15);
            var currentYearTrimesters = trimesters.Where(t => t.EndDate.Year == DateTime.Now.Year);
            currentYearTrimesters.Should().NotBeEmpty();
            currentYearTrimesters.Count().Should().Equals(3);
        }
    }
}
