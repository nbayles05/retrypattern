using System;
using System.Collections.Generic;
using System.Net;

namespace RetryPattern
{
    /// <summary>
    /// Reusable retry strategy that can be used with any IShouldRetry and INextWait
    /// </summary>
    public class RetryStrategy : IRetryStrategy
    {
        private IShouldRetry _shouldRetry;
        private INextWait _nextWait;

        public RetryStrategy(IShouldRetry shouldRetry, INextWait nextWait)
        {
            // in a real implementation, we should be null checking these
            _shouldRetry = shouldRetry;
            _nextWait = nextWait;
        }

        public TimeSpan NextWait(int failCount)
        {
            return _nextWait.NextWait(failCount);
        }

        public bool ShouldRetry(Exception ex, int failCount)
        {
            return _shouldRetry.ShouldRetry(ex, failCount);
        }

        public static RetryStrategy Default
        {
            get
            {
                return new RetryStrategy(new MaxCountShouldRetry(), new IncrementalBackoffWait());
            }
        }
    }
}