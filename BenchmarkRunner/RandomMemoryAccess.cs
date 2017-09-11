using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace BenchmarkRunner
{
    [ShortRunJob]
    public class RandomMemoryAccess
    {
        private float[] _data;

        [Params(32)]
        public int Count { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _data = new float[Count];
        }

        [Benchmark(Baseline = true)]
        public float UnsafeAdd()
        {
            float res1 = 42;
            float res2 = 42;

            ref float b = ref _data[0];

            for (int i = 0; i < Count; i+=4)
            {
                ref float bb = ref Unsafe.Add(ref b, i);
                res1 = Unsafe.Add(ref bb, 1);
                res2 = Unsafe.Add(ref bb, 2);
            }

            return res1 + res2;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Lol
        {
            public float m0, m1, m2, m3;
        }

        [Benchmark]
        public float StructMember()
        {
            float res1 = 42;
            float res2 = 42;

            ref Lol b = ref Unsafe.As<float, Lol>(ref _data[0]);
            int n = Count / 4;

            for (int i = 0; i < n; i++)
            {
                ref Lol bb = ref Unsafe.Add(ref b, i);
                res1 = bb.m1;
                res2 = bb.m2;
            }

            return res1 + res2;
        }

        [Benchmark]
        public float MemberStacky()
        {
            float res1 = 42;
            float res2 = 42;

            ref Lol b = ref Unsafe.As<float, Lol>(ref _data[0]);
            int n = Count / 4;

            for (int i = 0; i < n; i++)
            {
                Lol bb = Unsafe.Add(ref b, i);
                res1 = bb.m1;
                res2 = bb.m2;
            }

            return res1 + res2;
        }
    }
}