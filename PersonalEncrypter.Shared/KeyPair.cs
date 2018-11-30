namespace PersonalEncrypter.Shared
{
    public class KeyPair
    {
        public KeyPair(byte[] publicKey, byte[] privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public byte[] PrivateKey { get; set; }
        public byte[] PublicKey { get; set; }
    }
}
