using System;

namespace RetryPattern
{
    public interface IShouldRetry
    {
        bool ShouldRetry(Exception ex, int failCount);
    }
}