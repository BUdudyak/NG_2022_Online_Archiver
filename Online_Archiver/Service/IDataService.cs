using Online_Archiver.Models;
using System.Collections.Generic;
using System.IO;

namespace Online_Archiver.Service
{
    public interface IDataService
    {
        public List<FileDetails> GetFiles();
    }
}
