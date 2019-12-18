using System;
using System.Security.Cryptography;

namespace RetryPattern
{
    /// <summary>
    /// Time between attempts will increase exponentially
    /// This time will be randomized by default such that over a large
    /// number of retries it will average out to 2^c / 2 
    /// where c is the number of failures
    /// </summary>
    public class ExponentialBackoffWait : INextWait
    {
        static readonly int DEFAULT_FACTOR = 500;
        static readonly int MAX_WAIT = 1000 * 60 * 60; // 1 hour
        static readonly RandomNumberGenerator _random;
        static ExponentialBackoffWait()
        {
            _random = RandomNumberGenerator.Create();
        }

        public ExponentialBackoffWait()
        {
            Factor = DEFAULT_FACTOR;
            MaxWait = MAX_WAIT;
            ImmediateFirstRetry = true;
            Randomize = true;            
        }

        /// <summary>
        /// Number of milliseconds to increase wait time for each retry
        /// </summary>
        public int Factor { get; set; }

        /// <summary>
        /// This overrides the ridiculous numbers that can come with more than
        /// a couple of retries
        /// </summary>
        public int MaxWait { get; set; }

        /// <summary>
        /// If true (default) will retry immediately the first time
        /// </summary>
        public bool ImmediateFirstRetry { get; set; }

        /// <summary>
        /// If true (default), will randomize wait between 0 and max wait for the retry number
        /// </summary>
        public bool Randomize { get; set; }

        /// <summary>
        /// returns a random number between 0 and 1
        /// </summary>
        private double GetRandomNumber()
        {
            byte[] data = new byte[4];
            _random.GetBytes(data);
            var randomNumber = BitConverter.ToUInt32(data, 0);
            if (randomNumber == 0)
                return 0;
            return (randomNumber % 100) * 0.01;
        }

        public TimeSpan NextWait(int failCount)
        {
            // if it's the first fail and we should retry immediately, just do it
            if (failCount == 0 && ImmediateFirstRetry)
            {
                return TimeSpan.Zero;
            }

            // since we are using a 32 bit int, the max exponent can be 31
            // at this point we know that if ImmediateFirstRetry is true that fail count
            // will we greater than 0 so we are safe to subtract 1
            var exponent = Math.Min(ImmediateFirstRetry ? failCount - 1 : failCount, 31);

            // if ImmediateFirstRetry, we would follow this pattern
            // 0,1,2,4,8,16,32
            // otherwise
            // 1,2,4,8,16,32,64
            var multiplier = Math.Pow(2, exponent);

            // if we are randomizing the wait, we need a random number, otherwise use 1 
            // to force max wait time for this fail number
            var rnd = Randomize ? GetRandomNumber() : 1;

            // calculate the number of milliseconds to wait
            return TimeSpan.FromMilliseconds(Math.Min(MaxWait, multiplier * Factor * rnd));

        }
    }
}