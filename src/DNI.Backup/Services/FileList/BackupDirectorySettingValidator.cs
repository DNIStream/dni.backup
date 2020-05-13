using System.IO;

using FluentValidation;

namespace DNI.Backup.Services.FileList {
    public class BackupDirectorySettingValidator : AbstractValidator<BackupDirectorySetting> {
        public BackupDirectorySettingValidator() {
            RuleFor(x => x.RootDir)
                .NotNull()
                .NotEmpty()
                .WithMessage("RootDir is required")
                .MustAsync(async (path,t) => Path.IsPathRooted(path) && Path.IsPathFullyQualified(path))
                .WithMessage(@"RootDir must be a fully qualified root path (e.g. C:\ in Windows, /usr/ in linux)")
                .MustAsync(async (path, t) => Directory.Exists(path))
                .WithMessage(path => $"Path '{path.RootDir}' does not exist");

            RuleFor(x => x.IncludeGlob)
                .NotNull()
                .NotEmpty()
                .WithMessage("IncludeGlob is required");

            RuleForEach(x => x.ExcludeGlobs)
                .NotNull()
                .NotEmpty()
                .WithMessage("ExcludeGlobs entries must have a value");
        }
    }
}