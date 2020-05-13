using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace DNI.Backup.Services.FileList {
    public class FileListService : IFileListService {
        private readonly IValidator<BackupDirectorySetting> _backupDirectorySettingValidator;

        public FileListService(IValidator<BackupDirectorySetting> backupDirectorySettingValidator) {
            _backupDirectorySettingValidator = backupDirectorySettingValidator;
        }

        public async IAsyncEnumerable<string> GetFilesAsync(IEnumerable<BackupDirectorySetting> backupDirectorySettings) {
            if(backupDirectorySettings == null) {
                throw new ArgumentNullException(nameof(backupDirectorySettings));
            }

            var settings = backupDirectorySettings.ToArray();
            if(!settings.Any()) {
                throw new ArgumentException("backupDirectorySettings must contain at least one entry", nameof(backupDirectorySettings));
            }

            foreach(var setting in settings) {
                var result = await _backupDirectorySettingValidator.ValidateAsync(setting);
                if(!result.IsValid) {
                    var errorMessage = new StringBuilder();
                    errorMessage.AppendLine("The following validation errors occurred:");
                    foreach(var error in result.Errors) {
                        errorMessage.AppendLine($"{error.PropertyName} | {error.ErrorMessage}");    
                    }

                    throw new BackupDirectoryValidationException(errorMessage.ToString());
                }
            }

            var matches = new Matcher();
            var s = settings[0];
            matches.AddInclude(s.IncludeGlob);
            var paths = matches.Execute(new DirectoryInfoWrapper(new DirectoryInfo(s.RootDir)));
            foreach(var p in paths.Files) {
                yield return Path.GetFullPath(Path.Combine(s.RootDir, p.Path));
            }
        }
    }
}