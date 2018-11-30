using PersonalEncrypter.Shared;
using PersonalEncrypter.Shared.Interfaces;
using System;
using System.Text;

namespace PersonalEncrypter.SodiumEncypter
{
    public class Encrypter : IEncrypter
    {
        public KeyPair GenerateKeyPair()
        {
            var sodiumKeyPair = Sodium.PublicKeyBox.GenerateKeyPair();

            return new KeyPair(sodiumKeyPair.PublicKey, sodiumKeyPair.PrivateKey);
        }

        public byte[] Encrypt(byte[] dataToEncrypt, byte[] nonce, byte[] recipientPublicKey, byte[] sendersPrivateKey)
        {
            return Sodium.PublicKeyBox.Create(dataToEncrypt, nonce, sendersPrivateKey, recipientPublicKey);
        }

        public byte[] EncryptText(string textToEncrypt, byte[] nonce, byte[] recipientPublicKey, byte[] sendersPrivateKey)
        {
            var stringData = Encoding.ASCII.GetBytes(textToEncrypt);
            return Encrypt(stringData, nonce, recipientPublicKey, sendersPrivateKey);
        }

        public byte[] Decrypt(byte[] dataToDecrypt, byte[] nonce, byte[] recipientPrivateKey, byte[] sendersPublicKey)
        {
            return Sodium.PublicKeyBox.Open(dataToDecrypt, nonce, recipientPrivateKey, sendersPublicKey);
        }

        public byte[] GenerateNonce()
        {
            return Sodium.PublicKeyBox.GenerateNonce();
        }
    }
}
