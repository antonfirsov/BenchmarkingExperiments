namespace BenchmarkRunner
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Jobs;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Environments;
    using BenchmarkDotNet.Horology;
    using BenchmarkDotNet.Jobs;

    class MyConfig : ManualConfig
    {
        public MyConfig()
        {
            Job coreJob =
                new Job("Core Job", EnvMode.RyuJitX64)
                    {
                        Env = { Runtime = Runtime.Core }
                    };

            Job net46Job =
                new Job(".NET 4.6 Job", EnvMode.RyuJitX64)
                    {
                        Env = { Runtime = Runtime.Clr }
                    };

            this.Add(coreJob);
            this.Add(net46Job);
            this.Add(MemoryDiagnoser.Default);
        }
    }

    [Config(typeof(MyConfig))]
    public class HelloBenchMarx
    {
        [Benchmark]
        public int[] Hello()
        {
            return new int[666];
        }
    }
}