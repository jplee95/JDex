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

    public enum JDexValueType {
        String, Bool, Int, Float
    }

    public class JDexValue {

        private object _value;
        private JDexValueType _type;

        /// <summary>The type the value is</summary>
        public JDexValueType Type => _type;

        private JDexValue(object value, JDexValueType type) {
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _type = type;
        }

        public static explicit operator string(JDexValue value) {
            if(value._type != JDexValueType.String) throw new InvalidCastException( );
            return (string) value._value;
        }
        public static explicit operator bool(JDexValue value) {
            if(value._type != JDexValueType.Bool) throw new InvalidCastException( );
            return (bool) value._value;
        }
        public static explicit operator int(JDexValue value) {
            if(value._type != JDexValueType.Float && value._type != JDexValueType.Int) throw new InvalidCastException( );
            return (int) value._value;
        }
        public static explicit operator float(JDexValue value) {
            if(value._type != JDexValueType.Float && value._type != JDexValueType.Int) throw new InvalidCastException( );
            return (float) value._value;
        }

        public static implicit operator JDexValue(string value) => new JDexValue(value, JDexValueType.String);
        public static implicit operator JDexValue(bool value) => new JDexValue(value, JDexValueType.Bool);
        public static implicit operator JDexValue(int value) => new JDexValue(value, JDexValueType.Int);
        public static implicit operator JDexValue(float value) => new JDexValue(value, JDexValueType.Float);

        public override string ToString( ) => _value.ToString( );
        public override bool Equals(object obj) => obj is JDexValue value && EqualityComparer<object>.Default.Equals(_value, value._value) || obj == _value;
        public override int GetHashCode( ) => HashCode.Combine(_value);

    }
}
