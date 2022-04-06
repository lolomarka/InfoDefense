using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InfoDefense.Interfaces
{
    public interface IFileService
    {
        Task<string> OpenAsync(string fileName);
    }
}
