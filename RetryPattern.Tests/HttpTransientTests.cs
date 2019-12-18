using NUnit.Framework;
using RetryPattern;
using System;
using System.Net;

namespace Tests
{

    public class HttpTransientTests
    {
        [TestCase(HttpStatusCode.RequestTimeout)]
        public void ShouldRetryTransientWebErrors(HttpStatusCode statusCode)
        {
            var runCount = 0;
            var shouldRetry = new HttpTransientShouldRetry();
            var nextWait = new NoWait();
            var retry = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                Retry.Run(() =>
                {
                    runCount++;
                    if (runCount > 1)
                    {
                        // pretend it worked the second time
                        return;
                    }
                    // make sure it retries the number of expected times by throwing an exception
                    throw new WebException("test", WebExceptionStatus.ProtocolError);
                }, retry);

                Assert.AreEqual(2, runCount);
                
            }
            catch (Exception)
            {
                Assert.Fail("should not have thrown an exception");
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(99)]
        public void ShouldNotRetryPermanentWebErrors(int maxRetries)
        {
            var runCount = 0;
            var shouldRetry = new HttpTransientShouldRetry(maxRetries);
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
                Assert.IsTrue(runCount == maxRetries + 1);
            }
        }
    }
}