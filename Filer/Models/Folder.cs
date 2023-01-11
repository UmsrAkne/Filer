using System.Collections.Generic;

namespace Filer.Models
{
    public class Folder
    {
        public List<ExtendFileInfo> Files { get; set; }

        public string Name { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
}