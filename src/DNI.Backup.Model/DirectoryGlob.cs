using System.Collections.Generic;

namespace DNI.Backup.Model {
    public class DirectoryGlob : IDirectoryGlob {
        public string SourceRootDir { get; set; }

        public string IncludeGlob { get; set; }

        public IEnumerable<string> ExcludeGlobs { get; set; }
    }
}