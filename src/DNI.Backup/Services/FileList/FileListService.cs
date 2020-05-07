using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

namespace DNI.Backup.Services.FileList {
    public class FileListService : IFileListService {
        public FileListService() {
            
        }

        public async Task<IEnumerable<string>> GetFilesAsync(IEnumerable<BackupDirectorySetting> backupDirectorySettings) {
            if(backupDirectorySettings == null) {
                throw new ArgumentNullException(nameof(backupDirectorySettings));
            }

            if(!backupDirectorySettings.Any()) {
                throw new ArgumentException("backupDirectorySettings must contain at least one entry", nameof(backupDirectorySettings));
            }

            //var matches = new Matcher();
            //matches.AddInclude("**/*");
            //matches.add
            //matches.Execute(new DirectoryInfoWrapper(new DirectoryInfo());
            throw new NotImplementedException();
        }
    }
}