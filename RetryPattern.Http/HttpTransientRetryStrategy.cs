using RetryPattern;
using System.Net.Http;

/// <summary>
/// Putting these classes in a different file to illustrate that we don't need to include System.Net 
/// or similar dependencies in the base Retry objects.
/// </summary>
namespace RetryPattern
{

    /// <summary>
    /// Convenience wrapper to handle HttpTransient errors
    /// </summary>
    public class HttpTransientRetryStrategy : RetryStrategy, IRetryStrategy
    {
        public HttpTransientRetryStrategy()
            : base(new HttpTransientShouldRetry(), new IncrementalBackoffWait())
        {
        }

        public HttpTransientRetryStrategy(int maxFailCount)
            : base(new HttpTransientShouldRetry(maxFailCount), new IncrementalBackoffWait())
        {
        }
    }    
}