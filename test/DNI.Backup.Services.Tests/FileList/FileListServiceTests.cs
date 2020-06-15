using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Backup.Services.BackupInitialiser;
using DNI.Backup.Services.FileList;
using DNI.Backup.TestHelpers;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Services.Tests.FileList {
    [Trait(TestTraits.TEST_TYPE, TestTraits.INTEGRATION)]
    public class FileListServiceTests {
        private readonly ITestOutputHelper _output;

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        private readonly Mock<IValidator<DirectoryGlobSetting>> _directoryGlobSettingsValidatorMock;

        public FileListServiceTests(ITestOutputHelper _output) {
            this._output = _output;

            _directoryGlobSettingsValidatorMock = Mock.Get(_fixture.Create<IValidator<DirectoryGlobSetting>>());
        }

        private IFileListService GetService() {
            return new FileListService(_directoryGlobSettingsValidatorMock.Object);
        }

        // TODO: https://anthonychu.ca/post/async-streams-dotnet-core-3-iasyncenumerable/
        [Fact]
        public async Task GetFiles_ThrowsException_WhenBackupDirectorySettings_IsNull() {
            // Arrange
            var service = GetService();

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetFilesAsync(null));
            Assert.Equal("directoryGlobSettings", result.ParamName);
        }

        [Fact]
        public async Task GetFiles_ThrowsException_WhenBackupDirectorySettings_ContainsEmptyEnumerable() {
            // Arrange
            var service = GetService();
            var settings = new DirectoryGlobSetting[0];

            // Act & Assert
            var result = await Assert.ThrowsAsync<ArgumentException>(() => service.GetFilesAsync(settings));
            Assert.Equal("directoryGlobSettings", result.ParamName);
        }

        [Fact]
        public async Task GetFiles_CallsValidator_ForEachBackupDirectorySetting() {
            // Arrange
            var service = GetService();
            var settings = _fixture.CreateMany<IDirectoryGlobSettings>(3);

            // Act
            var result = await service.GetFilesAsync(settings);

            // Assert
            _directoryGlobSettingsValidatorMock
                .Verify(x => x.ValidateAsync(It.IsAny<IDirectoryGlobSettings>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        public async Task GetFiles_ThrowsValidationError_WhenValidatorReturnsErrors() {
            // Arrange
            var service = GetService();
            var settings = _fixture.CreateMany<IDirectoryGlobSettings>(1);
            var errors = _fixture.CreateMany<ValidationFailure>(3).ToList();
            var validationResult = new ValidationResult(errors);
            _directoryGlobSettingsValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<IDirectoryGlobSettings>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => validationResult);

            // Act & Assert
            var result = await Assert.ThrowsAsync<DirectoryGlobSettingsValidationException>(() => service.GetFilesAsync(settings));
            var newline = Environment.NewLine;
            Assert.Equal($"The following validation errors occurred:{newline}" +
                         $"{errors[0].PropertyName} | {errors[0].ErrorMessage}{newline}" +
                         $"{errors[1].PropertyName} | {errors[1].ErrorMessage}{newline}" +
                         $"{errors[2].PropertyName} | {errors[2].ErrorMessage}{newline}", result.Message);
        }

        [Fact]
        public async Task GetFiles_ReturnsAllFilePaths_WhenCatchAllGlob_AndNoExcludesSpecified() {
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
                File.Create(f).Dispose();
            }

            var service = GetService();
            var settings = new BackupSet {
                SourceRootDir = rootPath,
                IncludeGlob = "**/*"
            };

            // Act
            var results = (await service.GetFilesAsync(new[] {settings})).ToArray();
            Assert.Equal(4, results.Length);
            foreach(var file in results) {
                Assert.Contains(file, files);
            }

            // Cleanup
            Directory.Delete(rootPath, true);
        }

        [Fact]
        public async Task GetFiles_ReturnsOnlyFilePaths_SpecifiedInIncludeGlob() {
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
                File.Create(f).Dispose();
            }

            var service = GetService();
            var settings = new BackupSet {
                SourceRootDir = rootPath,
                IncludeGlob = ".git/"
            };

            // Act
            var results = (await service.GetFilesAsync(new[] {settings})).ToArray();
            Assert.Single(results);
            Assert.Equal($@"{rootPath}\.git\test.txt", results[0]);

            // Cleanup
            Directory.Delete(rootPath, true);
        }

        [Fact]
        public async Task GetFiles_ReturnsAllFilePathsExceptExcludedFileGlob() {
            // Arrange
            var rootPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(rootPath);
            var files = new[] {
                $@"{rootPath}\test.txt",
                $@"{rootPath}\test\test.txt",
                $@"{rootPath}\.git\test.txt",
                $@"{rootPath}\temp\jeff\test.txt",
                $@"{rootPath}\test1.txt",
                $@"{rootPath}\jeff\test1.txt"
            };
            foreach(var f in files) {
                var directoryPath = Path.GetDirectoryName(f);
                Directory.CreateDirectory(directoryPath);
                File.Create(f).Dispose();
            }

            var service = GetService();
            var settings = new BackupSet {
                SourceRootDir = rootPath,
                IncludeGlob = "**/*",
                ExcludeGlobs = new[] {
                    "**/test.txt"
                }
            };

            // Act
            var results = (await service.GetFilesAsync(new[] {settings})).ToArray();
            Assert.Equal(2, results.Length);
            Assert.Equal(results, new[] {
                $@"{rootPath}\test1.txt",
                $@"{rootPath}\jeff\test1.txt"
            }, StringComparer.InvariantCulture);

            // Cleanup
            Directory.Delete(rootPath, true);
        }

        [Fact]
        public async Task GetFiles_ReturnsAllFilePathsExceptExcludedFileGlobs_AndReordersAlphabetically() {
            // Arrange
            var rootPath = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(rootPath);
            var files = new[] {
                $@"{rootPath}\test.txt",
                $@"{rootPath}\test\test.txt",
                $@"{rootPath}\test\chris\chris.png",
                $@"{rootPath}\.git\test.txt",
                $@"{rootPath}\.git\chris.png",
                $@"{rootPath}\temp\jeff\test.txt",
                $@"{rootPath}\test1.txt",
                $@"{rootPath}\jeff\test1.txt"
            };
            foreach(var f in files) {
                var directoryPath = Path.GetDirectoryName(f);
                Directory.CreateDirectory(directoryPath);
                File.Create(f).Dispose();
            }

            var service = GetService();
            var settings = new BackupSet {
                SourceRootDir = rootPath,
                IncludeGlob = "**/*",
                ExcludeGlobs = new[] {
                    "*/test*.txt",
                    ".git/**/*"
                }
            };

            // Act
            var results = (await service.GetFilesAsync(new[] {settings})).ToArray();
            var expected = new[] {
                $@"{rootPath}\test.txt",
                $@"{rootPath}\test1.txt",
                $@"{rootPath}\temp\jeff\test.txt",
                $@"{rootPath}\test\chris\chris.png"
            };
            _output.WriteLine("Actual:");
            _output.WriteLine(string.Join('\n', results));

            _output.WriteLine("Expected:");
            _output.WriteLine(string.Join('\n', expected));
            Assert.Equal(4, results.Length);
            Assert.Equal(results, expected, StringComparer.InvariantCulture);

            // Cleanup
            Directory.Delete(rootPath, true);
        }
    }
}