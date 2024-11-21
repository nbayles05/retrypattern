using Microsoft.VisualStudio.TestTools.UnitTesting;
using RetryPattern;
using System;

namespace Tests
{
    public class MaxCountTests
    {
        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 1)]
        [DataRow(2, 2)]
        [DataRow(3, 3)]
        [DataRow(4, 4)]
        [DataRow(99, 99)]
        public void ShouldRetryMaxTimes(int maxFailCount, int expectedTryCount)
        {
            var runCount = 0;
            var shouldRetry = new MaxCountShouldRetry(maxFailCount);
            var nextWait = new NoWait();
            var strategy = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                Retry.Run(() =>
                {
                    runCount++;
                    // make sure it retries the number of expected times by throwing an exception
                    throw new Exception();
                }, strategy);
                Assert.Fail("should have thrown an exception");
            }
            catch (Exception)
            {
                Assert.IsTrue(runCount == expectedTryCount);
            }
        }
    }
}