using System.IO;

using FluentValidation;

namespace DNI.Backup.Model.Validators {
    public class DirectoryGlobValidator : AbstractValidator<DirectoryGlob> {
        public DirectoryGlobValidator() {
            RuleFor(x => x.SourceRootDir)
                .NotNull()
                .NotEmpty()
                .WithMessage("SourceRootDir is required")
                .Must(path => Path.IsPathRooted(path) && Path.IsPathFullyQualified(path))
                .WithMessage(@"SourceRootDir must be a fully qualified root path (e.g. C:\ in Windows, /usr/ in linux)")
                .Must(Directory.Exists)
                .WithMessage(path => $"Path '{path.SourceRootDir}' does not exist");

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