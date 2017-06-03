namespace BenchmarkRunner
{
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Jobs;

    class TestConfig : ManualConfig
    {
        public TestConfig()
        {
            this.Add(Job.Core);
            this.Add(Job.Clr);
            this.Add(MemoryDiagnoser.Default);
        }
    }
}