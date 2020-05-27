using System.Collections.Generic;

namespace DNI.Backup.Services.FileList {
    public interface IDirectoryGlobSettings {
        string SourceRootDir { get; set; }

        string IncludeGlob { get; set; }

        IEnumerable<string> ExcludeGlobs { get; set; }
    }
}