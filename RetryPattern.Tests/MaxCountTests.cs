using NUnit.Framework;
using RetryPattern;
using System;

namespace Tests
{

    public class MaxCountTests
    {
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(99, 99)]
        public void ShouldRetryMaxTimes(int maxFailCount, int expectedTryCount)
        {
            var runCount = 0;
            var shouldRetry = new MaxCountShouldRetry(maxFailCount);
            var nextWait = new NoWait();
            var retry = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                Retry.Run(() =>
                {
                    runCount++;
                    // make sure it retries the number of expected times by throwing an exception
                    throw new Exception();
                }, retry);
                Assert.Fail("should have thrown an exception");
            }
            catch (Exception)
            {
                Assert.IsTrue(runCount == expectedTryCount);
            }
        }
    }
}