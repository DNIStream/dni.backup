using System.Threading.Tasks;

using DNI.Backup.Services.FileList;

using FluentValidation;
using FluentValidation.TestHelper;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Test.Services.FileList {
    public class BackupDirectorySettingValidatorTests {
        private readonly ITestOutputHelper _output;

        public BackupDirectorySettingValidatorTests(ITestOutputHelper _output) {
            this._output = _output;
        }

        private IValidator<BackupDirectorySetting> GetValidator() {
            return new BackupDirectorySettingValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BackupDirectorySettingValidator_ValidatesRootDir_NullOrEmpty(string value) {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator
                .ShouldHaveValidationErrorFor(s => s.RootDir, value)
                .WithErrorMessage("RootDir is required");
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test/test/")]
        [InlineData("test/test")]
        [InlineData("../test/test")]
        [InlineData(@"test\test\")]
        [InlineData(@"test\test")]
        [InlineData(@"\test\test")]
        [InlineData(@"..\test\test")]
        public void BackupDirectorySettingValidator_ValidateRootDir_IsANonRelativeRootPath(string value) {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator
                .ShouldHaveValidationErrorFor(s => s.RootDir, value)
                .WithErrorMessage(@"RootDir must be a fully qualified root path (e.g. C:\ in Windows, /usr/ in linux)");
        }
    }
}