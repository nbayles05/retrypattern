using RetryPattern;
using System;

namespace Tests
{
    /// <summary>
    /// Test class to return no wait time so unit tests will complete more quickly
    /// </summary>
    public class NoWait : INextWait
    {
        public TimeSpan NextWait(int failCount)
        {
            return TimeSpan.Zero;
        }
    }
}