using System.Collections.Generic;
using System.Linq;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Backup.Services.FileList;
using DNI.Backup.TestHelpers;

using FluentValidation.TestHelper;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Services.Tests.FileList {
    [Trait(TestTraits.TEST_TYPE, TestTraits.INTEGRATION)]
    public class BackupDirectorySettingValidatorTests {
        private readonly ITestOutputHelper _output;

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public BackupDirectorySettingValidatorTests(ITestOutputHelper _output) {
            this._output = _output;
        }

        private DirectoryGlobSettingsValidator GetValidator() {
            return new DirectoryGlobSettingsValidator();
        }

        #region SourceRootDir

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void BackupDirectorySettingValidatorsRootDir_NullOrEmpty(string value) {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator
                .ShouldHaveValidationErrorFor(s => s.SourceRootDir, value)
                .WithErrorMessage("SourceRootDir is required");
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
        public void BackupDirectorySettingValidatorRootDir_FailsValidationIfInputIsANonRelativeRootPath(string value) {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator
                .ShouldHaveValidationErrorFor(s => s.SourceRootDir, value)
                .WithErrorMessage(@"SourceRootDir must be a fully qualified root path (e.g. C:\ in Windows, /usr/ in linux)");
        }

        [Theory]
        [InlineData(@"C:\")]
        [InlineData(@"C:\Users")]
        [InlineData(@"C:\Program Files\Common Files")]
        public void BackupDirectorySettingValidatorRootDir_PassesValidationIfInputIsAValidRootedPath(string value) {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator.ShouldNotHaveValidationErrorFor(s => s.SourceRootDir, value);
        }

        [Fact]
        public void BackupDirectorySettingValidatorRootDir_FailsValidationIfDirectoryDoesNotExist() {
            // Arrange
            var pathToTest = @"C:\ASDASDASDADS";
            var validator = GetValidator();

            // Act & Assert
            validator.ShouldHaveValidationErrorFor(s => s.SourceRootDir, pathToTest)
                .WithErrorMessage(@$"Path '{pathToTest}' does not exist");
        }

        #endregion

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void BackupDirectorySettingValidator_IncludeGlob_FailsValidationIfEmpty(string value) {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator.ShouldHaveValidationErrorFor(s => s.IncludeGlob, value)
                .WithErrorMessage("IncludeGlob is required");
        }

        [Fact]
        public void BackupDirectorySettingValidator_ExcludeGlobs_PassesValidationIfNull() {
            // Arrange
            var validator = GetValidator();

            // Act & Assert
            validator.ShouldNotHaveValidationErrorFor(s => s.ExcludeGlobs, (IEnumerable<string>)null);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void BackupDirectorySettingValidator_ExcludeGlobs_FailsValidationIfAnyEntryIsEmpty(string value) {
            // Arrange
            var globs = Enumerable.Repeat(value, 3);

            var validator = GetValidator();

            // Act & Assert
            validator.ShouldHaveValidationErrorFor(s => s.ExcludeGlobs, globs)
                .WithErrorMessage("ExcludeGlobs entries must have a value");
        }

    }
}