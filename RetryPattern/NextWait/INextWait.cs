using System;

namespace RetryPattern
{
    public interface INextWait
    {
        TimeSpan NextWait(int failCount);
    }
}