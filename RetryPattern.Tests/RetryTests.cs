using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        [TestMethod]
        // should never retry
        [DataRow(0, 1)]
        // should retry once
        [DataRow(1, 1)]
        // should retry up to the max
        [DataRow(5, 5)]
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

        [TestMethod]
        // should never retry
        [DataRow(0, 1)]
        // should retry once
        [DataRow(1, 1)]
        // should retry up to the max
        [DataRow(5, 5)]
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
                Assert.IsInstanceOfType(ex, typeof(NotSupportedException));
                Assert.AreEqual(expectedTryCount, tryCount);
            }
        }
    }
}