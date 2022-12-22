// Copyright 2022 Jordan Paladino
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify,
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies
// or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using JDex;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JDexTest {

    [TestClass]
    public class JDexReaderWriter {
        public static readonly string TEST_STRING = 
            "\t# Comments can start on its own line\n" +
            "index_name: \"string value\", \"second value\"\n" +
            "\tsecond_index: \"value\" # Or be at the end of an entry\n" +
            "\t\t# Shorthand index declaration continue after the first index\n" +
            "\t\tthird_index: one: \"one\" two: \"two\"\n" +
            "\n" +
            "numeric_values:\n" +
            "\tint: -213768\n" +
            "\t# Hex and binary values can not have a sign (+ or -)\n" +
            "\thex: 0x2222\n" +
            "\tbinary: 0b100100\n" +
            "\tfloat: 0.002e-3\n" +
            "\n" +
            "# allowed values for bool: true or false\n" +
            "boolean_values:\n" +
            "\ttrue: true\n" +
            "\tfalse: false\n" +
            "\n" +
            "# You can have multiple index entries at root level\n" +
            "box_index: 2\n" +
            "\t# Index keys can be repeated\n" +
            "\tbox: \"big\"\n" +
            "\tbox: \"small\"\n" +
            "\n" +
            "# Unicode and other escape characters in value string\n" +
            "unicode_character: \"\\u0065\"\n" +
            "exscape_character: \"\\n\\t\\f\"\n";

        [TestMethod]
        //[DeploymentItem("~\\Resources\\test.jdex", ".")]
        public void JDexReaderTest( ) {
            using(var writer = new StreamWriter("test.jdex"))
                writer.WriteLine(TEST_STRING);

            using(var reader = new JDexReader("test.jdex"))
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
