using NUnit.Framework;
using Pixlpark.Text;

namespace Tests
{
    public class StandardReplacerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ReplaceSingle()
        {
            var input = "[$ data $] asdfasdfasdfasdf";
            var result = StandardReplacer.Replace(input, token => "data");
            Assert.AreEqual("data asdfasdfasdfasdf", result);
        }

        [Test]
        public void ReplaceMultiple()
        {
            var input = "[$ data1 $][$ data2 $] asdfasdfasdfasdf[$ data3 $]";
            var result = StandardReplacer.Replace(input, token =>
            {
                if (token == " data1 ") return "data1";
                if (token == " data2 ") return "data2";
                if (token == " data3 ") return "data3";
                return token;
            });
            Assert.AreEqual("data1data2 asdfasdfasdfasdfdata3", result);
        }

        [Test]
        public void ReplaceRecursive()
        {
            var input = "[$ data $] asdfasdfasdfasdf";

            var result = StandardReplacer.Replace(input, token =>
            {
                if (token == " data ") return "in [$ inside $]";
                if (token == " inside ") return "bum!";
                return null;
            });
            Assert.AreEqual("in bum! asdfasdfasdfasdf", result);
        }
    }
}