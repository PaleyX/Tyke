using System;
using System.IO;
using System.Diagnostics;

namespace Tyke.Net.ListKnife
{
    internal class Translators : IDisposable
    {
        internal Translators(string transPathname, string directory)
        {
            TransPathname = transPathname;
            Directory = directory;
        }

        private string TransPathname { get; }
        private string Directory { get; }

        internal void CreateTemp2Perm()
        {
            string pathname = Path.Combine(Directory, "TempPerm.dat");

            try
            {
                using FileStream istream = new(TransPathname, FileMode.Open, FileAccess.Read, FileShare.None),
                                 ostream = new(pathname, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                using (var reader = new BinaryReader(istream))
                {
                    using (var writer = new BinaryWriter(ostream))
                    {
                        int temp = 0;

                        writer.Write(temp); // blank ??? Why ???

                        // TODO: isn't this just copying the file? 
                        for (int i = 0; i < reader.BaseStream.Length / sizeof(Int32); ++i)
                        {
                            writer.Write(reader.ReadInt32());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error creating Perm to Temp file: " + e.Message);
            }
        }

        internal void CreatePerm2Temp()
        {
            string pathname = Path.Combine(Directory, "PermTemp.dat");

            try
            {
                using (FileStream istream = new(TransPathname, FileMode.Open, FileAccess.Read, FileShare.None),
                                  ostream = new(pathname, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (var reader = new BinaryReader(istream))
                    {
                        using (var writer = new BinaryWriter(ostream))
                        {
                            int max = 0;
                            int urn = 0;
                            int read = 0;

                            // translate
                            for (int i = 0; i < reader.BaseStream.Length / sizeof(Int32); ++i)
                            {
                                var temp = reader.ReadInt32();
                                ++read;

                                // A test
                                if (temp > max)
                                    max = temp;

                                long seek = writer.Seek(temp * sizeof(Int32), SeekOrigin.Begin);
                                Debug.Assert(seek == (temp * sizeof(Int32)));
                                writer.Write(++urn);
                            }

                            //Debug.Assert(max == MaxPermUrn);
                            Debug.Assert(read == reader.BaseStream.Length / sizeof(Int32));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error creating Perm to Temp file: " + e.Message);
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            // nowt
        }
        #endregion
    }
}
