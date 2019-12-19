using NUnit.Framework;
using RetryPattern;
using System;

namespace Tests
{
    public class IncrementalWaitTests
    {
        [TestCase(2000, 0, 0)]
        [TestCase(2000, 1, 0)]
        [TestCase(500, 2, 500)]
        [TestCase(1, 3, 2)]
        [TestCase(1000, 4, 3000)]
        [TestCase(2000, 99, 196000)]
        public void ShouldWaitExpectedTime(int factor, int failCount, double expectedWait)
        {
            var nextWait = new IncrementalBackoffWait(factor);
            var timespan = nextWait.NextWait(failCount);
            Assert.AreEqual(expectedWait, timespan.TotalMilliseconds);
        }
    }
}