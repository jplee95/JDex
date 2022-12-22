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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace JDex {

    /// <summary>JDex node of values and child nodes</summary>
    public sealed class JDexNode : IJDexNode<JDexNode>, IReadOnlyJDexNode<ReadOnlyJDexNode> {

        private string _key;
        private JDexNode _parent;
        private List<JDexValue> _values;
        private Dictionary<string, JDexNodeList> _children;

        /// <summary>Gets or sets the value at the specified <paramref name="index"/></summary>
        /// <param name="index">The zero-based index of the value to get or set</param>
        /// <returns>The value at the specified <paramref name="index"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="ValueCount"/></exception>
        public JDexValue this[int index] {
            get => _values[index];
            set => _values[index] = value ?? throw new ArgumentNullException(nameof(value));
        }
        /// <summary>Gets the <see cref="JDexNodeList"/> at the specified <paramref name="key"/></summary>
        /// <param name="key">The non-null key of the list to get</param>
        /// <returns>The <see cref="JDexNodeList"/> at the specified <paramref name="key"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in collection</exception>
        public JDexNodeList this[string key] => _children[ValidKey(key)];
        /// <summary>Gets or sets the <see cref="JDexNode"/> at the specified <paramref name="key"/> and <paramref name="index"/></summary>
        /// <param name="key">The non-null key of the list to get or set</param>
        /// <param name="index">The zero-based index of the node to get or set</param>
        /// <returns>The node at the specified <paramref name="key"/> and <paramref name="index"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="JDexNodeList.Count"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        public JDexNode this[string key, int index] {
            get => _children[ValidKey(key)][index];
            set => _children[ValidKey(key)][index] = value ?? throw new ArgumentNullException(nameof(value));
        }

        private static string ValidKey(string key, bool path = false) {
            if(key == null) throw new ArgumentNullException(nameof(key));
            if(!ValidKeyName(key, path)) throw new ArgumentInvalidException(nameof(key), $"Invalid key name '{key}'");
            return key;
        }

        /// <summary>Gets the number of <see cref="JDexNode"/> in this <see cref="JDexNode"/></summary>
        public int Count => (from v in _children.Values.AsParallel( ) let c = v.Count select c).Sum( );
        /// <summary>Gets the number of values in this <see cref="JDexNode"/></summary>
        public int ValueCount => _values.Count;
        /// <summary>Gets the key of this <see cref="JDexNode"/>. This will be <see langword="null"/> if <see cref="Parent"/> is <see langword="null"/></summary>
        public string Key => _key;
        /// <summary>Gets the parent node of this <see cref="JDexNode"/>. This will be <see langword="null"/> if not added to another <see cref="JDexNode"/></summary>
        public JDexNode Parent => _parent;

        /// <summary>Initialzes a new instance of <see cref="JDexNode"/> that is empty</summary>
        public JDexNode( ) {
            _key = null;
            _parent = null;
            _values = new List<JDexValue>( );
            _children = new Dictionary<string, JDexNodeList>( );
        }

        /// <summary>Adds the specified <paramref name="node"/> into the children of this <see cref="JDexNode"/></summary>
        /// <param name="key">The non-null key of <paramref name="node"/> to add</param>
        /// <param name="node">The <see cref="JDexNode"/> to add</param>
        /// <exception cref="ArgumentCircularBranchException"><paramref name="node"/> is a parent node of this <see cref="JDexNode"/> or earlier</exception>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="node"/> is <see langword="null"/></exception>
        public void Add(string key, JDexNode node) {
            if(node == null) throw new ArgumentNullException(nameof(node));
            if(!_children.TryGetValue(ValidKey(key), out var list))
                list = _children[key] = new JDexNodeList(key, this);
            list.Add(node);
        }
        /// <summary>Adds the specified <paramref name="value"/> into this <see cref="JDexNode"/></summary>
        /// <param name="value">The value to add to this <see cref="JDexNode"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        public void AddValue(JDexValue value) => _values.Add(value ?? throw new ArgumentNullException(nameof(value)));
        /// <summary>Adds the specified <paramref name="collection"/> of values into this <see cref="JDexNode"/></summary>
        /// <param name="collection">The collection of values to insert into this <see cref="JDexNode"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/> or a value in <paramref name="collection"/> is <see langword="null"/></exception>
        public void AddValueRange(IEnumerable<JDexValue> collection) {
            if(collection == null)
                throw new ArgumentNullException(nameof(collection));
            if(collection.Any((JDexValue s) => s == null))
                throw new ArgumentNullException(nameof(collection), "Value in collection is null");
            _values.AddRange(collection);
        }
        /// <summary>Returns a read-only wrapper around this <see cref="JDexNode"/></summary>
        /// <returns>An object that acts as a read-only wrapper around this <see cref="JDexNode"/></returns>
        public ReadOnlyJDexNode AsReadOnly( ) => new ReadOnlyJDexNode(this);
        /// <summary>Removes all value and child nodes from this <see cref="JDexNode"/>. 
        /// This unlinks all child nodes and lists from this node</summary>
        public void Clear( ) {
            foreach(var group in _children) {
                foreach(var child in group.Value) {
                    child._key = null;
                    child._parent = null;
                }

                group.Value._key = null;
                group.Value._parent = null;
                group.Value._items.Clear( );
            }
            _children.Clear( );
        }
        /// <summary>Removes all value from this <see cref="IJDexNode{T}"/></summary>
        public void ClearValues( ) => _values.Clear( );
        /// <summary>Determins if this <see cref="JDexNode"/> has the specified <paramref name="key"/></summary>
        /// <param name="key">The non-null key to locate in this <see cref="JDexNode"/></param>
        /// <returns><see langword="true"/> if the specified key is found in this <see cref="JDexNode"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        public bool ContainsKey(string key) => _children.ContainsKey(ValidKey(key));
        /// <summary>Determins if <see cref="JDexNode"/> has the specified <paramref name="node"/></summary>
        /// <param name="node">The node to locate in this <see cref="JDexNode"/></param>
        /// <returns>true if the specified node is found in this <see cref="JDexNode"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/></exception>
        public bool ContainsNode(JDexNode node) {
            if(node == null) throw new ArgumentNullException(nameof(node));
            if(_children.TryGetValue(node._key, out var list))
                return list.Contains(node);
            return false;
        }
        /// <summary>Inserts the <paramref name="value"/> into this <see cref="JDexNode"/> at the specified <paramref name="index"/></summary>
        /// <param name="index">The zero-based index to insert value at</param>
        /// <param name="value">The value to insert into this <see cref="JDexNode"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then <see cref="ValueCount"/></exception>
        public void InsertValue(int index, JDexValue value) {
            if(value == null) throw new ArgumentNullException(nameof(value));
            if(index < 0 || index > _values.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _values.Insert(index, value);
        }
        /// <summary>Inserts the <paramref name="collection"/> of values into this <see cref="JDexNode"/> at the specified <paramref name="index"/></summary>
        /// <param name="index">The zero-based index to insert the collection of values at</param>
        /// <param name="collection">The collection of values to insert into this <see cref="JDexNode"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/> or a value in <paramref name="collection"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then <see cref="ValueCount"/></exception>
        public void InsertValueRange(int index, IEnumerable<JDexValue> collection) {
            if(index < 0 || index > _values.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if(collection == null)
                throw new ArgumentNullException(nameof(collection));
            if(collection.Any((JDexValue s) => s == null))
                throw new ArgumentNullException(nameof(collection), "Value in collection is null");
            _values.InsertRange(index, collection);
        }
        /// <summary>Remove the child <paramref name="node"/> from this <see cref="JDexNode"/></summary>
        /// <param name="node">The child node of this <see cref="JDexNode"/> to remove</param>
        /// <returns><see langword="true"/> if the child <paramref name="node"/> is successfully found and removed; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/></exception>
        public bool Remove(JDexNode node) {
            if(node == null) throw new ArgumentNullException(nameof(node));
            if(_children.TryGetValue(node._key, out var list))
                return list.Remove(node);
            return false;
        }
        /// <summary>Removes the child nodes with the specified <paramref name="key"/> from this <see cref="JDexNode"/></summary>
        /// <param name="key">The key of the child nodes to remove</param>
        /// <returns><see langword="true"/> if the child nodes is successfully found and removed; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        public bool Remove(string key) {
            if(_children.TryGetValue(ValidKey(key), out var list)) {
                foreach(var child in list) {
                    child._key = null;
                    child._parent = null;
                }
                list._key = null;
                list._parent = null;
            }
            return _children.Remove(key);
        }
        /// <summary>Removes the child node at the specified <paramref name="key"/> and <paramref name="index"/> of this <see cref="JDexNode"/></summary>
        /// <param name="key">The non-null key of the node to remove</param>
        /// <param name="index">The zero-based index of the node to remove</param>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="JDexNodeList.Count"/></exception>
        public void RemoveAt(string key, int index) => _children[ValidKey(key)].RemoveAt(index);
        /// <summary>Removes the first occurrence of the specified <paramref name="value"/> from this <see cref="JDexNode"/></summary>
        /// <param name="value">The value to remove from this <see cref="JDexNode"/></param>
        /// <returns><see langword="true"/> if the <paramref name="value"/> is successfully found and removed; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        public bool RemoveValue(JDexValue value) {
            if(value == null) throw new ArgumentNullException(nameof(value));
            return _values.Remove(value);
        }
        /// <summary>Removes the value at the specified <paramref name="index"/> from this <see cref="JDexNode"/></summary>
        /// <param name="index">The zero-based index of the value to remove</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to <see cref="ValueCount"/></exception>
        public void RemoveValueAt(int index) {
            if(index < 0 || index >= _values.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            _values.RemoveAt(index);
        }
        /// <summary>Removes a range of values from this <see cref="JDexNode"/></summary>
        /// <param name="index">The zero-based starting index of the values to remove</param>
        /// <param name="count">The number of values to remove</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="index"/> + <paramref name="count"/> is less than 0 or greater then or equal to <see cref="ValueCount"/> or <paramref name="count"/> is less then 0</exception>
        public void RemoveValueRange(int index, int count) {
            if(index < 0 || index >= _values.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if(index + count < 0 || index + count > _values.Count)
                throw new ArgumentOutOfRangeException(nameof(count));
            _values.RemoveRange(index, count);
        }
        /// <summary>Gets the <see cref="JDexNode"/> associated with the specified <paramref name="key"/> and <paramref name="index"/></summary>
        /// <param name="key">The key of the <see cref="JDexNode"/> to get</param>
        /// <param name="index">The index of the <see cref="JDexNode"/> to get</param>
        /// <param name="node">When the <paramref name="key"/> and <paramref name="index"/> are not found, <paramref name="node"/> will be <see langword="null"/></param>
        /// <returns><see langword="true"/> if this <see cref="JDexNode"/> has the specified <paramref name="key"/> and <paramref name="index"/>; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        public bool TryGetNode(string key, int index, [MaybeNullWhen(false)] out JDexNode node) {
            if(_children.TryGetValue(ValidKey(key), out var list) && index >= 0 && index < list.Count) {
                node = list[index];
                return true;
            }
            node = null;
            return false;
        }

        /// <summary>Converts this <see cref="JDexNode"/> into an instance of <see cref="string"/>. 
        /// Will not append the root (this) <see cref="JDexNode"/> node values to JDex formated string</summary>
        /// <returns>A string whose value is the JDex formated string form of this instance</returns>
        public override string ToString( ) {
            StringBuilder builder = new StringBuilder( );
            foreach(var childList in _children) {
                foreach(var item in childList.Value) {
                    BuildString(ref builder, item, 0);
                }
            }
            if(builder.Length > 0)
                builder.Remove(builder.Length - 1, 1);
            return builder.ToString( );
        }
        public override bool Equals(object obj) => ReferenceEquals(this, obj);
        public override int GetHashCode( ) => base.GetHashCode( );

        ReadOnlyJDexNode.ReadOnlyJDexNodeList IReadOnlyJDexNode<ReadOnlyJDexNode>.this[string key] => this[key].AsReadOnly( );
        ReadOnlyJDexNode IReadOnlyJDexNode<ReadOnlyJDexNode>.this[string key, int i] => this[key, i].AsReadOnly( );
        ReadOnlyJDexNode IReadOnlyJDexNode<ReadOnlyJDexNode>.Parent => _parent.AsReadOnly( );
        /// <summary>Determins if the specified path exists in the specified <see cref="JDexNode"/>. Constructing the path 
        /// through multiple params or using ':' in a string will denote for the next node key. '#' at the end of the key string
        /// with a zero-based index number for index of the node, without this by default specified as 0 index</summary>
        /// <param name="root">The node to traverse through with the specified <paramref name="path"/></param>
        /// <param name="path">The path to traverse with. Each new param is equivalent path for the next node</param>
        /// <returns><see langword="true"/> if the specified <paramref name="path"/> exists in this <see cref="JDexNode"/>; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="path"/> is invalid key path format</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="root"/> is <see langword="null"/></exception>
        public static bool HasPath(JDexNode root, params string[ ] path) {
            if(root == null) throw new ArgumentNullException(nameof(root));
            if(path == null) throw new ArgumentNullException(nameof(path));
            foreach(var part in path) ValidKey(part, true);

            var node = root;
            foreach(var part in path) {
                foreach(var segment in part.Split(':')) {
                    var index = 0;
                    var key = segment;
                    var p = segment.IndexOf('#');
                    if(p != -1) {
                        index = int.Parse(segment[(p + 1)..]);
                        key = key[..p];
                    }

                    if(!node.ContainsKey(key)) return false;
                    var group = node[key];
                    if(index >= group.Count) return false;

                    node = group[index];
                }
            }

            return true;
        }
        /// <summary>Returns the node from the specified path of the specified <see cref="JDexNode"/>. Constructing the path 
        /// through multiple params or using ':' in a string will denote for the next node key. '#' at the end of the key string
        /// with a zero-based index number for index of the node, without this by default specified as 0 index</summary>
        /// <param name="root">The node to traverse through with the specified <paramref name="path"/></param>
        /// <param name="path">The path to traverse with. Each new param is equivalent path for the next node</param>
        /// <returns>The <see cref="JDexNode"/> from the specified node through the specified <paramref name="path"/> in this <see cref="JDexNode"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="path"/> is invalid key path format</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="root"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">index is less than 0 or greater then or equal to <see cref="JDexNodeList.Count"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        public static JDexNode PathThrough(JDexNode root, params string[ ] path) {
            if(root == null) throw new ArgumentNullException(nameof(root));
            if(path == null) throw new ArgumentNullException(nameof(path));
            foreach(var part in path) {
                if(!ValidKeyName(part, true))
                    throw new ArgumentInvalidException("Invalid key path", nameof(path));
            }

            var node = root;
            foreach(var part in path) {
                foreach(var segment in part.Split(':')) {
                    var index = 0;
                    var key = segment;
                    var p = segment.IndexOf('#');
                    if(p != -1) {
                        index = int.Parse(segment[(p + 1)..]);
                        key = key[..p];
                    }
                    node = node[key, index];
                }
            }

            return node;
        }
        /// <summary>Parse the specified <paramref name="jdexString"/> into a <see cref="JDexNode"/></summary>
        /// <param name="jdexString">The JDex formated string to construct the node from</param>
        /// <returns>The constructed <see cref="JDexNode"/> from the parsed string</returns>
        /// <exception cref="JDexParseException">If the string is a improperly formated JDex string</exception>
        public static JDexNode Parse(string jdexString) {
            JDexNode root = new JDexNode( );

            var lines = jdexString.Split('\n');
            string line;
            int begin = 0;
            int offset;
            var onKey = false;
            int i = 0;

            bool consumeWhitespace( ) {
                while(begin + offset < line.Length) {
                    if(!char.IsWhiteSpace(line[begin + offset])) break;
                    offset++;
                }
                return begin + offset >= line.Length;
            }
            void shiftBegin(int shift = 0) {
                begin += offset + shift;
                offset = 0;
            }

            List<(string name, int index)> path = new List<(string name, int index)>( );
            try {
                for(i = 0; i < lines.Length; i++) {
                    var shortHand = false;

                    line = lines[i];
                    begin = 0;
                    offset = 0;
                    if(line.Length == 0 || consumeWhitespace( )) continue;
                    if(line[begin + offset] == '#') continue;

                    var indent = 0;
                    foreach(var c in line) {
                        if(c != '\t') break;
                        ++indent;
                    }
                    if(indent == line.Length) continue;
                    if(indent > path.Count)
                        throw new JDexParseException($"Invalid indentation at line {i + 1}, needs {path.Count} indents");
                    if(indent < path.Count)
                        path.RemoveRange(indent, path.Count - indent);
                    begin = indent;
                    offset = 0;

                    if(char.IsWhiteSpace(line[begin]))
                        throw new JDexParseException($"Non tab whitespace at the begining, at line {i + 1} col {begin + 1 + indent * 3}");

                    while(begin + offset < line.Length) {
                        var c = line[begin + offset];

                        onKey = true;
                        if(char.IsLetterOrDigit(c) || c == '_' || offset != 0 && c == '.') {
                            ++offset;
                            continue;
                        }

                        if(offset == 0 && c == '.')
                            throw new JDexParseException($"Keys can not start with '.' at line {i + 1} col {begin + offset + 1 + indent * 3}");
                        if(offset == 0 && c == ':')
                            throw new JDexParseException($"Empty key name given at line {i + 1} col {begin + offset + 1 + indent * 3}");
                        if(char.IsWhiteSpace(c))
                            throw new JDexParseException($"Invalid character at line {i + 1} col {begin + offset + 1 + indent * 3}, expected :");
                        else if(c != ':')
                            throw new JDexParseException($"Invalid key character at line {i + 1} col {begin + offset + 1 + indent * 3}, valid characters: a-Z, 0-9, _ or .");
                        if(line[begin + offset - 1] == '.')
                            throw new JDexParseException($"Keys can not end with '.' at line {i + 1} col {begin + offset + indent * 3}");
                        onKey = false;

                        var key = line.Substring(begin, offset);
                        ++offset;

                        var node = new JDexNode( );
                        if(!consumeWhitespace( )) {
                            shiftBegin( );

                            bool comma = false;
                            while(begin + offset < line.Length) {
                                var ch = line[begin + offset];

                                var hasValue = false;
                                if(ch == '"') {
                                    shiftBegin(1);

                                    bool skip = false;
                                    while(begin + offset < line.Length) {
                                        if(!skip && line[begin + offset] == '"')
                                            break;
                                        skip = line[begin + offset] == '\\';
                                        ++offset;
                                    }
                                    if(begin + offset >= line.Length)
                                        throw new JDexParseException($"Unexpected end of line for value entry at line {i + 1} col {begin + indent * 3}");

                                    node.AddValue(FromLiteral(line.Substring(begin, offset)));
                                    shiftBegin(1);
                                    if(consumeWhitespace( ))
                                        break;
                                    shiftBegin( );
                                    hasValue = true;
                                } else if(char.IsDigit(ch) || ch == '.' || ch == '+' || ch == '-') {
                                    shiftBegin( );

                                    var isFloat = false;
                                    var hasExponent = false;
                                    var isHex = false;
                                    var isBinary = false;
                                    var zeroFirst = false;

                                    while(begin + offset < line.Length) {
                                        ch = char.ToLower(line[begin + offset]);
                                        if(ch == ':') break;

                                        if(ch == ',' || ch == '#') {
                                            if(offset == 1 && isFloat || offset == 2 && (isHex || isBinary))
                                                if(ch == ',') throw new JDexParseException($"Unexpected end of entry at line {i + 1} col {begin + offset + indent * 3}");
                                                else if(ch == '#') throw new JDexParseException($"Unexpected end of line at {i + 1} col {begin + offset + indent * 3}");
                                            break;
                                        }

                                        if(isBinary && offset >= 34)
                                            throw new JDexParseException($"Unexpected binary digit length (max 32) at line {i + 1} col {begin + offset + indent * 3}");
                                        else if(isHex && offset >= 10)
                                            throw new JDexParseException($"Unexpected hex digit length (max 8) at line {i + 1} col {begin + offset + indent * 3}");

                                        if(offset == 0 && ch == '0') zeroFirst = true;
                                        else if(ch == '+' || ch == '-') {
                                            if(!(offset == 0 || hasExponent && char.ToLower(line[begin + offset - 1]) == 'e'))
                                                throw new JDexParseException($"Unexpected symbol '{ch}' at {i + 1} col {begin + offset + indent * 3}");
                                        } else if(ch == 'e') {
                                            if(offset == 0 || hasExponent)
                                                throw new JDexParseException($"Unexpected symbol '{ch}' at {i + 1} col {begin + offset + indent * 3}");
                                            if(offset > 0 && line[begin + offset - 1] == '.')
                                                throw new JDexParseException($"Unexpected '.' at {i + 1} col {begin + offset + indent * 3 - 1}");
                                            isFloat = true;
                                            hasExponent = true;
                                        } else if(ch == '.') {
                                            if(isFloat || isHex || isBinary)
                                                throw new JDexParseException($"Unexpected '.' at line {i + 1} col {begin + offset + indent * 3}");
                                            isFloat = true;
                                        } else if(offset == 1 && zeroFirst && (ch == 'x')) isHex = true;
                                        else if(offset == 1 && zeroFirst && (ch == 'b')) isBinary = true;
                                        else if(isBinary && ch != '0' && ch != '1' || !(char.IsDigit(ch) || isHex && ch >= 'a' && ch <= 'f'))
                                            throw new JDexParseException($"Unexpected symbol '{ch}' at {i + 1} col {begin + offset + indent * 3}");
                                        ++offset;
                                    }

                                    if(isFloat) {
                                        var str = line.Substring(begin, offset);
                                        if(str[^1] == '.')
                                            throw new JDexParseException($"Unexpected '.' at {i + 1} col {begin + offset + indent * 3 - 1}");
                                        if(str[^1] == '+' || str[^1] == '-')
                                            throw new JDexParseException($"Expected numeric value at {i + 1} col {begin + offset + indent * 3}");
                                        node.AddValue(float.Parse(str));
                                    } else {
                                        if((isHex || isBinary) && offset == 2)
                                            throw new JDexParseException($"Expected numeric value at {i + 1} col {begin + offset + indent * 3}");

                                        var s = 0;
                                        var t = 10;
                                        if(isHex) {
                                            s = 2;
                                            t = 16;
                                        } else if(isBinary) {
                                            s = 2;
                                            t = 2;
                                        }
                                        
                                        node.AddValue(Convert.ToInt32(line.Substring(begin + s, offset - s), t));
                                    }
                                    if(consumeWhitespace( ))
                                        break;
                                    shiftBegin( );
                                    hasValue = true;
                                } else if(ch == 't' || ch == 'f') {
                                    shiftBegin( );

                                    const string trueString = "true";
                                    const string falseString = "false";

                                    bool valid = true;
                                    if(ch == 't')
                                        for(; offset < trueString.Length && valid; offset++)
                                            valid = trueString[offset] == line[begin + offset];
                                    else if(ch == 'f')
                                        for(; offset < falseString.Length && valid; offset++)
                                            valid = falseString[offset] == line[begin + offset];

                                    if(begin + offset + 1 < line.Length && line[begin + offset] == ':') break;

                                    if(valid) {
                                        node.AddValue(ch == 't');
                                        if(consumeWhitespace( ))
                                            break;
                                        shiftBegin( );
                                        hasValue = true;
                                    } else throw new JDexParseException($"Unexpected symbol at line {i + 1} col {begin + indent * 3}");
                                } else {
                                    if(consumeWhitespace( ))
                                        break;
                                    shiftBegin( );
                                }

                                if(begin + offset >= line.Length) break;
                                c = line[begin + offset];
                                if(char.IsLetterOrDigit(c) || c == '.' || c == '_' || c == ':') {
                                    if(comma) throw new JDexParseException($"Unexpected symbol at line {i + 1} col {begin + indent * 3}");

                                    offset = 0;
                                    break;
                                }

                                comma = false;
                                shiftBegin( );

                                if(line[begin] == '#') break;
                                if(line[begin] == ',') {
                                    if(!hasValue)
                                        throw new JDexParseException($"Unexpected ',' at line {i + 1} col {begin + indent * 3}");

                                    ++offset;
                                    if(consumeWhitespace( ))
                                        throw new JDexParseException($"Unexpected end of line at line {i + 1}");

                                    comma = true;
                                    continue;
                                }

                                throw new JDexParseException($"Unexpected symbol at line {i + 1} col {begin + indent * 3}");
                            }
                        }

                        var addToNode = root;
                        foreach(var (name, index) in path)
                            addToNode = addToNode[name, index];

                        int nodeIndex = addToNode.ContainsKey(key) ? addToNode[key].Count : 0;
                        addToNode.Add(key, node);
                        if(!shortHand) path.Add((key, nodeIndex));
                        shortHand = true;

                        if(begin + offset < line.Length && line[begin + offset] == '#') break;
                    }
                    if(onKey) throw new JDexParseException($"Unexpected end of file at line {i + 1} col {begin + offset + 1 + indent * 3}, expected :");
                }
            } catch(FormatException e) { throw new JDexParseException($"{e.Message} at line {i + 1} col {begin + 1}"); } 
            catch(OverflowException e) { throw new JDexParseException($"{e.Message} at line {i + 1} col {begin + 1}"); }

            return root;
        }
        private static void BuildString(ref StringBuilder builder, JDexNode node, int indent) {
            builder.Append('\t', indent);
            builder.Append(node._key).Append(':');

            if(node._values.Count > 0)
                builder.Append(" ").AppendJoin(", ", node._values.Select((JDexValue value) => {
                    switch(value.Type) {
                    case JDexValueType.String:
                    return new StringBuilder(value.ToString( ).Length + 2)
                    .Append("\"").Append(ToLiteral(value.ToString( ))).Append("\"").ToString( );
                    case JDexValueType.Bool:
                    case JDexValueType.Int:
                    case JDexValueType.Float:
                    return value.ToString( );
                    }
                    throw new SystemException("Unreachable");
                }));

            builder.Append('\n');

            foreach(var childList in node._children) {
                foreach(var item in childList.Value) {
                    BuildString(ref builder, item, indent + 1);
                }
            }
        }
        /// <summary>Validates the specified <paramref name="name"/> conforms to JDex key name or 
        /// path format. JDex key name format valid character set is 'a-Z', '0-9', '_' or if not 
        /// on key bounds '.'. JDex key path conforms to JDex key names for entry names with ':' 
        /// denoting for sub key entries and '#' followed by positive number for index of sub 
        /// entry at the end of the key name. Both ':' and '#' are not valid on string bounds</summary>
        /// <param name="name">The name to validate is conformant to JDex key name or path format</param>
        /// <param name="path">Codition for testing name is conformant to JDex key name or path format</param>
        /// <returns><see langword="true"/> if the specified <paramref name="name"/> is a conforms to JDex key name or path format based on <paramref name="path"/> condition; otherwise, <see langword="false"/></returns>
        public static bool ValidKeyName(string name, bool path = false) {
            bool first = true;
            bool InvalidChar(char c) {
                var b = !first && c == '.' || c == '_' || char.IsLetterOrDigit(c) ||
                    path && !first && (c == ':' || c == '#');
                first = false;
                return !b;
            }
            return !(name == null || name.Length == 0 || name.Any(InvalidChar) || name[^1] == '.' ||
                path && (name[^1] == ':' || name[^1] == '#' || name.Count((char c) => c == '#') > name.Count((char c) => c == ':') + 1));
        }
        // https://stackoverflow.com/a/14087738
        private static string ToLiteral(string input) {
            StringBuilder literal = new StringBuilder(input.Length);
            foreach(var c in input) {
                switch(c) {
                case '\"': literal.Append("\\\""); break;
                case '\\': literal.Append(@"\\"); break;
                case '\0': literal.Append(@"\0"); break;
                case '\a': literal.Append(@"\a"); break;
                case '\b': literal.Append(@"\b"); break;
                case '\f': literal.Append(@"\f"); break;
                case '\n': literal.Append(@"\n"); break;
                case '\r': literal.Append(@"\r"); break;
                case '\t': literal.Append(@"\t"); break;
                case '\v': literal.Append(@"\v"); break;
                default:
                // ASCII printable character
                if(c >= 0x20 && c <= 0x7e) {
                    literal.Append(c);
                    // As UTF16 escaped character
                } else {
                    literal.Append(@"\u");
                    literal.Append(((int) c).ToString("x4"));
                }
                break;
                }
            }
            return literal.ToString( );
        }
        private static string FromLiteral(string input) {
            bool IsHex(char c) => char.IsNumber(c) || c >= 'a' && c <= 'a' || c >= 'A' && c <= 'F';

            StringBuilder str = new StringBuilder(input.Length);
            for(var i = 0; i < input.Length; i++) {
                var c = input[i];
                if(c == '\\') {
                    c = input[++i];
                    if(i >= input.Length)
                        throw new FormatException("Invalid string escape");

                    switch(c) {
                    case '\'': str.Append(c); break;
                    case '"': str.Append(c); break;
                    case '\\': str.Append(c); break;
                    case '0': str.Append('\0'); break;
                    case 'a': str.Append('\a'); break;
                    case 'b': str.Append('\b'); break;
                    case 'f': str.Append('\f'); break;
                    case 'n': str.Append('\n'); break;
                    case 'r': str.Append('\r'); break;
                    case 't': str.Append('\t'); break;
                    case 'v': str.Append('\v'); break;

                    case 'u': {
                        var begin = ++i;
                        while(i < input.Length && i - begin < 4)
                            if(!IsHex(input[i++]))
                                throw new FormatException("Invalid string escape");
                        if(i - begin != 4)
                            throw new FormatException("Invalid string escape");
                        str.Append(char.ConvertFromUtf32(int.Parse(input.Substring(begin, 4), System.Globalization.NumberStyles.HexNumber)));
                    }
                    break;
                    case 'U': {
                        var begin = ++i;
                        while(i < input.Length && i - begin < 8)
                            if(!IsHex(input[i++]))
                                throw new FormatException("Invalid string escape");
                        if(i - begin != 8)
                            throw new FormatException("Invalid string escape");
                        var code = int.Parse(input.Substring(begin, 8), System.Globalization.NumberStyles.HexNumber);
                        if(code < 0 || code > 0x0010FFFF)
                            throw new FormatException("Invalid string escape");
                        str.Append(char.ConvertFromUtf32(int.Parse(input.Substring(begin, 4), System.Globalization.NumberStyles.HexNumber)));
                    }
                    break;
                    case 'x': {
                        var begin = ++i;
                        while(i < input[i++] && i - begin < 4)
                            if(!IsHex(input[i++])) break;
                        str.Append(char.ConvertFromUtf32(int.Parse(input.Substring(begin, i - begin), System.Globalization.NumberStyles.HexNumber)));
                    }
                    break;

                    default:
                    throw new FormatException("Invalid string escape");
                    }

                    continue;
                }

                str.Append(c);
            }
            return str.ToString( );
        }

        public static bool operator ==(JDexNode left, JDexNode right) => ReferenceEquals(left, right);
        public static bool operator !=(JDexNode left, JDexNode right) => !(left == right);

        /// <summary>List of nodes inside of <see cref="JDexNode"/></summary>
        public sealed class JDexNodeList : IList<JDexNode>, IReadOnlyList<ReadOnlyJDexNode> {
            internal string _key;
            internal JDexNode _parent;
            internal List<JDexNode> _items = new List<JDexNode>( );

            /// <summary>Gets or sets the node at the specified <paramref name="index"/></summary>
            /// <param name="index">The zero-based index of the node to get or set</param>
            /// <returns>The node at the specified <paramref name="index"/></returns>
            /// <exception cref="ArgumentNullException"><paramref name="value"/> is null</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="Count"/></exception>
            /// <exception cref="ArgumentCircularBranchException"><paramref name="value"/> is a parent node of this <see cref="JDexNode"/> or earlier</exception>
            public JDexNode this[int index] {
                get => _items[index];
                set {
                    if(index < 0 || index >= _items.Count)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    if(value == null) throw new ArgumentNullException(nameof(value));
                    var parent = value;
                    while(parent != null) {
                        if(parent == value)
                            throw new ArgumentCircularBranchException(nameof(value));
                        parent = parent._parent;
                    }
                    value._parent?.Remove(value);
                    value._parent = _parent;
                    value._key = _key;

                    _items[index] = value;
                }
            }

            /// <summary>Gets the number of <see cref="JDexNode"/> in this <see cref="JDexNodeList"/></summary>
            public int Count => _items.Count;
            /// <summary></summary>
            public JDexNode Parent => _parent;

            internal JDexNodeList(string key, JDexNode node) {
                _key = key ?? throw new ArgumentNullException(nameof(key));
                _parent = node ?? throw new ArgumentNullException(nameof(node));
                _items = new List<JDexNode>( );
            }

            /// <summary>Adds the specified <paramref name="node"/> into the children of this <see cref="JDexNode"/></summary>
            /// <param name="node">The <see cref="JDexNode"/> to add</param>
            /// <exception cref="ArgumentCircularBranchException"><paramref name="node"/> is a parent node of this <see cref="JDexNode"/> or earlier</exception>
            /// <exception cref="ArgumentNullException"><paramref name="node"/> is null</exception>
            public void Add(JDexNode node) {
                if(node == null) throw new ArgumentNullException(nameof(node));
                var parent = _parent;
                while(parent != null) {
                    if(parent == node)
                        throw new ArgumentCircularBranchException(nameof(node));
                    parent = parent._parent;
                }

                node._parent?.Remove(node);
                node._parent = _parent;
                node._key = _key;

                _items.Add(node);
            }
            /// <summary>Returns a read-only wrapper around this <see cref="JDexNodeList"/></summary>
            /// <returns>An object that acts as a read-only wrapper around this <see cref="JDexNodeList"/></returns>
            public ReadOnlyJDexNode.ReadOnlyJDexNodeList AsReadOnly( ) => new ReadOnlyJDexNode.ReadOnlyJDexNodeList(this);
            /// <summary>Removes all child nodes from this <see cref="IJDexNode{T}"/></summary>
            public void Clear( ) {
                _parent._children.Remove(_key);
                _parent = null;

                foreach(var node in _items) {
                    node._key = null;
                    node._parent = null;
                }
                _items.Clear( );
            }
            /// <summary>Determins if <see cref="JDexNodeList"/> has the specified node</summary>
            /// <param name="node">The node to locate in <see cref="JDexNodeList"/></param>
            /// <returns><see langword="true"/> if the specified <paramref name="node"/> is found in this <see cref="JDexNodeList"/></returns>
            /// <exception cref="ArgumentNullException">node is null</exception>
            public bool Contains(JDexNode node) {
                if(node == null) throw new ArgumentNullException(nameof(node));
                return _items.Contains(node);
            }
            /// <summary>Returns an enumerator that iterates through </summary>
            /// <returns></returns>
            public IEnumerator<JDexNode> GetEnumerator( ) => _items.GetEnumerator( );
            /// <summary>Searches for the specified <paramref name="node"/> and returns the zero-based index
            /// of the first occurrence within the range of nodes in this <see cref="JDexNodeList"/> that starts
            /// at the specified <paramref name="index"/> and contains the specified <paramref name="count"/> of nodes</summary>
            /// <param name="node">The node to locate within <see cref="JDexNodeList"/></param>
            /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list</param>
            /// <param name="count">The number of nodes in the section to search</param>
            /// <returns>The zero-based index of the first occurrence within the range of nodes in this <see cref="JDexNodeList"/>
            /// thats starts at <paramref name="index"/> and contains <paramref name="count"/> of nodes, if found; otherwise, -1</returns>
            /// <exception cref="ArgumentNullException"><paramref name="node"/> is null</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="index"/> + <paramref name="count"/> 
            /// is less than 0 or greater than or equal to <see cref="Count"/></exception>
            public int IndexOf(JDexNode node, int index, int count) {
                if(node == null) throw new ArgumentNullException(nameof(node));
                return _items.IndexOf(node, index, count);
            }
            /// <summary>Searches for the specified <paramref name="node"/> and returns the zero-based index
            /// of the first occurrence within the range of nodes in this <see cref="JDexNodeList"/> that starts
            /// at the specified <paramref name="index"/> to the last node</summary>
            /// <param name="node">The node to locate within <see cref="JDexNodeList"/></param>
            /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list</param>
            /// <returns>The zero-based index of the first occurrence within the range of nodes in this <see cref="JDexNodeList"/>
            /// thats starts at <paramref name="index"/> to the last node</returns>
            /// <exception cref="ArgumentNullException"><paramref name="node"/> is null</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/></exception>
            public int IndexOf(JDexNode node, int index) {
                if(node == null) throw new ArgumentNullException(nameof(node));
                return _items.IndexOf(node, index);
            }
            /// <summary>Searches for the specified <paramref name="node"/> and returns the zero-based index 
            /// of the first occurrence within the entire <see cref="JDexNodeList"/></summary>
            /// <param name="node">The node to locate within <see cref="JDexNodeList"/></param>
            /// <returns>The zero-based index of the first occurrence within the entire <see cref="JDexNodeList"/> if found; otherwise, -1</returns>
            /// <exception cref="ArgumentNullException"><paramref name="node"/> is null</exception>
            public int IndexOf(JDexNode node) {
                if(node == null) throw new ArgumentNullException(nameof(node));
                return _items.IndexOf(node);
            }
            /// <summary>Insers the node into this <see cref="JDexNodeList"/> at the specified <paramref name="index"/></summary>
            /// <param name="node">The node to insert into this <see cref="JDexNodeList"/></param>
            /// <param name="index">The zero-based index to insert into this <see cref="JDexNodeList"/></param>
            /// <exception cref="ArgumentNullException"><paramref name="node"/> is null</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/></exception>
            public void Insert(int index, JDexNode node) {
                if(node == null) throw new ArgumentNullException(nameof(node));
                if(index < 0 || index >= _items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                var parent = node;
                while(parent != null) {
                    if(parent == node)
                        throw new ArgumentCircularBranchException(nameof(node));
                    parent = parent._parent;
                }

                node._parent?.Remove(node);
                node._parent = _parent;
                node._key = _key;
                _items.Insert(index, node);
            }
            /// <summary>Remove the first occurrence of the specified <paramref name="node"/> from this <see cref="JDexNodeList"/></summary>
            /// <param name="node">The node to remove from this <see cref="JDexNodeList"/></param>
            /// <returns><see langword="true"/> if the node was successfully found and removed; otherwise, <see langword="false"/></returns>
            /// <exception cref="ArgumentNullException"><paramref name="node"/> is null</exception>
            public bool Remove(JDexNode node) {
                if(node == null) throw new ArgumentNullException(nameof(node));
                if(_items.Remove(node)) {
                    if(_items.Count == 0) {
                        _parent._children.Remove(_key);
                        _key = null;
                    }

                    node._parent = null;
                    node._key = null;
                    return true;
                }
                return false;
            }
            /// <summary>Removes the node at the specified <paramref name="index"/> of this <see cref="JDexNodeList"/></summary>
            /// <param name="index">The index of the node to remove</param>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to <see cref="Count"/></exception>
            public void RemoveAt(int index) {
                if(index < 0 || index >= _items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var node = _items[index];
                if(_items.Count == 0) {
                    node._parent?._children.Remove(_key);
                    _key = null;
                }
                node._parent = null;
                node._key = null;

                _items.RemoveAt(index);
            }

            ReadOnlyJDexNode IReadOnlyList<ReadOnlyJDexNode>.this[int index] => _items[index].AsReadOnly( );
            bool ICollection<JDexNode>.IsReadOnly => false;
            void ICollection<JDexNode>.CopyTo(JDexNode[ ] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
            IEnumerator IEnumerable.GetEnumerator( ) => GetEnumerator( );
            IEnumerator<ReadOnlyJDexNode> IEnumerable<ReadOnlyJDexNode>.GetEnumerator( ) {
                foreach(var item in _items)
                    yield return item.AsReadOnly( );
                yield break;
            }
        }

    }

}
