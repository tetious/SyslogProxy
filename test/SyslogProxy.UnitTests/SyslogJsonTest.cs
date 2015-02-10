//<78>2015-02-10T19:26:01.889893+00:00 mongo-monitor CROND  (ec2-user) CMD (/home/ec2-user/bin/run_node_metrics)

namespace SyslogProxy.UnitTests
{
    using Xunit;

    public class SyslogJsonTest
    {
        [Fact]
        public void CanParseThing()
        {
            var thing = new SyslogJson("78 2015-02-10T19:26:01.889893+00:00 mongo-monitor CROND  (ec2-user) CMD (/home/ec2-user/bin/run_node_metrics)");

            Program.WriteToSeq(thing).Wait();
        }
    }
}
