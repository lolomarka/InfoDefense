using InfoDefense.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace InfoDefense.Utils
{
    internal class DefaultFileService : IFileService 
    {
        public async Task<string> OpenAsync(string fileName)
        {
            string text = string.Empty;
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                text = await streamReader.ReadToEndAsync();
            }
            return text;
        }
    }
}
