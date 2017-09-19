using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace BenchmarkRunner
{
    [CoreJob]
    public class FetchVectors
    {
        private float[] _heapData;
        
        private GCHandle _heapHandle;
        private IntPtr _heapPtr;

        [StructLayout(LayoutKind.Sequential)]
        struct Float8
        {
            public float S0, S1, S2, S3, S4, S5, S6, S7;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4Pair
        {
            public Vector4 A;
            public Vector4 B;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Multiply(float value)
            {
                A *= value;
                B *= value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(float value)
            {
                Vector4 v = new Vector4(value);
                A += v;
                B += v;
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            _heapData = new float[16];
            _heapData[0] = 42;

            _heapHandle = GCHandle.Alloc(_heapData, GCHandleType.Pinned);
            _heapPtr = _heapHandle.AddrOfPinnedObject();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _heapHandle.Free();
        }

        [Benchmark(Baseline = true)]
        public float FetchAccrossRawStackData()
        {
            Float8 stackData = default(Float8);
            ref Vector<float> stackRef = ref Unsafe.As<Float8, Vector<float>>(ref stackData);
            ref Vector<float> heapRef = ref Unsafe.As<float, Vector<float>>(ref _heapData[0]);

            Vector<float> v = heapRef; // read from heap

            v *= new Vector<float>(666);
            v += new Vector<float>(555);

            stackRef = v;
            stackData.S7 = stackData.S0; // make sure we do something with the data
            v = stackRef;
            v *= new Vector<float>(555);
            heapRef = v;
            return _heapData[8];
        }

        [Benchmark]
        public unsafe float FetchAccrossRawStackData_UsePointers()
        {
            Float8 stackData = default(Float8);
            Float8* stackPtr = &stackData;
            void* heapPtr = (void*) _heapPtr;

            Vector<float> v = Unsafe.Read<Vector<float>>(heapPtr);

            v *= new Vector<float>(666);
            v += new Vector<float>(555);

            Unsafe.Write(stackPtr, v);
            stackData.S7 = stackData.S0; // make sure we do something with the data
            v = Unsafe.Read<Vector<float>>(stackPtr); ;
            v *= new Vector<float>(555);
            Unsafe.Write(heapPtr, v);
            return _heapData[8];
        }

        [Benchmark]
        public float FetchAcrossVector4Pair()
        {
            Vector4Pair stackData = default(Vector4Pair);
            ref Vector<float> stackRef = ref Unsafe.As<Vector4Pair, Vector<float>>(ref stackData);
            ref Vector<float> heapRef = ref Unsafe.As<float, Vector<float>>(ref _heapData[0]);

            Vector<float> v = heapRef; // read from heap

            v *= new Vector<float>(666);
            v += new Vector<float>(555);

            stackRef = v;
            stackData.B.W = stackData.A.X; // make sure we do something with the data
            v = stackRef;
            v *= new Vector<float>(555);
            heapRef = v;
            return _heapData[8];
        }

        [Benchmark]
        public float Vector4Only()
        {
            Vector4Pair stackData = default(Vector4Pair);
            ref Vector4Pair stackRef = ref stackData;
            ref Vector4Pair heapRef = ref Unsafe.As<float, Vector4Pair>(ref _heapData[0]);

            Vector4Pair v = heapRef; // read from heap

            v.Multiply(666);
            v.Add(555);
            
            stackRef = v;
            stackData.B.W = stackData.A.X; // make sure we do something with the data
            v = stackRef;
            v.Multiply(555);
            heapRef = v;
            return _heapData[8];
        }
    }
}