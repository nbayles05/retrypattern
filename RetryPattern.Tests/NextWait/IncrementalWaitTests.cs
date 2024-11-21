using Microsoft.VisualStudio.TestTools.UnitTesting;
using RetryPattern;

namespace Tests
{
    public class IncrementalWaitTests
    {
        [TestMethod]
        [DataRow(2000, 0, 0)]
        [DataRow(2000, 1, 0)]
        [DataRow(500, 2, 500)]
        [DataRow(1, 3, 2)]
        [DataRow(1000, 4, 3000)]
        [DataRow(2000, 99, 196000)]
        public void ShouldWaitExpectedTime(int factor, int failCount, double expectedWait)
        {
            var nextWait = new IncrementalBackoffWait(factor);
            var timespan = nextWait.NextWait(failCount);
            Assert.AreEqual(expectedWait, timespan.TotalMilliseconds);
        }
    }
}