using System.Collections.Generic;

namespace DNI.Backup.Services.FileList {
    public class BackupDirectorySetting {
        public string RootDir { get; set; }

        public string IncludeGlob { get; set; }

        public IEnumerable<string> ExcludeGlobs { get; set; }

        public bool PreserveTree { get; set; }
    }
}