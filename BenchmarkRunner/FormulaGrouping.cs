using System.Numerics;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace BenchmarkRunner
{
    [ShortRunJob]
    public class FormulaGrouping
    {
        //
        // a*a - b*b
        //

        private float[] _a;
        private float[] _b;
        private float[] _r;

        [Params(512)]
        public int Count { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _a = new float[Count];
            _b = new float[Count];
            _r = new float[Count];

            for (int i = 0; i < Count; i++)
            {
                _a[i] = i + 666;
                _b[i] = (Count-i);
            }
        }

        [Benchmark(Baseline = true)]
        public void SingleStep()
        {
            ref Vector<float> aa = ref Unsafe.As<float, Vector<float>>(ref _a[0]);
            ref Vector<float> bb = ref Unsafe.As<float, Vector<float>>(ref _b[0]);
            ref Vector<float> rr = ref Unsafe.As<float, Vector<float>>(ref _b[0]);

            int n = Count / Vector<float>.Count;

            for (int i = 0; i < n; i++)
            {
                Vector<float> a = Unsafe.Add(ref aa, i);
                Vector<float> b = Unsafe.Add(ref bb, i);

                a *= a;
                b *= b;

                Unsafe.Add(ref rr, i) = a - b;
            }
        }

        [Benchmark]
        public void SingleStep_G4()
        {
            ref Vector<float> aa = ref Unsafe.As<float, Vector<float>>(ref _a[0]);
            ref Vector<float> bb = ref Unsafe.As<float, Vector<float>>(ref _b[0]);
            ref Vector<float> rr = ref Unsafe.As<float, Vector<float>>(ref _r[0]);

            int n = Count / Vector<float>.Count;

            for (int i = 0; i < n; i+=4)
            {
                Vector<float> a0 = Unsafe.Add(ref aa, i);
                Vector<float> a1 = Unsafe.Add(ref aa, i+1);
                Vector<float> a2 = Unsafe.Add(ref aa, i+2);
                Vector<float> a3 = Unsafe.Add(ref aa, i+3);

                Vector<float> b0 = Unsafe.Add(ref bb, i);
                Vector<float> b1 = Unsafe.Add(ref bb, i+1);
                Vector<float> b2 = Unsafe.Add(ref bb, i+2);
                Vector<float> b3 = Unsafe.Add(ref bb, i+3);

                a0 *= a0;
                a1 *= a1;
                a2 *= a2;
                a3 *= a3;
                
                b0 *= b0;
                b1 *= b1;
                b2 *= b2;
                b3 *= b3;

                Unsafe.Add(ref rr, i) = a0 - b0;
                Unsafe.Add(ref rr, i+1) = a1 - b1;
                Unsafe.Add(ref rr, i+2) = a2 - b2;
                Unsafe.Add(ref rr, i+3) = a3 - b3;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CalculatePlz(ref Vector<float> aa, ref Vector<float> bb, ref Vector<float> rr, int i)
        {
            Vector<float> a = Unsafe.Add(ref aa, i);
            Vector<float> b = Unsafe.Add(ref bb, i);
            a *= a;
            b *= b;
            Unsafe.Add(ref rr, i) = a - b;
        }

        [Benchmark]
        public void SingleStep_G4_Clean()
        {
            ref Vector<float> aa = ref Unsafe.As<float, Vector<float>>(ref _a[0]);
            ref Vector<float> bb = ref Unsafe.As<float, Vector<float>>(ref _b[0]);
            ref Vector<float> rr = ref Unsafe.As<float, Vector<float>>(ref _r[0]);

            int n = Count / Vector<float>.Count;

            for (int i = 0; i < n; i += 4)
            {
                CalculatePlz(ref aa, ref bb, ref rr, i);
                CalculatePlz(ref aa, ref bb, ref rr, i + 1);
                CalculatePlz(ref aa, ref bb, ref rr, i + 2);
                CalculatePlz(ref aa, ref bb, ref rr, i + 3);
            }
        }

        [Benchmark]
        public void Grouped()
        {
            ref Vector<float> aa = ref Unsafe.As<float, Vector<float>>(ref _a[0]);
            ref Vector<float> bb = ref Unsafe.As<float, Vector<float>>(ref _b[0]);
            ref Vector<float> rr = ref Unsafe.As<float, Vector<float>>(ref _r[0]);

            int n = Count / Vector<float>.Count;

            for (int i = 0; i < n; i++)
            {
                ref Vector<float> pA = ref Unsafe.Add(ref aa, i);
                Vector<float> a = pA;
                pA = a * a;
            }

            for (int i = 0; i < n; i++)
            {
                ref Vector<float> pB = ref Unsafe.Add(ref bb, i);
                Vector<float> b = pB;
                pB = b * b;
            }

            for (int i = 0; i < n; i++)
            {
                Vector<float> a = Unsafe.Add(ref aa, i);
                Vector<float> b = Unsafe.Add(ref bb, i);
                
                Unsafe.Add(ref rr, i) = a - b;
            }
        }

        [Benchmark]
        public void Grouped_G4()
        {
            ref Vector<float> aa = ref Unsafe.As<float, Vector<float>>(ref _a[0]);
            ref Vector<float> bb = ref Unsafe.As<float, Vector<float>>(ref _b[0]);
            ref Vector<float> rr = ref Unsafe.As<float, Vector<float>>(ref _r[0]);

            int n = Count / Vector<float>.Count;

            SquareAll(n, ref aa);
            SquareAll(n, ref bb);
            
            for (int i = 0; i < n; i+=4)
            {
                Vector<float> a0 = Unsafe.Add(ref aa, i);
                Vector<float> a1 = Unsafe.Add(ref aa, i+1);
                Vector<float> a2 = Unsafe.Add(ref aa, i+2);
                Vector<float> a3 = Unsafe.Add(ref aa, i+3);

                Vector<float> b0 = Unsafe.Add(ref bb, i);
                Vector<float> b1 = Unsafe.Add(ref bb, i + 1);
                Vector<float> b2 = Unsafe.Add(ref bb, i + 2);
                Vector<float> b3 = Unsafe.Add(ref bb, i + 3);
                
                Unsafe.Add(ref rr, i) = a0 - b0;
                Unsafe.Add(ref rr, i+1) = a1 - b1;
                Unsafe.Add(ref rr, i+2) = a2 - b2;
                Unsafe.Add(ref rr, i+3) = a3 - b3;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SquareAll(int n, ref Vector<float> aa)
        {
            for (int i = 0; i < n; i += 4)
            {
                ref Vector<float> pA0 = ref Unsafe.Add(ref aa, i);
                ref Vector<float> pA1 = ref Unsafe.Add(ref aa, i + 1);
                ref Vector<float> pA2 = ref Unsafe.Add(ref aa, i + 2);
                ref Vector<float> pA3 = ref Unsafe.Add(ref aa, i + 3);

                Vector<float> a0 = pA0;
                Vector<float> a1 = pA1;
                Vector<float> a2 = pA2;
                Vector<float> a3 = pA3;

                pA0 = a0 * a0;
                pA1 = a1 * a1;
                pA2 = a2 * a2;
                pA3 = a3 * a3;
            }
        }
    }
}