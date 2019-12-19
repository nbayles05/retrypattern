using NUnit.Framework;
using RetryPattern;
using System;

namespace Tests
{
    public class ExponentialBackoffWaitTests
    {
        // since 1 is the minimum, it will treat 0 as 1
        [TestCase(2000, true, 0, 0, Int32.MaxValue)]
        [TestCase(2000, true, 1, 0, Int32.MaxValue)]
        [TestCase(2000, true, 2, 2000, Int32.MaxValue)]
        [TestCase(2000, true, 3, 4000, Int32.MaxValue)]
        [TestCase(2000, true, 4, 8000, Int32.MaxValue)]
        // this is a test of the max wait
        [TestCase(2000, true, 5, 16000, 16000)]
        [TestCase(2000, true, 20, 16000, 16000)]
        // since 1 is the minimum, it will treat 0 as 1
        [TestCase(2000, false, 0, 2000, Int32.MaxValue)]
        [TestCase(2000, false, 1, 2000, Int32.MaxValue)]
        [TestCase(2000, false, 2, 4000, Int32.MaxValue)]
        [TestCase(2000, false, 3, 8000, Int32.MaxValue)]
        [TestCase(2000, false, 4, 16000, Int32.MaxValue)]
        // the max wait should override
        [TestCase(2000, false, 5, 32000, Int32.MaxValue)]
        [TestCase(2000, false, 20, 32000, 32000)]

        public void ShouldWaitExpectedTime(int factor, bool immediateFirstRetry, int failCount, double expectedWait, int maxWait)
        {
            // first test the non-random version
            var nextWait = new ExponentialBackoffWait()
            {
                Factor = factor,
                ImmediateFirstRetry = immediateFirstRetry,
                Randomize = false,
                MaxWait = maxWait
            };
            var timespan = nextWait.NextWait(failCount);
            Assert.AreEqual(expectedWait, timespan.TotalMilliseconds);

            // now test the random version
            nextWait.Randomize = true;
            timespan = nextWait.NextWait(failCount);
            Assert.GreaterOrEqual(timespan.TotalMilliseconds, 0);
            Assert.LessOrEqual(timespan.TotalMilliseconds, maxWait);
        }
    }
}