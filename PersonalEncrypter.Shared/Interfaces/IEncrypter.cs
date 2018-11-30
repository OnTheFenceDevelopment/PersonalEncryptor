using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalEncrypter.Shared.Interfaces
{
    public interface IEncrypter
    {
        byte[] Decrypt(byte[] dataToDecrypt, byte[] nonce, byte[] recipientPrivateKey, byte[] sendersPublicKey);
        byte[] Encrypt(byte[] dataToEncrypt, byte[] nonce, byte[] recipientPublicKey, byte[] sendersPrivateKey);
        byte[] EncryptText(string textToEncrypt, byte[] nonce, byte[] recipientPublicKey, byte[] sendersPrivateKey);
        KeyPair GenerateKeyPair();
        byte[] GenerateNonce();
    }
}
