using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace BenchmarkRunner
{
    [CoreJob]
    [ClrJob]
    public class BulkMultiply
    {
        private float[] _data;

        [Params(32, 256)]
        public int Count { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _data = new float[Count];
            for (int i = 0; i < Count; i++)
            {
                _data[i] = i;
            }
        }


        [Benchmark(Baseline = true)]
        public void Scalar()
        {
            ref float b = ref _data[0];
            float x = 42;

            for (int i = 0; i < Count; i++)
            {
                Unsafe.Add(ref b, i) *= x;
            }
        }

        [Benchmark]
        public void Scalar_Grouped()
        {
            ref float b = ref _data[0];
            float x = 42;
            int n = Count / 8;

            for (int i = 0; i < n; i += 8)
            {
                Unsafe.Add(ref b, i) *= x;
                Unsafe.Add(ref b, i + 1) *= x;
                Unsafe.Add(ref b, i + 2) *= x;
                Unsafe.Add(ref b, i + 3) *= x;
                Unsafe.Add(ref b, i + 4) *= x;
                Unsafe.Add(ref b, i + 5) *= x;
                Unsafe.Add(ref b, i + 6) *= x;
                Unsafe.Add(ref b, i + 7) *= x;
            }
        }

        [Benchmark]
        public void Vector4()
        {
            ref Vector4 b = ref Unsafe.As<float, Vector4>(ref _data[0]);
            float x = 42;

            for (int i = 0; i < Count / 4; i++)
            {
                Unsafe.Add(ref b, i) *= x;
            }
        }

        [Benchmark]
        public void Vector4_Grouped()
        {
            ref Vector4 b = ref Unsafe.As<float, Vector4>(ref _data[0]);
            float x = 42;

            int n = Count / 4 / 4;

            for (int i = 0; i < n; i++)
            {
                Unsafe.Add(ref b, i) *= x;
                Unsafe.Add(ref b, i + 1) *= x;
                Unsafe.Add(ref b, i + 2) *= x;
                Unsafe.Add(ref b, i + 3) *= x;
            }
        }
    }
}