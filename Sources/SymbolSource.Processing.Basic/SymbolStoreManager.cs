using System;
using System.Globalization;
using System.IO;
using Microsoft.Cci.Pdb;

namespace SymbolSource.Processing.Basic
{
    public class SymbolStoreManager : ISymbolStoreManager
    {
        private readonly PdbRwInternal pdbRwInternal = new PdbRwInternal(); 
      
        public string ReadHash(string pdbFilePath)
        {
            return pdbRwInternal.ReadHash(pdbFilePath);
        }

        public string ReadHash(Stream stream)
        {
            return stream.Execute(s => ReadHash(s));
        }

        public void WriteHash(string pdbFilePath, string hash)
        {
            pdbRwInternal.WriteHash(pdbFilePath, hash);
        }

        internal class PdbRwInternal
        {
            //Obliczenie adresu na podstawie studiowania Microsoft.Cci.Pdb.PdbFile
            BitAccess bits = new BitAccess(0);

            public string ReadHash(string pdbFileHash)
            {
                using (var read = File.Open(pdbFileHash, FileMode.Open))
                {
                    PdbFileHeader head = new PdbFileHeader(read, bits);
                    PdbReader reader = new PdbReader(read, head.pageSize);
                    MsfDirectory dir = new MsfDirectory(reader, head, bits);

                    bits.MinCapacity(28);
                    reader.Seek(dir.streams[1].pages[0], 0);
                    reader.Read(bits.Buffer, 0, 28);

                    int ver;
                    int sig;
                    int age;
                    Guid guid;
                    bits.ReadInt32(out ver); //  0..3  Version
                    bits.ReadInt32(out sig); //  4..7  Signature
                    bits.ReadInt32(out age); //  8..11 Age
                    bits.ReadGuid(out guid); // 12..27 GUID
                    return (guid.ToString("N") + age.ToString("x")).ToUpper();
                }
            }

            public void WriteHash(string pdbFileHash, string hash)
            {
                Guid guid = new Guid(hash.Remove(32));
                int age = int.Parse(hash.Substring(32), NumberStyles.HexNumber);

                using (var read = File.Open(pdbFileHash, FileMode.Open))
                {
                    PdbFileHeader head = new PdbFileHeader(read, bits);
                    PdbReader reader = new PdbReader(read, head.pageSize);
                    MsfDirectory dir = new MsfDirectory(reader, head, bits);

                    reader.Seek(dir.streams[1].pages[0], 8); //bo przeskakujemy 8 znaków na ver i sig

                    using (BinaryWriter writer = new BinaryWriter(read))
                    {
                        writer.Write(age);
                        writer.Write(guid.ToByteArray());
                    }
                }
            }
        }
    }
}