``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 7 SP1 (6.1.7601)
Processor=Intel Core i7-4810MQ CPU 2.80GHz (Haswell), ProcessorCount=8
Frequency=2728193 Hz, Resolution=366.5430 ns, Timer=TSC
  [Host]       : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1085.0
  .NET 4.6 Job : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1085.0
  Core Job     : .NET Core 4.6.25009.03, 64bit RyuJIT

Jit=RyuJit  Platform=X64  

```
 | Method |          Job | Runtime |     Mean |     Error |    StdDev |  Gen 0 | Allocated |
 |------- |------------- |-------- |---------:|----------:|----------:|-------:|----------:|
 |  Hello | .NET 4.6 Job |     Clr | 124.2 ns | 1.3046 ns | 1.2203 ns | 0.8538 |   2.63 KB |
 |  Hello |     Core Job |    Core | 123.8 ns | 0.5880 ns | 0.5500 ns | 0.8538 |   2.62 KB |
