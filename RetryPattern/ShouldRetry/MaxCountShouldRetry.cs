using System;

namespace RetryPattern
{
    /// <summary>
    /// Will retry up to a max number of times without regard to the nature of the error
    /// </summary>
    public class MaxCountShouldRetry : IShouldRetry
    {
        private const int MAX_FAIL_COUNT = 3;

        public MaxCountShouldRetry()
            : this(MAX_FAIL_COUNT)
        {
        }

        public MaxCountShouldRetry(int maxFailCount)
        {
            MaxFailCount = maxFailCount;
        }

        public int MaxFailCount { get; set; }

        public virtual bool ShouldRetry(Exception ex, int failCount)
        {
            return failCount <= MaxFailCount;
        }
    }
}