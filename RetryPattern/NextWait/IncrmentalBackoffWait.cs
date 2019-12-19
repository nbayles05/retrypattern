using System;

namespace RetryPattern
{
    /// <summary>
    /// First retry is immediate, then time between attempts will increase by 2 seconds
    /// </summary>
    public class IncrementalBackoffWait : INextWait
    {
        static readonly int DEFAULT_FACTOR = 2000;

        public IncrementalBackoffWait()
            : this(DEFAULT_FACTOR)
        {
        }

        public IncrementalBackoffWait(int factor)
        {
            Factor = factor;
        }

        /// <summary>
        /// Number of milliseconds to increase wait time for each retry
        /// </summary>
        public int Factor { get; set; }

        public virtual TimeSpan NextWait(int failCount)
        {
            // the minimum value is 1
            failCount = Math.Max(1, failCount);

            return TimeSpan.FromMilliseconds((failCount - 1) * Factor);
        }
    }
}