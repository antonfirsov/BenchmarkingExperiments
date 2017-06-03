using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace BenchmarkRunner
{
    [Config(typeof(TestConfig))]
    public class TestCoreCompatDrawing
    {
        [Benchmark]
        public void CreateBitmap()
        {
            using (var bitmap = new Bitmap(10, 10))
            {
                bitmap.SetPixel(0, 0, Color.Red);
            }
        }

    }
}