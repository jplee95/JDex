using JDex;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JDexTest {

    [TestClass]
    public class JDexReaderWriter {
        public static readonly string TEST_STRING =
                "something: \"Hello World\"\n" +
                "\tkey1: \"this\", \"something else\"\n" +
                "\tkey2: \"1\"\n" +
                "\t\tother_list:\n" +
                "\t\t\ta: \"1\"\n" +
                "\t\t\tb: \"1\"\n" +
                "\t\t\tc: \"1\"\n" +
                "\t\tset:\n" +
                "\t\t\tcube: index: \"1\"\n" +
                "\t\t\tcube: index: \"2\"\n" +
                "\t\t\tcube: index: \"4\"\n" +
                "net.ry:\n" +
                "\tother: \"something\"";

        [TestMethod]
        [DeploymentItem("..\\..\\..\\Resources\\test.jdex", ".")]
        public void JDexReaderTest( ) {
            using var reader = new JDexReader("test.jdex");
            Assert.AreEqual(JDexNode.Parse(TEST_STRING).ToString( ), reader.ReadToEnd( ).ToString( ));
        }

        [TestMethod]
        public void JDexWriterTest( ) {
            if(File.Exists("writer_test.jdex")) File.Delete("writer_test.jdex");

            var node = JDexNode.Parse(TEST_STRING);
            using(var writer = new JDexWriter("writer_test.jdex"))
                writer.Write(node);
            using(var file = new StreamReader("writer_test.jdex"))
                Assert.AreEqual(node.ToString( ) + "\n", file.ReadToEnd( ));
        }

    }
}
