using System;
using System.IO;
using System.Threading.Tasks;

namespace DNI.Backup.Services.Signature {
    /// <summary>
    ///     Creates a signature stream for the specified file
    /// </summary>
    public class SignatureService : ISignatureService {
        public Task<Stream> CreateSignature(string inputFilePath) {
            //var signatureBuilder = new SignatureBuilder();
            //using(var sourceFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
            //    using(var signatureStream = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
            //        signatureBuilder.Build(sourceFileStream, new SignatureWriter(signatureStream));
            //    }
            //}

            throw new NotImplementedException();
        }
    }

    public interface ISignatureService {
        Task<Stream> CreateSignature(string inputFilePath);
    }
}