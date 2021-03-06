﻿## Usage ##

### Generate Key Pair ###
In order to use any encryption system you will need to generate a Key Pair consisting of a Public and Private key. The public key can be freely distributed but the private key must be protected from loss and unauthorised access. 

> If you lose your private key then you will not be able to decrypt messages sent to you that were encrypted using the matching private key.

>If someone else gains access to your private key then they will be able to read any data you have encrypted with it or data someone has encrypted with your public key (providing they also have access to the corresponding public keys that is).

#### Options ####

Action | Option | Notes
-------|--------|-------
generatekeys | | Signals CLI to generate a key pair
|| name | The name prefix for the key pair
|| output | The output path for the keys, if the folder does not exist it will be created

To generate a new Key Pair:

`C:\> dotnet PersonalEncryptor generatekeys --name Alice --output C:\Users\Alice\MyKeys`

Note that omitting the name option will generate files named PrivateKey.xml and PublicKey.xml

> It is highly recommended that the private key be moved to a secure location, separate from the public key.

### Encrypt File ###

Files are encrypted using the senders PUBLIC key and the recipients PRIVATE key to produce an 'encrypted packet, a text file in JSON format. The file also contains a Digital Signature for validating the files contents during decryption. While this file can be opened in any text editor the contents can only be decrypted using the appropriate keys.

#### Options ####
Action | Option | Notes
-------|--------|-------
encryptfile | | Signals CLI to encrypt specified file
| | filepath | The full path to the file to be encrypted
| | senderkeypath | The full path to the sender's PRIVATE key
| | recipientkeypath | The full path to the recipient's PUBLIC key
| | output | The output path (and filename) where the encrypted file will be written to, if the folder does not exist it will be created

#### Usage ####
To encrypt a file, use the following command using the appropriate file paths (note that this is a single line command):

`C:\> dotnet PersonalEncryptorCLI encryptfile --filepath C:\Users\Alice\MyData\InputFile.pdf --senderkeypath C:\Users\Alice\MyKeys\AlicePrivateKey.xml --recipientkeypath C:\Users\Alice\MyKeys\BobPublicKey.xml --output C:\Users\Alice\MyData\TextFile.json`

### Decrypt File ###

Files are decrypted in the same manner as used for encryption but where the sender's PRIVATE and recipient's PUBLIC keys were used to perform the encryption the other key in each pair is now required, i.e. the sender's PUBLIC and recipient's PRIVATE keys. From version 1.1.0 onwards the original filename will be included in the Encrypted Packet and this will be decrypted and used when creating the output file during decryption.

#### Options ####
Action | Option | Notes
-------|--------|-------
decryptfile | | Signals CLI to decrypt specified file
| | pathtopacket | The full path to the file to be decrypted
| | senderkeypath | The full path to the sender's PUBLIC key
| | recipientkeypath | The full path to the recipient's PRIVATE key
| | output | The output path (excluding filename) where the decrypted file will be written to, if the folder does not exist it will be created

#### Usage ####
To decyrpt an 'encrypted packet' JSON file using the sender's PUBLIC key and recipient's PRIVATE key, use the following command using the appropriate file paths (note that this is a single line command)

`C:\> dotnet PersonalEncryptor decryptfile --pathtopacket C:\Users\Bob\MyData\TextFile.json --senderkeypath C:\Users\Bob\MyKeys\AlicePublicKey.xml --recipientkeypath C:\Users\Bob\MyKeys\BobPrivateKey.xml --output C:\Users\Bob\MyData`
