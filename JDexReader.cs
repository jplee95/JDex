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

#nullable enable

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JDex {
    public sealed class JDexReader : MarshalByRefObject, IDisposable {

        private StreamReader _reader;

        private JDexReader(StreamReader reader) { _reader = reader; }
        public JDexReader(Stream stream) => _reader = new StreamReader(stream);
        public JDexReader(string path) => _reader = new StreamReader(path);
        public JDexReader(Stream stream, bool detectEncodingFromByteOrderMarks) => _reader = new StreamReader(stream, detectEncodingFromByteOrderMarks);
        public JDexReader(Stream stream, Encoding encoding) => _reader = new StreamReader(stream, encoding);
        public JDexReader(string path, bool detectEncodingFromByteOrderMarks) => _reader = new StreamReader(path, detectEncodingFromByteOrderMarks);
        public JDexReader(string path, Encoding encoding) => _reader = new StreamReader(path, encoding);
        public JDexReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) => _reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks);
        public JDexReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) => _reader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks);
        public JDexReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) => _reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize);
        public JDexReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) => _reader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks, bufferSize);
        public JDexReader(Stream stream, Encoding? encoding = null, bool detectEncodingFromByteOrderMarks = true, int bufferSize = -1, bool leaveOpen = false) => _reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen);

        public Stream BaseStream { get => _reader.BaseStream; }
        public Encoding CurrentEncoding { get => _reader.CurrentEncoding; }
        public bool EndOfStream { get => _reader.EndOfStream; }

        public JDexNode Read( ) {
            bool found = false;
            string? line;
            StringBuilder compound = new StringBuilder( );
            while((line = _reader.ReadLine( )) != null) {
                if(compound.Length > 0) compound.Append("\n");
                compound.Append(line);
                if(line.Length > 0) {
                    var c = (char) _reader.Peek( );
                    if(found && !char.IsWhiteSpace(c) && c != '#') break;
                    if(!char.IsWhiteSpace(line[0]) && line[0] != '#') found = true;
                }
            }
            return JDexNode.Parse(compound.ToString( ));
        }
        public Task<JDexNode> ReadAsync( ) { // TODO: validate this code
            var task = new Task<JDexNode>(Read);
            task.Start( ); // Is this right?
            return task;
        }
        public JDexNode ReadToEnd( ) => JDexNode.Parse(_reader.ReadToEnd( ));
        public Task<JDexNode> ReadToEndAsync( ) {  // TODO: validate this code
            var task = new Task<JDexNode>(ReadToEnd);
            task.Start( ); // Is this right?
            return task;
        }

        public void Close( ) => _reader.Close( );
        public void DiscardBufferedData( ) => _reader.DiscardBufferedData( );
        public void Dispose( ) => _reader.Dispose( );

        public static JDexReader Synchronized(JDexReader reader) =>
            new JDexReader(TextReader.Synchronized(reader._reader) as StreamReader ?? throw new ArgumentNullException(nameof(reader)));

    }
}
