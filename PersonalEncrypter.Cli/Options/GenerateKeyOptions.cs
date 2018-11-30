using CommandLine;

namespace PersonalEncrypter.Cli.Options
{
    [Verb("generatekeys", HelpText = "Generates a Private/Public key pair")]
    public class GenerateKeyOptions
    {
        [Option('n',"name", HelpText ="Key name prefix, e.g. <name>PrivateKey.txt and <name>PublicKey.txt")]
        public string Name { get; set; }

        [Option('o',"output", HelpText = "Output path for generated key pair files")]
        public string OutputPath { get; set; }
    }
}
