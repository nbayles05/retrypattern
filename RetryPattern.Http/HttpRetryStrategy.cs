using RetryPattern;
using System.Net.Http;

/// <summary>
/// Putting these classes in a different file to illustrate that we don't need to include System.Net 
/// or similar dependencies in the base Retry objects.
/// </summary>
namespace RetryPattern
{
    /// <summary>
    /// Convenience factory to create a default way of handling http errors
    /// </summary>
    public static class HttpRetryStrategy
    {
        public static IRetryStrategy Default
        {
            get
            {
                return new RetryStrategy(new HttpTransientShouldRetry(), new IncrementalBackoffWait());
            }
        }
    }
}