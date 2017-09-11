namespace BenchmarkRunner
{
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Environments;
    using BenchmarkDotNet.Jobs;

    class TestConfig : ManualConfig
    {
        public TestConfig()
        {
            var coreJob = Job.Dry.With(Runtime.Core).WithId("Dry.Core");
            var clrJob = Job.Dry.With(Runtime.Clr).WithId("Dry.Clr");
            
            this.Add(coreJob);
            this.Add(clrJob);
            this.Add(MemoryDiagnoser.Default);
        }
    }
}