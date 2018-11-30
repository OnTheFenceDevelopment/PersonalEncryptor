using CommandLine;

namespace PersonalEncrypter.Cli.Options
{

    [Verb("decryptfile", HelpText = "Decrypts specified file")]
    public class DecryptFileOptions
    {
        [Option('f', "filepath", HelpText = "The path to the file to be decrypted (include filename)", Required = true)]
        public string FilePath { get; set; }

        [Option('s', "senderkeypath", HelpText = "Full path to senders PUBLIC key (including filename)", Required = true)]
        public string SenderKeyPath { get; set; }

        [Option('r', "recipientkeypath", HelpText = "Full path to recipients PRIVATE key (including filename)", Required = true)]
        public string RecipientKeyPath { get; set; }

        [Option('o', "output", HelpText = "Output path for decrypted file (excluding filename - original filename will be used)", Required = true)]
        public string OutputPath { get; set; }
    }
}
