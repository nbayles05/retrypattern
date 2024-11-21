using Microsoft.VisualStudio.TestTools.UnitTesting;
using RetryPattern;
using System;
using System.Net;

namespace Tests
{
    public class HttpTransientTests
    {
        [TestMethod]
        // should retry to the max number of times
        [DataRow(HttpStatusCode.RequestTimeout, 5, 5)]
        [DataRow(HttpStatusCode.TooManyRequests, 5, 5)]
        // should retry once
        [DataRow(HttpStatusCode.Unauthorized, 5, 2)]
        // should never retry
        [DataRow(HttpStatusCode.BadRequest, 5, 1)]
        public void ShouldRetryHttpStatusCodes(HttpStatusCode statusCode, int maxFailCount, int expectedTryCount)
        {
            var tryCount = 0;
            var shouldRetry = new HttpTransientShouldRetry(maxFailCount);
            var nextWait = new NoWait();
            var strategy = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                Retry.Run(() =>
                {
                    tryCount++;

                    // make sure it retries the number of expected times by throwing an exception
                    var response = HttpMock.CreateWebResponse(statusCode, null);

                    // using WebExceptionStatus.MessageLengthLimitExceeded because that is one that never triggers a retry
                    // so we can be sure that we are testing the HttpStatusCode instead of WebException.Status
                    throw new WebException("test", null, WebExceptionStatus.MessageLengthLimitExceeded, response);
                }, strategy);

                Assert.Fail("should have thrown an exception");

            }
            catch (Exception)
            {
                Assert.AreEqual(expectedTryCount, tryCount);
            }
        }

        [TestMethod]
        // this is a retry once error
        [DataRow(WebExceptionStatus.CacheEntryNotFound, 5, 2)]
        // this is a retry many error (transient)
        [DataRow(WebExceptionStatus.Timeout, 5, 5)]
        // this is a don't retry error
        [DataRow(WebExceptionStatus.MessageLengthLimitExceeded, 5, 1)]
        public void ShouldRetrySomeWebExceptions(WebExceptionStatus statusCode, int maxFailCount, int expectedTryCount)
        {
            Assert.IsTrue(expectedTryCount <= maxFailCount, "Invalid expectedTryCount or maxFailCount");

            var tryCount = 0;
            var shouldRetry = new HttpTransientShouldRetry(maxFailCount);
            var nextWait = new NoWait();
            var strategy = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                Retry.Run(() =>
                {
                    tryCount++;
                    throw new WebException("test", statusCode);
                }, strategy);
                Assert.Fail("should have thrown an exception");
            }
            catch (Exception)
            {
                Assert.AreEqual(expectedTryCount, tryCount);
            }
        }

        [TestMethod]
        public void ShouldRetryTimeoutException()
        {
            var tryCount = 0;
            var shouldRetry = new HttpTransientShouldRetry(5);
            var nextWait = new NoWait();
            var strategy = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                Retry.Run(() =>
                {
                    tryCount++;
                    throw new TimeoutException("test");
                }, strategy);
                Assert.Fail("should have thrown an exception");
            }
            catch (Exception)
            {
                Assert.AreEqual(5, tryCount);
            }
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(99)]
        public void ShouldNotRetryNonWebErrors(int maxRetries)
        {
            var tryCount = 0;
            var shouldRetry = new HttpTransientShouldRetry(maxRetries);
            var nextWait = new NoWait();
            var strategy = new RetryStrategy(shouldRetry, nextWait);

            try
            {
                Retry.Run(() =>
                {
                    tryCount++;
                    // make sure it retries the number of expected times by throwing an exception
                    throw new Exception();
                }, strategy);
                Assert.Fail("should have thrown an exception");
            }
            catch (Exception)
            {
                Assert.IsTrue(tryCount == 1);
            }
        }
    }
}