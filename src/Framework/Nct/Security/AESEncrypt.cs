using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Nct.Security
{
  
    public class AESEncrypt
    {
        public static AESEncrypt Instance { get; }

        public byte[] InitVectorBytes;

        public static string DefaultPassPhrase { get; set; }

       
        public static byte[] DefaultInitVectorBytes { get; set; }

     
        public static byte[] DefaultSalt { get; set; }

      
        public const int Keysize = 256;

        static AESEncrypt()
        {
            DefaultPassPhrase = "xcGnGZ041HLL1IM0";
            DefaultInitVectorBytes = Encoding.ASCII.GetBytes("mfE49230Yf259vo9");
            DefaultSalt = Encoding.ASCII.GetBytes("nct2jd10");
            Instance = new AESEncrypt();
        }

        public AESEncrypt()
        {
            InitVectorBytes = DefaultInitVectorBytes;
        }

        public string Encrypt(string plainText, string passPhrase = null, byte[] salt = null)
        {
            if (plainText == null)
            {
                return null;
            }

            passPhrase ??= DefaultPassPhrase;

            salt ??= DefaultSalt;

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, salt,100,HashAlgorithmName.SHA256))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = Aes.Create())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, InitVectorBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public string Decrypt(string cipherText, string passPhrase = null, byte[] salt = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return null;
            }

            passPhrase ??= DefaultPassPhrase;

            salt ??= DefaultSalt;

            var cipherTextBytes = Convert.FromBase64String(cipherText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, salt, 100, HashAlgorithmName.SHA256))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = Aes.Create())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, InitVectorBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                int totalDecryptedByteCount = 0;
                                while (totalDecryptedByteCount < plainTextBytes.Length)
                                {
                                    var decryptedByteCount = cryptoStream.Read(
                                        plainTextBytes,
                                        totalDecryptedByteCount,
                                        plainTextBytes.Length - totalDecryptedByteCount
                                    );

                                    if (decryptedByteCount == 0)
                                    {
                                        break;
                                    }

                                    totalDecryptedByteCount += decryptedByteCount;
                                }

                                return Encoding.UTF8.GetString(plainTextBytes, 0, totalDecryptedByteCount);
                            }
                        }
                    }
                }
            }

        }

    }
}
