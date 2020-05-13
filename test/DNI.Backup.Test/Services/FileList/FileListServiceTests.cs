using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Backup.Services.FileList;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Test.Services.FileList {
    public class FileListServiceTests {
        private readonly ITestOutputHelper _output;

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        private Mock<IValidator<BackupDirectorySetting>> _backupDirectorySettingValidatorMock;

        public FileListServiceTests(ITestOutputHelper _output) {
            this._output = _output;

            _backupDirectorySettingValidatorMock = Mock.Get(_fixture.Create<IValidator<BackupDirectorySetting>>());
        }

        private IFileListService GetService() {
            return new FileListService(_backupDirectorySettingValidatorMock.Object);
        }
        // TODO: https://anthonychu.ca/post/async-streams-dotnet-core-3-iasyncenumerable/
        [Fact]
        public async Task GetFiles_ThrowsException_WhenBackupDirectorySettings_IsNull() {
            // Arrange
            var service = GetService();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetFilesAsync(null));
            Assert.Equal("backupDirectorySettings", result.ParamName);
        }

        [Fact]
        public async Task GetFiles_ThrowsException_WhenBackupDirectorySettings_ContainsEmptyEnumerable() {
            // Arrange
            var service = GetService();
            var settings = new BackupDirectorySetting[0];

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentException>(() => service.GetFilesAsync(settings));
            Assert.Equal("backupDirectorySettings", result.ParamName);
        }

        [Fact]
        public async Task GetFiles_CallsValidator_ForEachBackupDirectorySetting() {
            // Arrange
            var service = GetService();
            var settings = _fixture.CreateMany<BackupDirectorySetting>(3);

            // Act
            var result = service.GetFilesAsync(settings);

            // Assert
            _backupDirectorySettingValidatorMock
                .Verify(x => x.ValidateAsync(It.IsAny<BackupDirectorySetting>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GetFiles_ThrowsValidationError_WhenValidatorReturnsErrors() {
            // Arrange
            var service = GetService();
            var settings = _fixture.CreateMany<BackupDirectorySetting>(1);
            var errors = _fixture.CreateMany<ValidationFailure>(3).ToList();
            var validationResult = new ValidationResult(errors);
            _backupDirectorySettingValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<BackupDirectorySetting>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => validationResult);

            // Act & Assert
            var result = await Assert.ThrowsAsync<BackupDirectoryValidationException>(() => service.GetFilesAsync(settings));
            var newline = Environment.NewLine;
            Assert.Equal($"The following validation errors occurred:{newline}" +
                         $"{errors[0].PropertyName} | {errors[0].ErrorMessage}{newline}" +
                         $"{errors[1].PropertyName} | {errors[1].ErrorMessage}{newline}" +
                         $"{errors[2].PropertyName} | {errors[2].ErrorMessage}{newline}", result.Message);
        }

        [Fact]
        public async Task GetFiles_ReturnsAllFilePathsWhenExpected() {
            // Arrange
            var rootPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(rootPath);
            var files = new[] {
                $@"{rootPath}\test.txt",
                $@"{rootPath}\test\test.txt",
                $@"{rootPath}\.git\test.txt",
                $@"{rootPath}\temp\jeff\test.txt"
            };
            foreach(var f in files) {
                var directoryPath = Path.GetDirectoryName(f);
                Directory.CreateDirectory(directoryPath);
                File.Create(f);
            }

            var service = GetService();
            var settings = new BackupDirectorySetting {
                RootDir = rootPath,
                IncludeGlob = "**/*"
            };

            // Act
            await foreach(var file in service.GetFilesAsync(new[] {settings})) {
                Assert.Contains(file, files);
            }

            // Cleanup
            // Directory.Delete(rootPath, true);
        }
    }
}