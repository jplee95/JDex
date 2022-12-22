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

using System;
using System.Collections.Generic;
using JDex;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JDexTest {

    [TestClass]
    public class JDexExceptions {

        [TestMethod]
        public void JDexValueTest( ) {
            JDexValue value = "string";

            Assert.ThrowsException<ArgumentNullException>(( ) => value = (string) null);
            Assert.ThrowsException<InvalidCastException>(( ) => _ = (bool) value);
            Assert.ThrowsException<InvalidCastException>(( ) => _ = (int) value);
            Assert.ThrowsException<InvalidCastException>(( ) => _ = (float) value);

            value = 12;
            Assert.ThrowsException<InvalidCastException>(( ) => _ = (string) value);
        }

        [TestMethod]
        public void JDexNodeTest( ) {
            var root = new JDexNode( );
            root.AddValue("First");
            root.Add("node", new JDexNode( ));

            Assert.ThrowsException<ArgumentNullException>(( ) => _ = new ReadOnlyJDexNode(null));

            Assert.ThrowsException<ArgumentNullException>(( ) => root[0] = null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => _ = root[1]);

            Assert.ThrowsException<ArgumentInvalidException>(( ) => _ = root["@@@"]);
            Assert.ThrowsException<ArgumentNullException>(( ) => _ = root[null]);
            Assert.ThrowsException<KeyNotFoundException>(( ) => _ = root["key"]);

            Assert.ThrowsException<ArgumentCircularBranchException>(( ) => root["node", 0] = root);
            Assert.ThrowsException<ArgumentInvalidException>(( ) => _ = root["@@@", 0]);
            Assert.ThrowsException<ArgumentNullException>(( ) => _ = root[null, 0]);
            Assert.ThrowsException<ArgumentNullException>(( ) => root["node", 0] = null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => _ = root["node", -1]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => _ = root["node", 2]);
            Assert.ThrowsException<KeyNotFoundException>(( ) => _ = root["key", 0]);

            Assert.ThrowsException<ArgumentCircularBranchException>(( ) => root["node", 0].Add("test", root));
            Assert.ThrowsException<ArgumentInvalidException>(( ) => root.Add("@@@", new JDexNode( )));
            Assert.ThrowsException<ArgumentNullException>(( ) => root.Add(null, new JDexNode( )));
            Assert.ThrowsException<ArgumentNullException>(( ) => root.Add("node", null));

            Assert.ThrowsException<ArgumentNullException>(( ) => root.AddValue(null));

            Assert.ThrowsException<ArgumentNullException>(( ) => root.AddValueRange(null));
            Assert.ThrowsException<ArgumentNullException>(( ) => root.AddValueRange(new JDexValue[ ] { null }));

            Assert.ThrowsException<ArgumentInvalidException>(( ) => root.ContainsKey("@@@"));
            Assert.ThrowsException<ArgumentNullException>(( ) => root.ContainsKey(null));

            Assert.ThrowsException<ArgumentNullException>(( ) => root.ContainsNode(null));

            Assert.ThrowsException<ArgumentNullException>(( ) => root.InsertValue(0, null));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.InsertValue(-1, "value"));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.InsertValue(2, "value"));

            Assert.ThrowsException<ArgumentNullException>(( ) => root.InsertValueRange(0, null));
            Assert.ThrowsException<ArgumentNullException>(( ) => root.InsertValueRange(0, new JDexValue[ ] { null }));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.InsertValueRange(-1, new JDexValue[ ] { "value" }));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.InsertValueRange(2, new JDexValue[ ] { "value" }));

            Assert.ThrowsException<ArgumentNullException>(( ) => root.Remove((JDexNode) null));

            Assert.ThrowsException<ArgumentInvalidException>(( ) => root.Remove("@@@"));
            Assert.ThrowsException<ArgumentNullException>(( ) => root.Remove((string) null));

            Assert.ThrowsException<ArgumentInvalidException>(( ) => root.RemoveAt("@@@", 0));
            Assert.ThrowsException<ArgumentNullException>(( ) => root.RemoveAt(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveAt("node", -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveAt("node", 2));

            Assert.ThrowsException<ArgumentNullException>(( ) => root.RemoveValue(null));

            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveValueAt(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveValueAt(2));

            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveValueRange(-1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveValueRange(2, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveValueRange(0, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveValueRange(1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => root.RemoveValueRange(0, 2));

            Assert.ThrowsException<ArgumentInvalidException>(( ) => root.TryGetNode("@@@", 0, out _));
            Assert.ThrowsException<ArgumentNullException>(( ) => root.TryGetNode(null, 0, out _));

            Assert.ThrowsException<ArgumentInvalidException>(( ) => JDexNode.HasPath(root, "@@@"));
            Assert.ThrowsException<ArgumentNullException>(( ) => JDexNode.HasPath(null, "node"));
            Assert.ThrowsException<ArgumentNullException>(( ) => JDexNode.HasPath(root, null));

            Assert.ThrowsException<ArgumentInvalidException>(( ) => JDexNode.PathThrough(root, "@@@"));
            Assert.ThrowsException<ArgumentNullException>(( ) => JDexNode.PathThrough(null, "node"));
            Assert.ThrowsException<ArgumentNullException>(( ) => JDexNode.PathThrough(root, null));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => JDexNode.PathThrough(root, "node#1"));
            Assert.ThrowsException<KeyNotFoundException>(( ) => JDexNode.PathThrough(root, "key"));
        }

        [TestMethod]
        public void JDexNodeListTest( ) {
            var root = new JDexNode( );
            root.Add("node", new JDexNode( ));
            var group = root["node"];

            Assert.ThrowsException<ArgumentCircularBranchException>(( ) => group[0] = root);
            Assert.ThrowsException<ArgumentNullException>(( ) => group[0] = null);
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => _ = group[-1]);
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => _ = group[2]);

            Assert.ThrowsException<ArgumentCircularBranchException>(( ) => group.Add(root));
            Assert.ThrowsException<ArgumentNullException>(( ) => group.Add(null));

            Assert.ThrowsException<ArgumentNullException>(( ) => group.Contains(null));

            Assert.ThrowsException<ArgumentNullException>(( ) => group.IndexOf(null, 0, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.IndexOf(new JDexNode( ), -1, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.IndexOf(new JDexNode( ), 2, 1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.IndexOf(new JDexNode( ), 1, -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.IndexOf(new JDexNode( ), 0, -1));

            Assert.ThrowsException<ArgumentNullException>(( ) => group.IndexOf(null, 0));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.IndexOf(new JDexNode( ), -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.IndexOf(new JDexNode( ), 2));

            Assert.ThrowsException<ArgumentNullException>(( ) => group.IndexOf(null));

            Assert.ThrowsException<ArgumentNullException>(( ) => group.Insert(0, null));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.Insert(-1, new JDexNode( )));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.Insert(2, new JDexNode( )));

            Assert.ThrowsException<ArgumentNullException>(( ) => group.Remove(null));

            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.RemoveAt(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(( ) => group.RemoveAt(2));
        }

        [TestMethod]
        public void JDexNodeParserTest( ) {
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse(","));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string:, \"\","));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string.:"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse(".string:"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string:,"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string: \"value\","));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string#"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse(":string"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string: \"value\" \"\""));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string: \"value\"\""));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string: \"value"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string: \"str\\\"\", \""));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string: \"str\" node"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string: \"str\", node"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("string: \"str\", \" #node"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("\t string: \"value\""));

            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: ."));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0."));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0.e"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: e0"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0e."));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0e23."));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: .000.123"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0x"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0x311DC11A11"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0b"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0b101010100010010101010100101011010001"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0x56.23"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0x56G23"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0b11.01"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 0b11401"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: -0x34F"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: +0b1010"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("number: 34276045389747568"));
            
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("boolean: tre"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("boolean: fale"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("boolean: false true"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("boolean: TRUE"));
            Assert.ThrowsException<JDexParseException>(( ) => JDexNode.Parse("boolean: FALSE"));
        }

    }
}
