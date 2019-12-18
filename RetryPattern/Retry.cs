using System;
using System.Threading.Tasks;

namespace RetryPattern
{
    public static class Retry
    {
        public static async Task RunAsync(Func<Task> func)
        {
            await RunAsync(func, RetryStrategy.Default).ConfigureAwait(false);
        }

        public static async Task RunAsync(Func<Task> func, IRetryStrategy strategy)
        {
            var failCount = 0;
            while (true)
            {
                try
                {
                    await func.Invoke().ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    failCount++;
                    if (!strategy.ShouldRetry(ex is AggregateException ? ex.InnerException : ex, failCount))
                    {
                        throw;
                    }
                }
                await Task.Delay(strategy.NextWait(failCount)).ConfigureAwait(false);
            }
        }

        public static async Task<T> RunAsync<T>(Func<Task<T>> func)
        {
            return await RunAsync(func, RetryStrategy.Default).ConfigureAwait(false);
        }

        public static async Task<T> RunAsync<T>(Func<Task<T>> func, IRetryStrategy strategy)
        {
            var failCount = 0;
            while (true)
            {
                try
                {
                    return await func.Invoke().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    failCount++;
                    if (!strategy.ShouldRetry(ex is AggregateException ? ex.InnerException : ex, failCount))
                    {
                        throw;
                    }
                }
                await Task.Delay(strategy.NextWait(failCount)).ConfigureAwait(false);
            }
        }

        public static void Run(Action func)
        {
            Run(func, RetryStrategy.Default);
        }

        public static void Run(Action func, IRetryStrategy strategy)
        {
            var failCount = 0;
            while (true)
            {
                try
                {
                    func.Invoke();
                    return;
                }
                catch (Exception ex)
                {
                    failCount++;
                    if (!strategy.ShouldRetry(ex is AggregateException ? ex.InnerException : ex, failCount))
                    {
                        throw;
                    }
                }
                Task.Delay(strategy.NextWait(failCount));
            }
        }

        public static T Run<T>(Func<T> func)
        {
            return Run(func, RetryStrategy.Default);
        }

        public static T Run<T>(Func<T> func, IRetryStrategy strategy)
        {
            var failCount = 0;
            while (true)
            {
                try
                {
                    return func.Invoke();
                }
                catch (Exception ex)
                {
                    failCount++;
                    if (!strategy.ShouldRetry(ex is AggregateException ? ex.InnerException : ex, failCount))
                    {
                        throw;
                    }
                }
                Task.Delay(strategy.NextWait(failCount));
            }
        }
    }
}