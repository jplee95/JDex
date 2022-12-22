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
    /// <summary>Represents a read-only JDex node of values and child nodes</summary>
    /// <typeparam name="T">The type of self inheriting node</typeparam>
    public interface IReadOnlyJDexNode<T> where T : IReadOnlyJDexNode<T> {
        /// <summary>Gets the value at the specified <paramref name="index"/></summary>
        /// <param name="index">The zero-based index of the value to get or set</param>
        /// <returns>The value at the specified <paramref name="index"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="ValueCount"/></exception>
        JDexValue this[int index] { get; }
        /// <summary>Gets the <see cref="ReadOnlyJDexNode.ReadOnlyJDexNodeList"/> at the specified <paramref name="key"/></summary>
        /// <param name="key">The non-null key of the list to get</param>
        /// <returns>The <see cref="JDexNode.JDexNodeList"/> at the specified <paramref name="key"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        ReadOnlyJDexNode.ReadOnlyJDexNodeList this[string key] { get; }
        /// <summary>Gets the <see cref="IReadOnlyJDexNode{T}"/> at the specified <paramref name="key"/> and <paramref name="index"/></summary>
        /// <param name="key">The non-null key of value to get or set</param>
        /// <param name="index">The zero-based index of the node to get or set</param>
        /// <returns>The node at the specified <paramref name="key"/> and <paramref name="index"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0 or greater then or equal to <see cref="ReadOnlyJDexNode.ReadOnlyJDexNodeList.Count"/></exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not exist in node</exception>
        T this[string key, int index] { get; }

        /// <summary>Gets the number of <see cref="IReadOnlyJDexNode{T}"/> in this <see cref="IReadOnlyJDexNode{T}"/></summary>
        int Count { get; }
        /// <summary>Gets the number of values in this <see cref="IReadOnlyJDexNode{T}"/></summary>
        int ValueCount { get; }
        /// <summary>Gets the key of this <see cref="IReadOnlyJDexNode{T}"/>. This will be <see langword="null"/> if <see cref="Parent"/> == <see langword="null"/></summary>
        string Key { get; }
        /// <summary>Gets the parent node of this <see cref="IReadOnlyJDexNode{T}"/>. This will be <see langword="null"/> if not a child of another <see cref="IReadOnlyJDexNode{T}"/></summary>
        T Parent { get; }

    }

}
