using System;
using System.IO;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Backup.Services.Rsync;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Test.Services.RSync {
    [Trait(TestTraits.TEST_TYPE, TestTraits.INTEGRATION)]
    public class RsyncServiceTests {
        private readonly ITestOutputHelper _output;

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public RsyncServiceTests(ITestOutputHelper _output) {
            this._output = _output;
        }

        private IRsyncService GetService() {
            return new RsyncService();
        }

        #region CreateSignatureAsync

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_IsNullOrEmpty(string inputPath) {
            // Arrange
            var service = GetService();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateSignatureAsync(inputPath));
        }

        [Fact]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_DoesNotExist() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var service = GetService();

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => service.CreateSignatureAsync(filePath));
        }

        [Fact]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_IsLocked() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var service = GetService();

            await using(File.Create(filePath)) {
                // Act & Assert
                var ex = await Assert.ThrowsAsync<IOException>(() => service.CreateSignatureAsync(filePath));
                Assert.Equal($"The process cannot access the file '{filePath}' because it is being used by another process.", ex.Message);
            }

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task CreateSignature_GeneratesValidSignatureStream_WhenFileExists() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            await File.Create(filePath).DisposeAsync();

            var service = GetService();

            // Act
            await using(var stream = await service.CreateSignatureAsync(filePath)) {
                // Assert
                Assert.NotNull(stream);
                Assert.True(stream.CanSeek);
                Assert.Equal(0, stream.Position);
            }

            // Cleanup
            File.Delete(filePath);
        }

        #endregion

        #region CreateDeltaAsync

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task CreateDelta_ThrowsException_WhenInputFilePath_IsNullOrEmpty(string inputPath) {
            // Arrange
            var service = GetService();
            var signatureStream = new MemoryStream();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateDeltaAsync(inputPath, signatureStream));
        }

        [Fact]
        public async Task CreateDelta_ThrowsException_WhenInputFilePath_DoesNotExist() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var signatureStream = new MemoryStream();
            var service = GetService();

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => service.CreateDeltaAsync(filePath, signatureStream));
        }

        [Fact]
        public async Task CreateDelta_ThrowsException_WhenInputFilePath_IsLocked() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var signatureStream = new MemoryStream();
            var service = GetService();

            await using(File.Create(filePath)) {
                // Act & Assert
                var ex = await Assert.ThrowsAsync<IOException>(() => service.CreateDeltaAsync(filePath, signatureStream));
                Assert.Equal($"The process cannot access the file '{filePath}' because it is being used by another process.", ex.Message);
            }

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task CreateDelta_ThrowsException_WhenSignatureStreamIsNull() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var service = GetService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateDeltaAsync(filePath, null));
            Assert.Equal("signatureStream", ex.ParamName);
        }

        [Fact]
        public async Task CreateDelta_GeneratesValidDeltaStream_WhenFileExists_AndSignatureIsValid() {
            // Arrange
            var service = GetService();
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            await File.Create(filePath).DisposeAsync();
            await using(var signatureStream = await service.CreateSignatureAsync(filePath)) {
                // Act
                await using(var stream = await service.CreateDeltaAsync(filePath, signatureStream)) {
                    // Assert
                    Assert.NotNull(stream);
                    Assert.True(stream.CanSeek);
                    Assert.Equal(0, stream.Position);
                }
            }

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task CreateDelta_GeneratesValidDeltaStream_WhenDestinationFileDiffersFromSource() {
            // Arrange
            var service = GetService();
            var sourceFilePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var destinationFilePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            await File.Create(sourceFilePath).DisposeAsync();
            await using(var destinationFileStream = File.CreateText(destinationFilePath)) {
                await destinationFileStream.WriteAsync(_fixture.Create<string>());
            }

            await using(var signatureStream = await service.CreateSignatureAsync(destinationFilePath)) {
                // Act
                await using(var stream = await service.CreateDeltaAsync(sourceFilePath, signatureStream)) {
                    // Assert
                    Assert.NotNull(stream);
                    Assert.True(stream.CanSeek);
                    Assert.Equal(0, stream.Position);
                }
            }

            // Cleanup
            File.Delete(sourceFilePath);
            File.Delete(destinationFilePath);
        }

        #endregion

        #region ApplyDeltaAsync

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task ApplyDelta_ThrowsException_WhenInputFilePath_IsNullOrEmpty(string inputPath) {
            // Arrange
            var service = GetService();
            var deltaStream = new MemoryStream();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.ApplyDeltaAsync(inputPath, deltaStream));
        }

        [Fact]
        public async Task ApplyDelta_ThrowsException_WhenInputFilePath_DoesNotExist() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var deltaStream = new MemoryStream();
            var service = GetService();

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => service.ApplyDeltaAsync(filePath, deltaStream));
        }

        [Fact]
        public async Task ApplyDelta_ThrowsException_WhenInputFilePath_IsLocked() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var deltaStream = new MemoryStream();
            var service = GetService();

            await using(File.Create(filePath)) {
                // Act & Assert
                var ex = await Assert.ThrowsAsync<IOException>(() => service.ApplyDeltaAsync(filePath, deltaStream));
                Assert.Equal($"The process cannot access the file '{filePath}' because it is being used by another process.", ex.Message);
            }

            // Cleanup
            File.Delete(filePath);
        }

        [Fact]
        public async Task ApplyDelta_ThrowsException_WhenDeltaStreamIsNull() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var service = GetService();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => service.ApplyDeltaAsync(filePath, null));
            Assert.Equal("deltaStream", ex.ParamName);
        }

        [Fact]
        public async Task ApplyDelta_GeneratesValidDeltaStream_WhenFileExists_AndDeltaIsValid() {
            // Arrange
            var service = GetService();
            var expectedText = _fixture.Create<string>();
            var destinationFilePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var sourceFilePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            await File.Create(destinationFilePath).DisposeAsync();
            await using(var sourceFileStream = File.CreateText(sourceFilePath)) {
                await sourceFileStream.WriteAsync(expectedText);
            }

            bool result;
            await using(var signatureStream = await service.CreateSignatureAsync(destinationFilePath)) {
                await using(var deltaStream = await service.CreateDeltaAsync(sourceFilePath, signatureStream)) {
                    // Act
                    result = await service.ApplyDeltaAsync(destinationFilePath, deltaStream);
                }
            }

            // Assert
            var actualText = File.ReadAllText(destinationFilePath);
            Assert.Equal(expectedText, actualText);
            Assert.True(result);

            // Cleanup
            File.Delete(destinationFilePath);
            File.Delete(sourceFilePath);
        }

        // TODO: Verify failed delta / copies

        // TODO: Verify invalid signatures

        // TODO: Verify that the temp file is deleted?

        #endregion
    }
}