using System.Collections.Generic;

namespace DNI.Backup.Model {
    public interface IDirectoryGlob {
        string SourceRootDir { get; set; }

        string IncludeGlob { get; set; }

        IEnumerable<string> ExcludeGlobs { get; set; }
    }
}