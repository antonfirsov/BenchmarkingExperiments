namespace BenchmarkRunner
{
    using System;
    using System.Numerics;
    using System.Runtime.CompilerServices;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Attributes.Jobs;
    
    [ClrJob]
    [CoreJob]
    public class WidenBits
    {
        private const int Count = 64;

        private byte[] source = new byte[Count];
        private ushort[] dest = new ushort[Count];

        public WidenBits()
        {
            if (Vector<byte>.Count != Vector<ushort>.Count * 2)
            {
                throw new Exception("Something went really wrong!");
            }
        }

        [Benchmark(Baseline = true)]
        public void Standard()
        {
            ref byte sourceBaseRef = ref this.source[0];
            ref ushort destBaseRef = ref this.dest[0];

            for (int i = 0; i < Count; i++)
            {
                ref byte s = ref Unsafe.Add(ref sourceBaseRef, i);
                ref ushort d = ref Unsafe.Add(ref destBaseRef, i);
                d = s;
            }    
        }

        [Benchmark]
        public void SIMD()
        {
            ref Vector<byte> sourceBaseRef = ref Unsafe.As<byte, Vector<byte>>(ref this.source[0]);
            ref Vector<ushort> destBaseRef = ref Unsafe.As<ushort, Vector<ushort>>(ref this.dest[0]);

            int n = Count / Vector<byte>.Count;

            for (int i = 0; i < n; i++)
            {
                Vector<byte> s = Unsafe.Add(ref sourceBaseRef, i);

                Vector<ushort> low;
                Vector<ushort> high;

                Vector.Widen(s, out low, out high);

                Unsafe.Add(ref destBaseRef, i * 2) = low;
                Unsafe.Add(ref destBaseRef, i * 2 + 1) = high;
            }
        }
    }
}