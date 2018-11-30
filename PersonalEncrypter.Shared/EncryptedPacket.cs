namespace PersonalEncrypter.Shared
{
    public class EncryptedPacket
    {
        public byte[] EncryptedData { get; set; }

        public byte[] Filename { get; set; }

        public byte[] Nonce { get; set; }
    }
}
