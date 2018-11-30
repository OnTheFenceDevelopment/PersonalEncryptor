using CommandLine;
using Newtonsoft.Json;
using PersonalEncrypter.Cli.Options;
using PersonalEncrypter.Shared;
using PersonalEncrypter.SodiumEncypter;
using System;
using System.Collections.Generic;
using System.IO;

namespace PersonalEncrypter.Cli
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<GenerateKeyOptions, EncryptFileOptions, DecryptFileOptions>(args)
                .MapResult(
                (GenerateKeyOptions opts) => GenerateKeyPair(opts),
                (EncryptFileOptions opts) => EncryptFile(opts),
                (DecryptFileOptions opts) => DecryptFile(opts),
                errs => 1);
        }

        static int GenerateKeyPair(GenerateKeyOptions opts)
        {
            try
            {
                if (EnsureOutputFolderExists(opts.OutputPath) == false)
                {
                    WriteMessageToConsole("Output Path Invalid - Key Generation Aborted", ConsoleColor.Red);
                    return 1;
                }

                if (KeysExist(opts.Name, opts.OutputPath))
                {
                    WriteMessageToConsole("One of more keys of the same name already exist in the specified location, Overwrite? (Y/N)", ConsoleColor.Red);

                    var overwrite = Console.ReadKey();

                    while (overwrite.Key != ConsoleKey.Y && overwrite.Key != ConsoleKey.N)
                    {
                        WriteMessageToConsole("Please enter Y or N", ConsoleColor.Red);
                        overwrite = Console.ReadKey();
                    }

                    if (overwrite.Key == ConsoleKey.N)
                        return 0;
                }

                var encrypter = new Encrypter();

                var keyPair = encrypter.GenerateKeyPair();

                File.WriteAllText(Path.Combine(opts.OutputPath, $"{opts.Name}PublicKey.txt"), Convert.ToBase64String(keyPair.PublicKey));
                File.WriteAllText(Path.Combine(opts.OutputPath, $"{opts.Name}PrivateKey.txt"), Convert.ToBase64String(keyPair.PrivateKey));

                WriteMessageToConsole($"Success: Key Pair generated in {opts.OutputPath}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                WriteMessageToConsole($"Exception: {ex.Message}", ConsoleColor.Red);
                return 1;
            }

            return 0;
        }

        static int EncryptFile(EncryptFileOptions opts)
        {
            if (FilesExist(new List<string> { opts.FilePath, opts.RecipientKeyPath, opts.SenderKeyPath }))
            {
                var packet = string.Empty;

                try
                {
                    var fileContents = File.ReadAllBytes(opts.FilePath);

                    var encrypter = new Encrypter();

                    var nonce = encrypter.GenerateNonce();

                    var encryptedData = encrypter.Encrypt(fileContents, nonce, GetKeyBytes(opts.RecipientKeyPath), GetKeyBytes(opts.SenderKeyPath));

                    var encryptedFilename = encrypter.EncryptText(Path.GetFileName(opts.FilePath), nonce, GetKeyBytes(opts.RecipientKeyPath), GetKeyBytes(opts.SenderKeyPath));

                    packet = JsonConvert.SerializeObject(new EncryptedPacket
                    {
                        Filename = encryptedFilename,
                        EncryptedData = encryptedData,
                        Nonce = nonce
                    });
                }
                catch (Exception ex)
                {
                    WriteMessageToConsole($"Error while encrypting data: {ex.Message}", ConsoleColor.Red);
                    return 1;
                }

                try
                {
                    File.WriteAllText(opts.OutputPath, packet);
                    WriteMessageToConsole($"Encrypted data successfully written to {opts.OutputPath}", ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    WriteMessageToConsole($"Exception while writing encrypted data to {opts.OutputPath}: {ex.Message}", ConsoleColor.Red);
                    return 1;
                }

                return 0;
            }
            else
            {
                WriteMessageToConsole("One or more of the specified files were not found - see above", ConsoleColor.Red);
                return 1;
            }
        }
        static int DecryptFile(DecryptFileOptions opts)
        {
            if (FilesExist(new List<string> { opts.FilePath, opts.RecipientKeyPath, opts.SenderKeyPath }))
            {
                byte[] decryptedData = null;
                byte[] originalFilename = null;

                try
                {
                    var packetText = File.ReadAllText(opts.FilePath);
                    var encryptedPacket = JsonConvert.DeserializeObject<EncryptedPacket>(packetText);

                    var encrypter = new Encrypter();
                    decryptedData = encrypter.Decrypt(encryptedPacket.EncryptedData, encryptedPacket.Nonce, GetKeyBytes(opts.RecipientKeyPath), GetKeyBytes(opts.SenderKeyPath));
                    originalFilename = encrypter.Decrypt(encryptedPacket.Filename, encryptedPacket.Nonce, GetKeyBytes(opts.RecipientKeyPath), GetKeyBytes(opts.SenderKeyPath));
                }
                catch (Exception ex)
                {
                    WriteMessageToConsole($"Exception while decrypted data: {ex.Message}", ConsoleColor.Red);
                    return 1;
                }

                try
                {
                    File.WriteAllBytes(Path.Combine(opts.OutputPath, System.Text.Encoding.Default.GetString(originalFilename)), decryptedData);
                    WriteMessageToConsole($"File successfully decrypted to {Path.Combine(opts.OutputPath, System.Text.Encoding.Default.GetString(originalFilename))}", ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    WriteMessageToConsole($"Exception while writing decrypted data to {opts.FilePath} : {ex.Message}", ConsoleColor.Red);
                    return 1;
                }

                return 0;
            }
            else
            {
                WriteMessageToConsole("One or more of the specified files were not found - see above", ConsoleColor.Red);
                return 1;
            }
        }

        #region Helper Methods

        private static bool EnsureOutputFolderExists(string folderPath)
        {
            var outcome = false;

            if (Directory.Exists(folderPath))
            {
                outcome = true;
            }
            else
            {
                WriteMessageToConsole($"The specified output folder [{folderPath}] does not exists - create it? (Y/N)", ConsoleColor.Red);

                var response = Console.ReadKey();
                while (response.Key != ConsoleKey.Y && response.Key != ConsoleKey.N)
                {
                    WriteMessageToConsole("Please enter Y or N", ConsoleColor.Red);
                    response = Console.ReadKey();
                }

                if (response.Key == ConsoleKey.N)
                    return false;

                try
                {
                    Directory.CreateDirectory(folderPath);
                    outcome = true;
                }
                catch (Exception ex)
                {
                    WriteMessageToConsole($"Unable to create folder [{folderPath}] - {ex.Message}", ConsoleColor.Red);
                    return false;
                }
            }

            return outcome;
        }

        private static bool KeysExist(string keyName, string outputPath)
        {
            if (File.Exists(Path.Combine(outputPath, $"{keyName}PublicKey.txt")) || File.Exists(Path.Combine(outputPath, $"{keyName}PrivateKey.txt")))
                return true;

            return false;
        }

        private static void WriteMessageToConsole(string message, ConsoleColor textColour)
        {
            Console.ForegroundColor = textColour;
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.ResetColor();
        }

        private static bool FilesExist(List<string> filePaths)
        {
            var allFilesExist = true;

            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath) == false)
                {
                    WriteMessageToConsole($"File not found: {filePath}", ConsoleColor.Red);
                    allFilesExist = false;
                }
            }

            return allFilesExist;
        }

        private static byte[] GetKeyBytes(string keyPath)
        {
            try
            {
                var key = File.ReadAllText(keyPath);
                return Convert.FromBase64String(key);
            }
            catch (Exception ex)
            {
                WriteMessageToConsole($"Exception while Extracting key from [{keyPath}]: {ex.Message}", ConsoleColor.Red);
                throw;
            }
        }

        #endregion
    }
}
