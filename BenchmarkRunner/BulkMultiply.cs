using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace BenchmarkRunner
{
    //[CoreJob]
    //[ClrJob]
    [ShortRunJob]
    public class BulkMultiply
    {
        private float[] _data;

        [Params(512)]
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
        public void Scalar_Group8()
        {
            ref float b = ref _data[0];
            float x = 42;
            
            for (int i = 0; i < Count; i += 8)
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

            int n = Count / 4;
            for (int i = 0; i < n; i++)
            {
                Unsafe.Add(ref b, i) *= x;
            }
        }

        [Benchmark]
        public void Vector4_Group8()
        {
            ref Vector4 b = ref Unsafe.As<float, Vector4>(ref _data[0]);
            float x = 42;

            int n = Count / 4;
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
        public void GenericVector()
        {
            ref Vector<float> b = ref Unsafe.As<float, Vector<float>>(ref _data[0]);
            Vector<float> x = new Vector<float>(42);

            int n = Count / Vector<float>.Count;
            for (int i = 0; i < n; i++)
            {
                Vector<float> v = Unsafe.Add(ref b, i);
                v *= x;
                Unsafe.Add(ref b, i) *= v;
            }
        }

        [Benchmark]
        public void GenericVector_Group4()
        {
            ref Vector<float> b = ref Unsafe.As<float, Vector<float>>(ref _data[0]);
            Vector<float> x = new Vector<float>(42);

            int n = Count / Vector<float>.Count;

            for (int i = 0; i < n; i += 4)
            {
                Vector<float> v0 = Unsafe.Add(ref b, i);
                Vector<float> v1 = Unsafe.Add(ref b, i + 1);
                Vector<float> v2 = Unsafe.Add(ref b, i + 2);
                Vector<float> v3 = Unsafe.Add(ref b, i + 3);
                
                v0 *= x;
                v1 *= x;
                v2 *= x;
                v3 *= x;
                
                Unsafe.Add(ref b, i) = v0;
                Unsafe.Add(ref b, i + 1) = v1;
                Unsafe.Add(ref b, i + 2) = v2;
                Unsafe.Add(ref b, i + 3) = v3;
            }
        }

        [Benchmark]
        public void GenericVector_Group8()
        {
            ref Vector<float> b = ref Unsafe.As<float, Vector<float>>(ref _data[0]);
            Vector<float> x = new Vector<float>(42);

            int n = Count / Vector<float>.Count;

            for (int i = 0; i < n; i+=8)
            {
                Vector<float> v0 = Unsafe.Add(ref b, i);
                Vector<float> v1 = Unsafe.Add(ref b, i + 1);
                Vector<float> v2 = Unsafe.Add(ref b, i + 2);
                Vector<float> v3 = Unsafe.Add(ref b, i + 3);
                Vector<float> v4 = Unsafe.Add(ref b, i + 4);
                Vector<float> v5 = Unsafe.Add(ref b, i + 5);
                Vector<float> v6 = Unsafe.Add(ref b, i + 6);
                Vector<float> v7 = Unsafe.Add(ref b, i + 7);

                v0 *= x;
                v1 *= x;
                v2 *= x;
                v3 *= x;
                v4 *= x;
                v5 *= x;
                v6 *= x;
                v7 *= x;

                Unsafe.Add(ref b, i) = v0;
                Unsafe.Add(ref b, i + 1) = v1;
                Unsafe.Add(ref b, i + 2) = v2;
                Unsafe.Add(ref b, i + 3) = v3;
                Unsafe.Add(ref b, i + 4) = v4;
                Unsafe.Add(ref b, i + 5) = v5;
                Unsafe.Add(ref b, i + 6) = v6;
                Unsafe.Add(ref b, i + 7) = v7;
            }
        }
    }
}