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

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_IsNullOrEmpty(string inputPath) {
            // Arrange
            var service = GetService();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateSignature(inputPath));
        }

        [Fact]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_DoesNotExist() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var service = GetService();

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => service.CreateSignature(filePath));
        }

        [Fact]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_IsLocked() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            var service = GetService();

            await using(File.Create(filePath)) {
                // Act & Assert
                var ex = await Assert.ThrowsAsync<IOException>(() => service.CreateSignature(filePath));
                Assert.Equal($"The process cannot access the file '{filePath}' because it is being used by another process.", ex.Message);
            }

            File.Delete(filePath);
        }

        [Fact]
        public async Task CreateSignature_GeneratesValidSignatureStream_WhenFileExists() {
            // Arrange
            var filePath = Path.Join(Path.GetTempPath(), $"{Guid.NewGuid()}.temp");
            File.Create(filePath).Dispose();

            var service = GetService();

            // Act
            await using(var stream = await service.CreateSignature(filePath)) {
                // Assert
                Assert.NotNull(stream);
                Assert.True(stream.CanSeek);
                Assert.Equal(0, stream.Position);
            }

            File.Delete(filePath);
        }
    }
}