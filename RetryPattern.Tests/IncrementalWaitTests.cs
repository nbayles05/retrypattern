using NUnit.Framework;
using RetryPattern;
using System;

namespace Tests
{
    public class IncrementalfWaitTests
    {
        [TestCase(2000, 0, 0)]
        [TestCase(2000, 1, 0)]
        [TestCase(2000, 2, 2000)]
        [TestCase(2000, 3, 4000)]
        [TestCase(2000, 4, 6000)]
        [TestCase(2000, 99, 196000)]
        public void ShouldWaitExpectedTime(int factor, int failCount, double expectedWait)
        {
            var nextWait = new IncrementalBackoffWait(factor);
            var timespan = nextWait.NextWait(failCount);
            Assert.AreEqual(expectedWait, timespan.TotalMilliseconds);
        }
    }
}