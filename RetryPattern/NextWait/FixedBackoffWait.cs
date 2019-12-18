using System;

namespace RetryPattern
{
    /// <summary>
    /// First retry is immediate, then fixed time between attempts
    /// </summary>
    public class FixedBackoffWait : INextWait
    {
        static readonly int DEFAULT_FACTOR = 2000;

        public FixedBackoffWait()
            : this(DEFAULT_FACTOR)
        {
        }

        public FixedBackoffWait(int factor)
        {
            Factor = factor;
        }

        /// <summary>
        /// Number of milliseconds to wait for each retry
        /// </summary>
        public int Factor { get; set; }

        public virtual TimeSpan NextWait(int failCount)
        {
            return TimeSpan.FromMilliseconds(Factor);
        }
    }
}