using System;
using System.IO;
using System.Runtime.Remoting;

namespace SymbolSource.Processing.Basic
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream src, Stream dest)
        {
            int size = (src.CanSeek) ? Math.Min((int) (src.Length - src.Position), 0x2000) : 0x2000;
            var buffer = new byte[size];
            int n;
            do
            {
                n = src.Read(buffer, 0, buffer.Length);
                dest.Write(buffer, 0, n);
            } while (n != 0);
        }

        public static void CopyTo(this MemoryStream src, Stream dest)
        {
            dest.Write(src.GetBuffer(), (int) src.Position, (int) (src.Length - src.Position));
        }

        public static void CopyTo(this Stream src, MemoryStream dest)
        {
            if (src.CanSeek)
            {
                var pos = (int) dest.Position;
                int length = (int) (src.Length - src.Position) + pos;
                dest.SetLength(length);

                while (pos < length)
                    pos += src.Read(dest.GetBuffer(), pos, length - pos);
            }
            else
                src.CopyTo((Stream) dest);
        }

        public static T Execute<T>(this Stream stream, Func<string, T> action)
        {
            return Execute(stream, action, Path.GetRandomFileName());
        }

        public static T Execute<T>(this Stream stream, Func<string, T> action, string fileName)
        {
            var folderPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var tempPath = Path.Combine(folderPath, fileName);

            Directory.CreateDirectory(folderPath);
            stream.Seek(0, SeekOrigin.Begin);
            using (var fileStream = File.OpenWrite(tempPath))
                stream.CopyTo(fileStream);

            var result = action(tempPath);
            
            File.Delete(tempPath);
            Directory.Delete(folderPath);
            
            return result;
        }

    }

    public class StreamDecorator : Stream
    {
        private readonly Stream innerStream;

        public StreamDecorator(Stream innerStream)
        {
            this.innerStream = innerStream;
        }

        public override bool CanRead
        {
            get { return innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return innerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return innerStream.CanWrite; }
        }

        public override long Length
        {
            get { return innerStream.Length; }
        }

        public override long Position
        {
            get { return innerStream.Position; }
            set { innerStream.Position = value; }
        }

        public override bool CanTimeout
        {
            get { return innerStream.CanTimeout; }
        }

        public override int ReadTimeout
        {
            get { return innerStream.ReadTimeout; }
            set { innerStream.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return innerStream.WriteTimeout; }
            set { innerStream.WriteTimeout = value; }
        }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            innerStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return innerStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            innerStream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            innerStream.Close();
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            return innerStream.CreateObjRef(requestedType);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return innerStream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            innerStream.EndWrite(asyncResult);
        }

        public override bool Equals(object obj)
        {
            return innerStream.Equals(obj);
        }

        public override int GetHashCode()
        {
            return innerStream.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return innerStream.InitializeLifetimeService();
        }

        public override int ReadByte()
        {
            return innerStream.ReadByte();
        }

        public override string ToString()
        {
            return innerStream.ToString();
        }

        public override void WriteByte(byte value)
        {
            innerStream.WriteByte(value);
        }
    }    

    public class TempFileStream : StreamDecorator
    {
        private readonly string tempFile;

        public TempFileStream(Stream innerStream, string tempFile) : base(innerStream)
        {
            this.tempFile = tempFile;
        }

        protected override void Dispose(bool disposing)
        {            
            base.Dispose(disposing);
            File.Delete(tempFile);
        }
    }
}