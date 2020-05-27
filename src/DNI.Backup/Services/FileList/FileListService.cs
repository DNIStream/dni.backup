using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace DNI.Backup.Services.FileList {
    public class FileListService : IFileListService {
        private readonly IValidator<DirectoryGlobSettings> _backupDirectorySettingValidator;

        public FileListService(IValidator<DirectoryGlobSettings> backupDirectorySettingValidator) {
            _backupDirectorySettingValidator = backupDirectorySettingValidator;
        }

        public async Task<IEnumerable<string>> GetFilesAsync(IEnumerable<IDirectoryGlobSettings> directoryGlobSettings) {
            if(directoryGlobSettings == null) {
                throw new ArgumentNullException(nameof(directoryGlobSettings));
            }

            var settings = directoryGlobSettings.ToArray();
            if(!settings.Any()) {
                throw new ArgumentException("directoryGlobSettings must contain at least one entry", nameof(directoryGlobSettings));
            }

            foreach(var setting in settings) {
                var result = await _backupDirectorySettingValidator.ValidateAsync(setting);
                if(!result.IsValid) {
                    var errorMessage = new StringBuilder();
                    errorMessage.AppendLine("The following validation errors occurred:");
                    foreach(var error in result.Errors) {
                        errorMessage.AppendLine($"{error.PropertyName} | {error.ErrorMessage}");
                    }

                    throw new DirectoryGlobSettingsValidationException(errorMessage.ToString());
                }
            }

            var matches = new Matcher();
            var s = settings[0];
            matches.AddInclude(s.IncludeGlob);
            if(s.ExcludeGlobs != null) {
                foreach(var ex in s.ExcludeGlobs) {
                    matches.AddExclude(ex);
                }
            }

            var paths = matches.Execute(new DirectoryInfoWrapper(new DirectoryInfo(s.SourceRootDir)));

            return paths
                .Files
                .Select(p => Path.GetFullPath(Path.Combine(s.SourceRootDir, p.Path)));
        }
    }
}