using System;
using BenchmarkDotNet.Running;

namespace Competition
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmarks>();
            Console.WriteLine("Benchmark complete. Press any key to close...");
            Console.ReadKey();
        }
    }
}
