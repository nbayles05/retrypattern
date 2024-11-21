using Microsoft.VisualStudio.TestTools.UnitTesting;
using RetryPattern;
using System;

namespace Tests
{
    public class FixedfWaitTests
    {
        [TestMethod]
        [DataRow(2000, 0, 0)]
        [DataRow(2000, 1, 0)]
        [DataRow(2000, 2, 2000)]
        [DataRow(2000, 3, 2000)]
        [DataRow(2000, 4, 2000)]
        [DataRow(2000, 99, 2000)]
        public void ShouldWaitExpectedTime(int factor, int failCount, double expectedWait)
        {
            var nextWait = new FixedBackoffWait(factor);
            var timespan = nextWait.NextWait(failCount);
            Assert.AreEqual(expectedWait, timespan.TotalMilliseconds);
        }
    }
}