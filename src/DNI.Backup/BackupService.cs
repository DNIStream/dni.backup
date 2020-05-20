﻿using System.IO;

using FastRsync.Core;
using FastRsync.Delta;
using FastRsync.Diagnostics;
using FastRsync.Signature;

namespace DNI.Backup {
    // https://github.com/GrzegorzBlok/FastRsyncNet

    public class BackupService {
        public void Test() {
            // Data we need to store for each file: Full file path, last signature
            //var sourceFilePath = @"D:\MediaProjects\obs\DNI-2020-04-08_12-57-08.mkv";
            //var destinationFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\Game-2020-04-23_18-48-49.mkv";
            var sourceFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test.txt";
            var destinationFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test-backup.txt";
            var newDestinationFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test-backup-new.txt";
            var deltaFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test.delta.txt";
            var signatureFilePath = @"D:\Git\Personal\DNI\dni.backup\test\DNI.Backup.Test\resources\test.sig.txt";

            // Machine A Check if "new file" exists, if not, just copy it
            if(!File.Exists(destinationFilePath)) {
                File.Copy(sourceFilePath, destinationFilePath);
                return;
            }

            // Machine B initiates a backup, sends request to machine A

            // Machine A (destination / backup server) - Build the signature of the destination file
            var signatureBuilder = new SignatureBuilder();
            using(var destinationFileStream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using(var signatureStream = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                    signatureBuilder.Build(destinationFileStream, new SignatureWriter(signatureStream));
                }
            }

            // Machine A sends the signature to Machine B

            // Machine B (source / workstation) - Produce the delta
            var deltaBuilder = new DeltaBuilder {
                ProgressReport = new ConsoleProgressReporter()
            };
            using(var sourceFileStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using(var signatureStream = new FileStream(signatureFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using(var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read)) {
                        deltaBuilder.BuildDelta(sourceFileStream, new SignatureReader(signatureStream, deltaBuilder.ProgressReport),
                            new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
                    }
                }
            }

            // Machine B responds / sends the delta to Machine A

            // Machine A (destination / backup server) - Patch local (destination) file with delta file
            var deltaApplier = new DeltaApplier {
                SkipHashCheck = false
            };

            using(var destinationFileStream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                using(var deltaStream = new FileStream(deltaFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using(var newDestinationFileStream =
                        new FileStream(newDestinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
                        deltaApplier.Apply(destinationFileStream, new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter()),
                            newDestinationFileStream);
                    }
                }
            }

            // Machine A (destination / backup server) renames / writes new file to original file path - or I figure out how to write to the same stream above
        }
    }
}