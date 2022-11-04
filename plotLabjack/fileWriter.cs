using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plotLabjack
{
    internal class fileWriter
    {
        public string fileName;
        public string filePath;
        public fileWriter(string filePath, string fileName)
        {
            this.filePath = filePath;
            this.fileName = fileName;
        }

        public async Task SimpleWriteAsync(string text)
        {
            UnicodeEncoding uniencoding = new UnicodeEncoding();

            string filename = @"d:\hello.csv";
            byte[] result = uniencoding.GetBytes(text);

            using (FileStream SourceStream = File.Open(filename, FileMode.OpenOrCreate))
            {
                SourceStream.Seek(0, SeekOrigin.End);
                await SourceStream.WriteAsync(result, 0, (int)result.Length);
            }
        }

    }
}
