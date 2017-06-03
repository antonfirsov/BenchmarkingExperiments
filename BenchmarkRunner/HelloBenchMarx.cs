namespace BenchmarkRunner
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Jobs;
    using BenchmarkDotNet.Environments;
    using BenchmarkDotNet.Horology;

    [Config(typeof(TestConfig))]
    public class HelloBenchMarx
    {
        [Benchmark]
        public int[] Hello()
        {
            return new int[666];
        }
    }
}