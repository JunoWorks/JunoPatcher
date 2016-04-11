using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Patcher
{
    public class VersionParser
    {
        string _filePath = string.Empty;

        public VersionParser(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(_filePath)) // If not exists, create
            {
                File.Create(_filePath).Dispose();
                writeVersion(1); // Write default version (1)
            }
        }

        public UInt32 readVersion()
        {
            BinaryReader reader = new BinaryReader(new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.None));
            reader.BaseStream.Position = 0x0;     // The offset you are reading the data from
            byte[] bytes = reader.ReadBytes(0x04); // Read 4 bytes into an array
            reader.Close();
            return BitConverter.ToUInt32(bytes, 0);
        }

        public bool writeVersion(UInt32 version)
        {
            BinaryWriter Writer = null;

            try {
                // Create a new stream to write to the file
                Writer = new BinaryWriter(File.OpenWrite(_filePath));

                // Writer raw data                
                Writer.Write(BitConverter.GetBytes(version));
                Writer.Flush();
                Writer.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
