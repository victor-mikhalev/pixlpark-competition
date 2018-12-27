using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using Pixlpark.Text;

namespace Competition
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private const int IterationsCount = 1_000;
        private static IReadOnlyDictionary<string, string> ReplacementData;
        private static string InputData;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var str = System.IO.File.ReadAllText(System.Environment.CurrentDirectory + "\\data\\replacements.json", Encoding.UTF8);
            ReplacementData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
            InputData = System.IO.File.ReadAllText(System.Environment.CurrentDirectory + "\\data\\input1.html", Encoding.UTF8);
        }

        [Benchmark]
        public static void RunStandardReplacer()
        {
            for (int i = 0; i < IterationsCount; i++)
            {
                StandardReplacer.Replace(InputData, token =>
                {
                    var t = "[$" + token + "$]";
                    return ReplacementData[t];
                });
            }
        }

        [Benchmark]
        public static void RunStupidReplacer()
        {
            for (int i = 0; i < IterationsCount; i++)
            {
                StupidReplacer.Replace(InputData, token =>
                {
                    var t = "[$" + token + "$]";
                    return ReplacementData[t];
                });
            }
        }
    }
}