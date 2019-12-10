using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Galaxy.Auth.Core.Helpers
{
    public static class Encryptor
    {
        public static string EncryptText(string text, string password)
        {
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(text);
            
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Empty password");
            
            // Get the bytes of the string
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            var bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            return Convert.ToBase64String(bytesEncrypted);
        }

        public static string DecryptText(string input, string password)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Empty input", nameof(input));
            
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Empty password", nameof(password));

            var bytesToBeDecrypted = Convert.FromBase64String(input);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes;

            using (var ms = new MemoryStream())
            {
                using (var aes = Aes.Create())
                {
                    if (aes == null)
                        return new byte[0];
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    aes.Key = passwordBytes;

                    var iv = new byte[16];
                    RandomNumberGenerator.Create().GetBytes(iv);

                    aes.Mode = CipherMode.CBC;
                    ms.Write(iv, 0, iv.Length);

                    aes.IV = iv;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes;

            var iv = new byte[16];
            Array.Copy(bytesToBeDecrypted, 0, iv, 0, iv.Length);

            using (var ms = new MemoryStream())
            {
                using (var aes = Aes.Create())
                {
                    if (aes == null)
                        return new byte[0];

                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    aes.Key = passwordBytes;
                    aes.IV = iv;

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
                    using (var binaryWriter = new BinaryWriter(cs))
                    {
                        //Decrypt Cipher Text from Message
                        binaryWriter.Write(
                            bytesToBeDecrypted,
                            iv.Length,
                            bytesToBeDecrypted.Length - iv.Length
                        );
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
