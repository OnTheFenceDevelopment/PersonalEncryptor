using CommandLine;

namespace PersonalEncrypter.Cli.Options
{
    [Verb("encryptfile", HelpText = "Encrypts specified file into an Encrypted Packet")]
    public class EncryptFileOptions
    {
        [Option('f',"filepath", HelpText ="The path to the file to be encrypted (include filename)", Required = true)]
        public string FilePath { get; set; }

        [Option('s',"senderkeypath", HelpText = "Full path to senders PRIVATE key (including filename)", Required = true)]
        public string SenderKeyPath { get; set; }

        [Option('r', "recipientkeypath", HelpText = "Full path to recipients PUBLIC key (including filename)", Required = true)]
        public string RecipientKeyPath { get; set; }

        [Option('o', "output", HelpText = "Output path for generated Encrypted Packet File (including filename)", Required = true)]
        public string OutputPath { get; set; }
    }
}
