using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace Online_Archiver.Models
{
    public class ArchivedFilesModel
    {
        public string ArchiveName { get; set; }
        public List<ArchivedInputModel> Files { get; set; }
        public List<string> FileName { get; set; }
        public int CompresionLevel { get; set; }
    }
}
