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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JDexTest {

    [TestClass]
    public class JDexFunctions {

        [TestMethod]
        public void JDexNodeAddSetRemoveTest( ) {
            var root = new JDexNode( );
            var node1 = new JDexNode( );
            Assert.AreEqual(null, node1.Key);
            Assert.AreEqual(null, node1.Parent);

            root.Add("key", node1);
            Assert.AreEqual(1, root.Count);
            Assert.AreEqual("key", node1.Key);
            Assert.AreEqual(root, node1.Parent);

            Assert.IsTrue(root.Remove(node1));
            Assert.AreEqual(null, node1.Key);
            Assert.AreEqual(null, node1.Parent);

            root.Add("key", node1);
            var node2 = new JDexNode( );
            root.Add("key2", node2);
            Assert.AreEqual(2, root.Count);

            root.Clear( );
            Assert.IsFalse(root.ContainsKey("key"));
            Assert.AreEqual(null, node1.Key);
            Assert.AreEqual(null, node1.Parent);

            Assert.AreEqual(null, node2.Key);
            Assert.AreEqual(null, node2.Parent);

            root.Add("key", node1);
            root.Add("key", node2);
            root.RemoveAt("key", 1);
            Assert.AreEqual(null, node2.Key);
            Assert.AreEqual(null, node2.Parent);

            Assert.IsTrue(root.Remove("key"));
            Assert.AreEqual(0, root.Count);
            Assert.AreEqual(null, node1.Key);
            Assert.AreEqual(null, node1.Parent);
            Assert.IsFalse(root.Remove("key"));
        }

        [TestMethod]
        public void JDexNodeListAddSetRemoveTest( ) {
            var root = new JDexNode( );
            var node1 = new JDexNode( );
            root.Add("key", node1);
            var group = root["key"];

            var node2 = new JDexNode( );
            group.Add(node2);
            Assert.AreEqual(2, group.Count);
            Assert.AreEqual("key", node2.Key);
            Assert.AreEqual(root, node2.Parent);

            group.Remove(node2);
            Assert.AreEqual(1, group.Count);
            Assert.AreEqual(null, node2.Key);
            Assert.AreEqual(null, node2.Parent);

            group.Clear( );
            Assert.AreEqual(0, group.Count);
            Assert.IsFalse(root.ContainsKey("key"));
            Assert.AreEqual(null, node1.Key);
            Assert.AreEqual(null, node1.Parent);
        }

        [TestMethod]
        public void JDexNodeEntryQueryTest( ) {
            var root = new JDexNode( );
            var node1 = new JDexNode( );
            root.Add("node1", node1);
            node1.Add("empty_node", new JDexNode( ));

            var empty_node = new JDexNode( );
            node1.Add("empty_node", empty_node);

            var node2 = new JDexNode( );
            root.Add("node2", node2);
            var node3 = new JDexNode( );
            node2.Add("node", node3);
            node3.Add("node", new JDexNode( ));

            Assert.AreEqual(2, root.Count);
            Assert.AreEqual(2, node1.Count);
            Assert.AreEqual(1, node2.Count);
            Assert.AreEqual(1, node3.Count);

            Assert.IsTrue(root.ContainsKey("node1"));
            Assert.IsFalse(root.ContainsKey("node"));
            Assert.IsTrue(root.ContainsNode(node2));
            Assert.IsFalse(root.ContainsNode(node3));

            Assert.AreEqual(empty_node, node1["empty_node", 1]);
            Assert.AreEqual(node3, JDexNode.PathThrough(root, "node2:node"));

            Assert.IsTrue(JDexNode.HasPath(root, "node1"));
            Assert.IsTrue(JDexNode.HasPath(root, "node1:empty_node"));
            Assert.IsTrue(JDexNode.HasPath(root, "node1:empty_node#1"));
            Assert.IsTrue(JDexNode.HasPath(root, "node2:node:node"));

            Assert.AreEqual("node1:\n\tempty_node:\n\tempty_node:\nnode2:\n\tnode:\n\t\tnode:", root.ToString( ));

            Assert.IsTrue(root.TryGetNode("node1", 0, out var node));
            Assert.AreEqual(node1, node);

            var readOnlyNode = root.AsReadOnly( );
            Assert.IsTrue(readOnlyNode == root);
            Assert.IsTrue(readOnlyNode != node1);
            Assert.IsTrue(root == readOnlyNode);
            Assert.IsTrue(node1 != readOnlyNode);
            Assert.IsTrue(root.AsReadOnly( ) == readOnlyNode);
            Assert.IsTrue(node1.AsReadOnly( ) != readOnlyNode);

            Assert.IsTrue(ReadOnlyJDexNode.HasPath(readOnlyNode, "node1:empty_node"));
            Assert.IsTrue(ReadOnlyJDexNode.HasPath(readOnlyNode, "node1:empty_node#1"));
            Assert.IsTrue(ReadOnlyJDexNode.HasPath(readOnlyNode, "node2:node:node"));

            Assert.AreEqual(2, readOnlyNode.Count);
            Assert.AreEqual(root.AsReadOnly( ), ReadOnlyJDexNode.PathThrough(readOnlyNode, "node1").Parent);
            Assert.AreEqual(node1.Key, ReadOnlyJDexNode.PathThrough(readOnlyNode, "node1").Key);
            Assert.AreEqual(node1.Count, ReadOnlyJDexNode.PathThrough(readOnlyNode, "node1").Count);
            Assert.AreEqual(node2.Count, ReadOnlyJDexNode.PathThrough(readOnlyNode, "node2").Count);
            Assert.AreEqual(node3.Count, ReadOnlyJDexNode.PathThrough(readOnlyNode, "node2:node").Count);

            Assert.AreEqual(node1.AsReadOnly( ), readOnlyNode["node1", 0]);
            var readOnlyGroup = readOnlyNode["node1"];
            Assert.AreEqual(1, readOnlyGroup.Count);
            Assert.AreEqual(node1.AsReadOnly( ), readOnlyGroup[0]);
            Assert.AreEqual(root.AsReadOnly( ), readOnlyGroup.Parent);
        }

        [TestMethod]
        public void JDexNodeValueTest( ) {
            var node = new JDexNode( );
            node.AddValue("value 1");
            node.AddValue("value 3");
            Assert.AreEqual(2, node.ValueCount);
            Assert.AreEqual("value 1", node[0]);
            Assert.AreEqual("value 3", node[1]);

            node.InsertValue(1, "value 2");
            Assert.AreEqual(3, node.ValueCount);
            Assert.AreEqual("value 1", node[0]);
            Assert.AreEqual("value 2", node[1]);
            Assert.AreEqual("value 3", node[2]);

            var root = new JDexNode( );
            root.Add("node", node);
            Assert.AreEqual("node: \"value 1\", \"value 2\", \"value 3\"", root.ToString( ));

            Assert.IsTrue(node.RemoveValue("value 1"));
            Assert.IsFalse(node.RemoveValue("value 1"));
            Assert.AreEqual(2, node.ValueCount);
            Assert.AreEqual("value 2", node[0]);
            Assert.AreEqual("value 3", node[1]);

            node.RemoveValueAt(0);
            Assert.AreEqual(1, node.ValueCount);
            Assert.AreEqual("value 3", node[0]);

            node.AddValueRange(new JDexValue[ ] { "value 4", "value 5" });
            Assert.AreEqual(3, node.ValueCount);
            Assert.AreEqual("value 3", node[0]);
            Assert.AreEqual("value 4", node[1]);
            Assert.AreEqual("value 5", node[2]);

            node.InsertValueRange(0, new JDexValue[ ] { "value 1", "value 2" });
            Assert.AreEqual(5, node.ValueCount);
            Assert.AreEqual("value 1", node[0]);
            Assert.AreEqual("value 2", node[1]);
            Assert.AreEqual("value 3", node[2]);
            Assert.AreEqual("value 4", node[3]);
            Assert.AreEqual("value 5", node[4]);

            node[3] = "value 9";
            Assert.AreEqual("value 9", node[3]);

            var readOnlyNode = node.AsReadOnly( );
            Assert.AreEqual(5, readOnlyNode.ValueCount);
            Assert.AreEqual("value 1", readOnlyNode[0]);
            Assert.AreEqual("value 2", readOnlyNode[1]);
            Assert.AreEqual("value 3", readOnlyNode[2]);
            Assert.AreEqual("value 9", readOnlyNode[3]);
            Assert.AreEqual("value 5", readOnlyNode[4]);

            node.RemoveValueRange(3, 2);
            Assert.AreEqual(3, node.ValueCount);
            Assert.AreEqual("value 1", node[0]);
            Assert.AreEqual("value 2", node[1]);
            Assert.AreEqual("value 3", node[2]);

            node.ClearValues( );
            Assert.AreEqual(0, node.ValueCount);
        }

        [TestMethod]
        public void JDexNodeParserTest( ) {
            var jdexString = "#some comment\nnode: \"Something\", \"Another\"\n\titem: \"Tag\" next: \"Other\"";
            var node = JDexNode.Parse(jdexString);

            Assert.IsTrue(JDexNode.HasPath(node, "node"));
            Assert.IsTrue(JDexNode.HasPath(node, "node:item"));

            var innerNode = JDexNode.PathThrough(node, "node");
            Assert.AreEqual("Something", innerNode[0]);
            Assert.AreEqual("Another", innerNode[1]);

            var innerItem = JDexNode.PathThrough(node, "node:item");
            Assert.AreEqual("Tag", innerItem[0]);

            var innerNext = JDexNode.PathThrough(node, "node:item:next");
            Assert.AreEqual("Other", innerNext[0]);

            var jdexStringValues = "Int: 1234567890\nSign: -76891234\nHex: 0x00f022AA\nBinary: 0b01010001\nFloat: .0111\nExponent: -12.1e-2\nTrue: true\nFalse: false";
            var valueNode = JDexNode.Parse(jdexStringValues);

            Assert.AreEqual(JDexValueType.Int, valueNode["Int", 0][0].Type);
            Assert.AreEqual(1234567890, valueNode["Int", 0][0]);

            Assert.AreEqual(JDexValueType.Int, valueNode["Hex", 0][0].Type);
            Assert.AreEqual(0x00f022AA, valueNode["Hex", 0][0]);

            Assert.AreEqual(JDexValueType.Int, valueNode["Binary", 0][0].Type);
            Assert.AreEqual(0b01010001, valueNode["Binary", 0][0]);

            Assert.AreEqual(JDexValueType.Float, valueNode["Float", 0][0].Type);
            Assert.AreEqual(.0111f, valueNode["Float", 0][0]);

            Assert.AreEqual(JDexValueType.Float, valueNode["Exponent", 0][0].Type);
            Assert.AreEqual(-12.1e-2f, valueNode["Exponent", 0][0]);

            Assert.AreEqual(JDexValueType.Bool, valueNode["True", 0][0].Type);
            Assert.AreEqual(true, valueNode["True", 0][0]);

            Assert.AreEqual(JDexValueType.Bool, valueNode["False", 0][0].Type);
            Assert.AreEqual(false, valueNode["False", 0][0]);
        }

    }
}
