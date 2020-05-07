using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FluentValidation;

namespace DNI.Backup.Services.FileList
{
    public class BackupDirectorySettingValidator: AbstractValidator<BackupDirectorySetting> {
        public BackupDirectorySettingValidator() {
            RuleFor(x => x.RootDir)
                .NotNull()
                .NotEmpty()
                .WithMessage("RootDir is required")
                .Must(path => Path.IsPathRooted(path) && Path.IsPathFullyQualified(path))
                .WithMessage(@"RootDir must be a fully qualified root path (e.g. C:\ in Windows, /usr/ in linux)");
        }
    }
}
