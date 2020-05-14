using System;
using System.Threading.Tasks;

using AutoFixture;
using AutoFixture.AutoMoq;

using DNI.Backup.Services.Signature;

using Xunit;
using Xunit.Abstractions;

namespace DNI.Backup.Test.Services.Signature {
    public class SignatureServiceTests {
        private readonly ITestOutputHelper _output;

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization {ConfigureMembers = true});

        public SignatureServiceTests(ITestOutputHelper _output) {
            this._output = _output;
        }

        private ISignatureService GetService() {
            return new SignatureService();
        }

        [Fact]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_IsNullOrEmpty() {
            // Arrange
            var service = GetService();

            // Act & Assert
            throw new NotImplementedException();
        }

        [Fact]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_DoesNotExist() {
            // Arrange
            var service = GetService();

            // Act & Assert
            throw new NotImplementedException();
        }

        [Fact]
        public async Task CreateSignature_ThrowsException_WhenInputFilePath_IsLocked() {
            // Arrange
            var service = GetService();

            // Act & Assert
            throw new NotImplementedException();
        }
    }
}