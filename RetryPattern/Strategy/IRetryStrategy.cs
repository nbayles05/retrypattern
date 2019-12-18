namespace RetryPattern
{
    public interface IRetryStrategy : IShouldRetry, INextWait
    {
    }
}