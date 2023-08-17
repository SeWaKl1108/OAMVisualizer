using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            try
            {
                UnicodeEncoding uniencoding = new UnicodeEncoding();

                string filename = Path.Combine(this.filePath, this.fileName);
                byte[] result = uniencoding.GetBytes(text);

                using (FileStream SourceStream = File.Open(filename, FileMode.OpenOrCreate))
                {
                    SourceStream.Seek(0, SeekOrigin.End);
                    await SourceStream.WriteAsync(result, 0, (int)result.Length);
                }
            }
            catch (IOException ex)
            {
                // Handle IO exceptions (e.g., file locked, path not found).
                MessageBox.Show($"IO Error: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle permission issues.
                MessageBox.Show($"Access Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other exceptions.
                MessageBox.Show($"Unexpected Error: {ex.Message}");
            }

        }

        public void updateFolder(string fileDialog)
        {

            this.filePath = Path.GetDirectoryName(fileDialog);
            this.fileName = Path.GetFileName(fileDialog);
        }

    }
}
