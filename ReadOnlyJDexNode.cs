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
using System.Collections;
using System.Collections.Generic;

namespace JDex {
    /// <summary>Read-only JDex node of values and child nodes</summary>
    public sealed class ReadOnlyJDexNode : IReadOnlyJDexNode<ReadOnlyJDexNode> {

        private JDexNode _item;

        /// <summary>Gets the value at the specified <paramref name="index"/></summary>
        /// <param name="index">The zero-based index of the value to get or set</param>
        /// <returns>The value at the specified <paramref name="index"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="ValueCount"/></exception>
        public JDexValue this[int i] => _item[i];
        /// <summary>Gets the <see cref="ReadOnlyJDexNodeList"/> at the specified <paramref name="key"/></summary>
        /// <param name="key">The non-null key of the list to get</param>
        /// <returns>The <see cref="JDexNode.JDexNodeList"/> at the specified <paramref name="key"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        public ReadOnlyJDexNodeList this[string key] => _item[key].AsReadOnly( );
        /// <summary>Gets the <see cref="ReadOnlyJDexNode"/> at the specified <paramref name="key"/> and <paramref name="index"/></summary>
        /// <param name="key">The non-null key of value to get or set</param>
        /// <param name="index">The zero-based index of the node to get or set</param>
        /// <returns>The node at the specified <paramref name="key"/> and <paramref name="index"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="ReadOnlyJDexNodeList.Count"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        public ReadOnlyJDexNode this[string key, int i] => _item[key, i].AsReadOnly( );

        /// <summary>Gets the number of <see cref="ReadOnlyJDexNode"/> in this <see cref="ReadOnlyJDexNode"/></summary>
        public int Count => _item.Count;
        /// <summary>Gets the number of values in this <see cref="ReadOnlyJDexNode"/></summary>
        public int ValueCount => _item.ValueCount;
        /// <summary>Gets the key of this <see cref="ReadOnlyJDexNode"/>. This will be <see langword="null"/> if <see cref="Parent"/> == <see langword="null"/></summary>
        public string Key => _item.Key;
        /// <summary>Gets the parent node of this <see cref="ReadOnlyJDexNode"/>. This will be <see langword="null"/> if not a child of another <see cref="ReadOnlyJDexNode"/></summary>
        public ReadOnlyJDexNode Parent => _item.Parent.AsReadOnly( );

        /// <summary>Initializes a new instance of <see cref="ReadOnlyJDexNode"/> and wraps around <paramref name="node"/> to create a read-only interface</summary>
        /// <param name="node">The node to wrap to create a read-only interface</param>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/></exception>
        public ReadOnlyJDexNode(JDexNode node) => _item = node ?? throw new ArgumentNullException(nameof(node));

        /// <summary>Converts this <see cref="ReadOnlyJDexNode"/> into an instance of <see cref="string"/>. 
        /// Will not append <see cref="ReadOnlyJDexNode"/> node values to JDex formated string</summary>
        /// <returns>A string whose value is the JDex formated string form of this instance</returns>
        public override string ToString( ) => _item.ToString( );
        public override bool Equals(object obj) {
            return obj is ReadOnlyJDexNode readOnlyNode && EqualityComparer<JDexNode>.Default.Equals(_item, readOnlyNode._item) ||
                obj is JDexNode node && EqualityComparer<JDexNode>.Default.Equals(_item, node);
        }
        public override int GetHashCode( ) => _item.GetHashCode( );

        /// <summary>Determins if the specified path exists in the specified <see cref="ReadOnlyJDexNode"/>. Constructing the path 
        /// through multiple params or using ':' in a string will denote for the next node key. '#' at the end of the key string
        /// with a zero-based index number for index of the node, without this by default specified as 0 index</summary>
        /// <param name="root">The node to traverse through with the specified <paramref name="path"/></param>
        /// <param name="path">The path to traverse with. Each new param is equivalent path for the next node</param>
        /// <returns><see langword="true"/> if the specified <paramref name="path"/> exists in this <see cref="ReadOnlyJDexNode"/>; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentException"><paramref name="path"/> is invalid key path format</exception>
        public static bool HasPath(ReadOnlyJDexNode root, params string[ ] path) => JDexNode.HasPath(root._item, path);
        /// <summary>Returns the node from the specified path of the specified <see cref="ReadOnlyJDexNode"/>. Constructing the path 
        /// through multiple params or using ':' in a string will denote for the next node key. '#' at the end of the key string
        /// with a zero-based index number for index of the node, without this by default specified as 0 index</summary>
        /// <param name="root">The node to traverse through with the specified <paramref name="path"/></param>
        /// <param name="path">The path to traverse with. Each new param is equivalent path for the next node</param>
        /// <returns>The <see cref="ReadOnlyJDexNode"/> from the specified node through the specified <paramref name="path"/> in this <see cref="ReadOnlyJDexNode"/></returns>
        /// <exception cref="ArgumentException"><paramref name="path"/> is invalid key path format</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="node"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="ReadOnlyJDexNodeList.Count"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        public static ReadOnlyJDexNode PathThrough(ReadOnlyJDexNode root, params string[ ] path) => JDexNode.PathThrough(root._item, path).AsReadOnly( );

        // Operators for comparing read-only JDexNodes to normal read-write JDexNodes
        public static bool operator ==(ReadOnlyJDexNode left, JDexNode right) => left._item == right;
        public static bool operator ==(JDexNode left, ReadOnlyJDexNode right) => left == right._item;
        public static bool operator ==(ReadOnlyJDexNode left, ReadOnlyJDexNode right) => left._item == right._item;
        public static bool operator !=(ReadOnlyJDexNode left, JDexNode right) => left._item != right;
        public static bool operator !=(JDexNode left, ReadOnlyJDexNode right) => left != right._item;
        public static bool operator !=(ReadOnlyJDexNode left, ReadOnlyJDexNode right) => left._item != right._item;

        /// <summary>List of read-only nodes inside of <see cref="ReadOnlyJDexNode"/></summary>
        public sealed class ReadOnlyJDexNodeList : IReadOnlyList<ReadOnlyJDexNode> {

            internal JDexNode.JDexNodeList _list;

            /// <summary>Gets the node at the specified <paramref name="index"/></summary>
            /// <param name="index">The zero-based index of the node to get</param>
            /// <returns>The node at the specified <paramref name="index"/></returns>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="Count"/></exception>
            public ReadOnlyJDexNode this[int index] => _list[index].AsReadOnly( );
            /// <summary>Gets the number of <see cref="ReadOnlyJDexNode"/> in the <see cref="ReadOnlyJDexNodeList"/></summary>
            public int Count => _list.Count;
            /// <summary></summary>
            public ReadOnlyJDexNode Parent => _list.Parent.AsReadOnly( );

            internal ReadOnlyJDexNodeList(JDexNode.JDexNodeList list) => _list = list ?? throw new ArgumentNullException(nameof(list));

            /// <summary>Returns an enumerator that iterates through </summary>
            /// <returns></returns>
            public IEnumerator<ReadOnlyJDexNode> GetEnumerator( ) {
                foreach(var item in _list)
                    yield return item.AsReadOnly( );
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator( ) => GetEnumerator( );

        }

    }

}
