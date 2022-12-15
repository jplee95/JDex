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

namespace JDex {
    /// <summary>Represents a JDex node of values and child JDex nodes</summary>
    /// <typeparam name="T">The type of self inheriting node</typeparam>
    public interface IJDexNode<T> where T : IJDexNode<T> {
        /// <summary>Gets or sets the value at the specified <paramref name="index"/></summary>
        /// <param name="index">The zero-based index of the value to get or set</param>
        /// <returns>The value at the specified <paramref name="index"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="ValueCount"/></exception>
        string this[int index] { get; set; }
        /// <summary>Gets the <see cref="JDexNode.JDexNodeList"/> at the specified <paramref name="key"/></summary>
        /// <param name="key">The non-null key of the list to get</param>
        /// <returns>The <see cref="JDexNode.JDexNodeList"/> at the specified <paramref name="key"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        JDexNode.JDexNodeList this[string key] { get; }
        /// <summary>Gets or sets the <see cref="IJDexNode{T}"/> at the specified <paramref name="key"/> and <paramref name="index"/></summary>
        /// <param name="key">The non-null key of value to get or set</param>
        /// <param name="index">The zero-based index of the node to get or set</param>
        /// <returns>The node at the specified <paramref name="key"/> and <paramref name="index"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="JDexNode.JDexNodeList.Count"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        T this[string key, int index] { get; set; }

        /// <summary>Gets the number of <see cref="IJDexNode{T}"/> in this <see cref="IJDexNode{T}"/></summary>
        int Count { get; }
        /// <summary>Gets the number of values in this <see cref="IJDexNode{T}"/></summary>
        int ValueCount { get; }
        /// <summary>Gets the key of this <see cref="IJDexNode{T}"/>. This will be <see langword="null"/> if <see cref="IJDexNode{T}.Parent"/> == <see langword="null"/></summary>
        string Key { get; }
        /// <summary>Gets the parent node of this <see cref="IJDexNode{T}"/>. This will be <see langword="null"/> if not a child of another <see cref="IJDexNode{T}"/></summary>
        T Parent { get; }

        /// <summary>Adds the specified <paramref name="node"/> into the children of this <see cref="IJDexNode{T}"/></summary>
        /// <param name="key">The non-null key of <paramref name="node"/> to add</param>
        /// <param name="node">The <see cref="IJDexNode{T}"/> to add</param>
        /// <exception cref="ArgumentCircularBranchException"><paramref name="node"/> is a parent node of this <see cref="IJDexNode{T}"/> or earlier</exception>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="node"/> is null</exception>
        void Add(string key, T node);
        /// <summary>Adds the specified <paramref name="value"/> to the <see cref="IJDexNode{T}"/></summary>
        /// <param name="value">The value to add to this <see cref="IJDexNode{T}"/></param>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        void AddValue(string value);
        /// <summary>Adds the specified <paramref name="collection"/> of values into this <see cref="IJDexNode{T}"/></summary>
        /// <param name="collection">The collection of values to insert into this <see cref="IJDexNode{T}"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/> or a value in <paramref name="collection"/> is <see langword="null"/></exception>
        void AddValueRange(IEnumerable<string> collection);
        /// <summary>Removes all child nodes from this <see cref="IJDexNode{T}"/>.
        /// This unlinks all child nodes and lists from this node</summary>
        void Clear( );
        /// <summary>Removes all value from this <see cref="IJDexNode{T}"/></summary>
        void ClearValues( );
        /// <summary>Determins if this <see cref="IJDexNode{T}"/> has the specified <paramref name="key"/></summary>
        /// <param name="key">The non-null key to locate in this <see cref="IJDexNode{T}"/></param>
        /// <returns><see langword="true"/> if the specified key is found in this <see cref="IJDexNode{T}"/></returns>
        /// <exception cref="ArgumentInvalidException"><paramref name="key"/> is invalid key</exception>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        bool ContainsKey(string key);
        /// <summary>Determins if <see cref="IJDexNode{T}"/> has the specified <paramref name="node"/></summary>
        /// <param name="node">The node to locate in this <see cref="IJDexNode{T}"/></param>
        /// <returns>true if the specified node is found in this <see cref="IJDexNode{T}"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/></exception>
        bool ContainsNode(T node);
        /// <summary>Inserts the <paramref name="value"/> into this <see cref="IJDexNode{T}"/> at the specified <paramref name="index"/></summary>
        /// <param name="index">The zero-based index to insert value at</param>
        /// <param name="value">The value to insert into this <see cref="IJDexNode{T}"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then <see cref="ValueCount"/></exception>
        void InsertValue(int index, string value);
        /// <summary>Inserts the <paramref name="collection"/> of values into this <see cref="IJDexNode{T}"/> at the specified <paramref name="index"/></summary>
        /// <param name="index">The zero-based index to insert the collection of values at</param>
        /// <param name="collection">The collection of values to insert into this <see cref="IJDexNode{T}"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/> or a value in <paramref name="collection"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then <see cref="ValueCount"/></exception>
        public void InsertValueRange(int index, IEnumerable<string> collection);
        /// <summary>Remove the child <paramref name="node"/> from this <see cref="IJDexNode{T}"/></summary>
        /// <param name="node">The child node of this <see cref="IJDexNode{T}"/> to remove</param>
        /// <returns><see langword="true"/> if the child <paramref name="node"/> is successfully found and removed; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <see langword="null"/></exception>
        bool Remove(T node);
        /// <summary>Removes the child nodes with the specified <paramref name="key"/> from this <see cref="IJDexNode{T}"/></summary>
        /// <param name="key">The key of the child nodes to remove</param>
        /// <returns><see langword="true"/> if the child nodes is successfully found and removed; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        bool Remove(string key);
        /// <summary>Removes the child node at the specified <paramref name="key"/> and <paramref name="index"/> of this <see cref="IJDexNode{T}"/></summary>
        /// <param name="key">The non-null key of the node to remove</param>
        /// <param name="index">The zero-based index of the node to remove</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="JDexNode.JDexNodeList.Count"/></exception>
        void RemoveAt(string key, int index);
        /// <summary>Removes the first occurrence of the specified <paramref name="value"/> from this <see cref="IJDexNode{T}"/></summary>
        /// <param name="value">The value to remove from this <see cref="IJDexNode{T}"/></param>
        /// <returns><see langword="true"/> if the <paramref name="value"/> is successfully found and removed; otherwise, <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
        bool RemoveValue(string value);
        /// <summary>Removes the value at the specified <paramref name="index"/> from this <see cref="IJDexNode{T}"/></summary>
        /// <param name="index">The zero-based index of the value to remove</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater than or equal to <see cref="ValueCount"/></exception>
        void RemoveValueAt(int index);
        /// <summary>Removes a range of values from this <see cref="IJDexNode{T}"/></summary>
        /// <param name="index">The zero-based starting index of the values to remove</param>
        /// <param name="count">The number of values to remove</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> or <paramref name="index"/> + <paramref name="count"/> is less than 0 or greater then or equal to <see cref="ValueCount"/> or <paramref name="count"/> is less then 0</exception>
        void RemoveValueRange(int index, int count);
    }

}
