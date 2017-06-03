using System;

namespace BenchmarkRunner
{
    using System.Reflection;

    using BenchmarkDotNet.Running;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Runner process framework:");
            Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);

            System.Reflection.Assembly a = typeof(Program).GetTypeInfo().Assembly;

            BenchmarkSwitcher benchmarkSwitcher = BenchmarkSwitcher.FromAssembly(a);
            benchmarkSwitcher.Run(args);
            
            Console.ReadLine();
        }
    }
}