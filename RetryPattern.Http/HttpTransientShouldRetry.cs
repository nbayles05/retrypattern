using System;
using System.Collections.Generic;
using System.Net;

/// <summary>
/// Putting these classes in a different project to illustrate that we don't need to include System.Net 
/// or similar dependencies in the base Retry objects.
/// </summary>
namespace RetryPattern
{
    /// <summary>
    /// Will retry certain http related errors up to a certain number of times.
    /// </summary>
    public class HttpTransientShouldRetry : MaxCountShouldRetry, IShouldRetry
    {
        private static readonly List<HttpStatusCode> _transientResponseStatusCodes =
            new List<HttpStatusCode>
            {
                HttpStatusCode.GatewayTimeout,
                HttpStatusCode.RequestTimeout,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.TooManyRequests,
            };

        private static readonly List<HttpStatusCode> _tryOnceResponseStatusCodes =
            new List<HttpStatusCode>
            {
                HttpStatusCode.Unauthorized,
                HttpStatusCode.BadGateway,
                HttpStatusCode.NoContent,
                HttpStatusCode.NotFound,
                HttpStatusCode.SwitchingProtocols,
            };

        private static readonly List<WebExceptionStatus> _transientExceptionStatuses =
             new List<WebExceptionStatus>
             {
                 WebExceptionStatus.Timeout,                 
             };

        private static readonly List<WebExceptionStatus> _tryOnceExceptionStatuses =
            new List<WebExceptionStatus>
            {
                WebExceptionStatus.CacheEntryNotFound,
                WebExceptionStatus.ConnectFailure,
                WebExceptionStatus.ConnectionClosed,
                WebExceptionStatus.KeepAliveFailure,
                WebExceptionStatus.Pending,
                WebExceptionStatus.PipelineFailure,
                WebExceptionStatus.SendFailure,
                WebExceptionStatus.TrustFailure
            };

        public HttpTransientShouldRetry()
            : base()
        {
        }

        public HttpTransientShouldRetry(int maxFailCount)
            : base(maxFailCount)
        {
        }

        public override bool ShouldRetry(Exception ex, int failCount)
        {
            if (!base.ShouldRetry(ex, failCount))
            {
                return false;
            }

            // always retry a timeout exception
            if (ex is TimeoutException)
            {
                return true;
            }

            // is this a web exception
            var wex = ex as System.Net.WebException;
            if (wex == null)
            {
                return false;
            }

            // try looking at the web exception status to see if it's a transient exception
            // there are some errors we will retry only once
            if (_transientExceptionStatuses.Contains(wex.Status) ||
                (failCount == 1 && _tryOnceExceptionStatuses.Contains(wex.Status)))
            {
                return true;
            }

            // now check to see if it's http response and if the response status code is a transient code
            // there are some errors we will retry only once
            var response = wex.Response as HttpWebResponse;
            if (response != null)
            {
                if (_transientResponseStatusCodes.Contains(response.StatusCode) ||
                    failCount ==1 && _tryOnceResponseStatusCodes.Contains(response.StatusCode))
                {
                    return true;
                }
            }
            
            // we didn't match any possible transient errors so let's not retry
            return false;
        }
    }
}