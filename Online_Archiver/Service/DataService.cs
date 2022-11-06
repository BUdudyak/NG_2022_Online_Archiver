using Microsoft.AspNetCore.Connections;
using Online_Archiver.Models;
using System.Collections.Generic;
using System.IO;

namespace Online_Archiver.Service
{
    public class DataService : IDataService
    {
        public List<FileDetails> GetFiles()
        {
            var listFiles = new List<FileDetails>();
            DirectoryInfo d = new DirectoryInfo(@"UploadedFiles"); //Assuming Test is your Folder

            FileInfo[] Files = d.GetFiles(); //Getting Text files

            foreach (FileInfo file in Files)
            {
                listFiles.Add(
                    new FileDetails
                    {
                        FileName = file.FullName,
                        Path = "UploadedFiles\\" + file.Name,
                    });
            }
            return listFiles;
        }
    }
}
