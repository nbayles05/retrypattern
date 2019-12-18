using NUnit.Framework;
using RetryPattern;
using System;

namespace Tests
{
    public class IncrementalfWaitTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(2000, 0, 0)]
        [TestCase(2000, 1, 2000)]
        [TestCase(2000, 2, 4000)]
        [TestCase(2000, 3, 6000)]
        [TestCase(2000, 4, 8000)]
        [TestCase(2000, 99, 198000)]
        public void ShouldWaitExpectedTime(int factor, int failCount, double expectedWait)
        {
            var nextWait = new IncrementalBackoffWait(factor);
            var timespan = nextWait.NextWait(failCount);
            Assert.AreEqual(expectedWait, timespan.TotalMilliseconds);
        }
    }
}