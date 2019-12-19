using NUnit.Framework;
using RetryPattern;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tests
{
    public class RetryTests
    {
        // should never retry
        [TestCase(0, 1)]
        // should retry once
        [TestCase(1, 1)]
        // should retry up to the max
        [TestCase(5, 5)]
        public void ShouldRetryExpectedNumberOfTimes(int maxFailCount, int expectedTryCount)
        {
            var tryCount = 0;
            var shouldRetry = new MaxCountShouldRetry(maxFailCount);
            var nextWait = new NoWait();
            var strategy = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                Retry.Run(() =>
                {
                    tryCount++;
                    throw new Exception("test");
                }, strategy);

                Assert.Fail("should have thrown an exception");
            }
            catch (Exception)
            {
                Assert.AreEqual(expectedTryCount, tryCount);
            }
        }

        // should never retry
        [TestCase(0, 1)]
        // should retry once
        [TestCase(1, 1)]
        // should retry up to the max
        [TestCase(5, 5)]
        public async Task ShouldRetryExpectedNumberOfTimesAsync(int maxFailCount, int expectedTryCount)
        {
            var tryCount = 0;
            var shouldRetry = new MaxCountShouldRetry(maxFailCount);
            var nextWait = new NoWait();
            var strategy = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                await Retry.RunAsync(async () =>
                {
                    tryCount++;
                    await Task.Delay(10);
                    throw new NotSupportedException();
                }, strategy);

                Assert.Fail("should have thrown an exception");
            }
            catch (Exception ex)
            {
                // making sure this is raised as NotSupportedException instead of AggregateException
                Assert.IsInstanceOf(typeof(NotSupportedException), ex);
                Assert.AreEqual(expectedTryCount, tryCount);
            }
        }
    }
}