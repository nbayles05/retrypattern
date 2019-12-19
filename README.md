# RetryPattern

Flexible Retry pattern for both syncronous and asynchronous methods.

## Basic Usage
 
Retry any exception with a simple increasing wait time between each attempt:
```
Retry.Run(() =>
{
    // do something that might throw an exception
    foo();
});
```
or if your method returns a result:
```
var result = Retry.Run(() =>
{
    // return the result of something that might fail
    return foo();
});
```



Retry any exception thrown by an asynchronous method with a simple increasing wait time between each attempt:
```
await Retry.RunAsync(async () =>
{
    // do something that might throw an exception
    await foo();
});
```
or if you are waiting for a result:
```
var result = await Retry.RunAsync(async () =>
{
    // return the result of something that might throw an exception
    return await foo();
});
```

## Customize with IRetryStrategy
If you want to retry only certain types of exceptions or you want to customize the wait time or maximum number of attempts, you will need to specify an **IRetryStrategy** which consists of:

### IShouldRetry
_determines if we should retry_
```
public interface IShouldRetry
{
    bool ShouldRetry(Exception ex, int failCount);
}
```
* Use **MaxCountShouldRetry** to retry up to a configurable maximum number of times. (default)


### INextWait
_calculates how long to wait before retrying_
```
public interface INextWait
{
    TimeSpan NextWait(int failCount);
}
```
* Use **IncrementalBackoffWait** for a simple increasing wait time between each attempt. (default)
* Use **FixedBackoffWait** to wait a fixed amount of time between each attempt.
* Use **ExponentialBackoffWait** to wait an exponentially increasing amount of time with some variability.  Typical usage would be to retry API calls with rate limits. [see also](https://en.wikipedia.org/wiki/Exponential_backoff).


### Customized Example

Here is an example of a custom IShouldRetry that will only retry TimeoutExceptions.  It inherits from MaxCountShouldRetry to gain the default functionality to stop retrying after a fixed number of attempts.
```
public class TimeoutShouldRetry : MaxCountShouldRetry, IShouldRetry
{
    public override bool ShouldRetry(Exception ex, int failCount)
    {
        if (!base.ShouldRetry(ex, failCount))
            return false;

        return ex.GetType() == typeof(TimeoutException);
    }
}
```


To use your custom **IShouldRetry** -or- **INextWait**, call any **Retry.Run** or **Retry.RunAsync** methods passing a RetryStrategy:
```
// using RunAsync here and returning a value, but you can also use the non-async version and you don't have to return a value
var result = await Retry.RunAsync(async () =>
{
    // return the result of something that might throw an exception
    return await foo();
}, new RetryStrategy(new TimeoutShouldRetry(), new ExponentialBackoffWait()));
```

## HttpRetryStrategy and HttpTransientShouldRetry
This is an example of how you might customize a RetryStrategy for your app(s) and make it available to in a simple way to all areas of your app that do something similar with similar desired behavior.

**HttpRetryStrategy** gives a simple factory to create a reusable IRetryStrategy for all uses of Http calls.
```
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
```

**HttpTransientShouldRetry** provides a customized implementation of IShouldRetry based on possible Error Codes and Http Status Codes that could be returned during web operations.
[see HttpTransientShouldRetry.cs](/RetryPattern.Http/HttpTransientShouldRetry.cs)


