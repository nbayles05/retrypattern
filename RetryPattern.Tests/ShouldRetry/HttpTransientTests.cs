using NUnit.Framework;
using RetryPattern;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Tests
{
    public class HttpTransientTests
    {
        // should retry to the max number of times
        [TestCase(HttpStatusCode.RequestTimeout, 5, 5)]
        [TestCase(HttpStatusCode.TooManyRequests, 5, 5)]
        // should retry once
        [TestCase(HttpStatusCode.Unauthorized, 5, 2)]
        // should never retry
        [TestCase(HttpStatusCode.BadRequest, 5, 1)]
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

        // this is a retry once error
        [TestCase(WebExceptionStatus.CacheEntryNotFound, 5, 2)]
        // this is a retry many error (transient)
        [TestCase(WebExceptionStatus.Timeout, 5, 5)]
        // this is a don't retry error
        [TestCase(WebExceptionStatus.MessageLengthLimitExceeded, 5, 1)]
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

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(99)]
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