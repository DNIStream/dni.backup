using System.Collections.Generic;

namespace DNI.Backup.Services.FileList {
    public class DirectoryGlobSettings: IDirectoryGlobSettings {
        public string SourceRootDir { get; set; }

        public string IncludeGlob { get; set; }

        public IEnumerable<string> ExcludeGlobs { get; set; }
    }
}