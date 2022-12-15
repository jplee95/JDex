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
    public sealed class JDexWriter : MarshalByRefObject, IAsyncDisposable, IDisposable {
        
        private StreamWriter _writer;

        private JDexWriter(StreamWriter writer) { _writer = writer; }
        public JDexWriter(Stream stream) {
            _writer = new StreamWriter(stream);
            _writer.NewLine = "\n";
        }
        public JDexWriter(string path) {
            _writer = new StreamWriter(path);
            _writer.NewLine = "\n";
        }
        public JDexWriter(Stream stream, Encoding encoding) {
            _writer = new StreamWriter(stream, encoding);
            _writer.NewLine = "\n";
        }
        public JDexWriter(string path, bool append) {
            _writer = new StreamWriter(path, append);
            _writer.NewLine = "\n";
        }
        public JDexWriter(Stream stream, Encoding encoding, int bufferSize) {
            _writer = new StreamWriter(stream, encoding, bufferSize);
            _writer.NewLine = "\n";
        }
        public JDexWriter(string path, bool append, Encoding encoding) {
            _writer = new StreamWriter(path, append, encoding);
            _writer.NewLine = "\n";
        }
        public JDexWriter(Stream stream, Encoding? encoding = null, int bufferSize = -1, bool leaveOpen = false) {
            _writer = new StreamWriter(stream, encoding, bufferSize, leaveOpen);
            _writer.NewLine = "\n";
        }
        public JDexWriter(string path, bool append, Encoding encoding, int bufferSize) {
            _writer = new StreamWriter(path, append, encoding, bufferSize);
            _writer.NewLine = "\n";
        }

        public bool AutoFlush { get => _writer.AutoFlush; set => _writer.AutoFlush = value; }
        public Stream BaseStream { get => _writer.BaseStream; }
        public Encoding Encoding { get => _writer.Encoding; }

        public void Write(JDexNode node) => _writer.WriteLine(node.ToString( ));
        public Task WriteAsync(JDexNode node) => _writer.WriteLineAsync(node.ToString( ));
        public void Write(ReadOnlyJDexNode node) => _writer.WriteLine(node.ToString( ));
        public Task WriteAsync(ReadOnlyJDexNode node) => _writer.WriteLineAsync(node.ToString( ));

        public void Flush( ) => _writer.Flush( );
        public Task FlushAsync( ) => _writer.FlushAsync( );

        public void Close( ) => _writer.Close( );
        public void Dispose( ) => _writer.Dispose( );
        public ValueTask DisposeAsync( ) => _writer.DisposeAsync( );

        public static JDexWriter Synchronized(JDexWriter writer) => 
            new JDexWriter(TextWriter.Synchronized(writer._writer) as StreamWriter ?? throw new ArgumentNullException(nameof(writer)));

    }
}
