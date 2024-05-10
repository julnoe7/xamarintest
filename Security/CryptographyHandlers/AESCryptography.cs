using Security.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Security.CryptographyHandlers {

    public class AESCryptography : ICryptography {

        #region properties
        private readonly string AESPassIV;
        private readonly string AESPassKey;
        private readonly string AESSaltIV;
        private readonly string AESSaltKey;
        #endregion

        #region constructor
        public AESCryptography(string aesPassIV, string aesPassKey, string aesSaltIV, string aesSaltKey) {
            AESPassIV = aesPassIV;
            AESPassKey = aesPassKey;
            AESSaltIV = aesSaltIV;
            AESSaltKey = aesSaltKey;
        }
        #endregion

        #region encrypt
        public string Encrypt(string value) {
            return AESEncrypt(value);
        }

        public string Encrypt(char[] value) {
            return AESEncrypt(value.ToString());
        }

        public string Encrypt(byte[] value, Encoding byteEncoding) {
            return AESEncrypt(byteEncoding.GetString(value));
        }
        #endregion

        #region decrypt
        public string Decrypt(string value) {
            return AESDecrypt(value);
        }

        public string Decrypt(char[] value) {
            return AESDecrypt(value.ToString());
        }

        public string Decrypt(byte[] value, Encoding byteEncoding) {
            return AESDecrypt(byteEncoding.GetString(value));
        }
        #endregion

        #region private methods
        private string AESEncrypt(string value) {

            string retValue = string.Empty;

            using (var memStream = new MemoryStream()) {

                using (var aes = new AesCryptoServiceProvider {
                    Padding = PaddingMode.ISO10126,
                    Key = AESKey(),
                    IV = AESIV()
                }) {

                    using ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
                    using var writerStream = new StreamWriter(new CryptoStream(memStream, transform, CryptoStreamMode.Write));
                    writerStream.Write(value);
                }

                retValue = Convert.ToBase64String(memStream.ToArray());
            }

            return retValue;
        }

        private string AESDecrypt(string value) {
            using var aes = new AesCryptoServiceProvider {
                Padding = PaddingMode.ISO10126,
                Key = AESKey(),
                IV = AESIV()
            };
            using ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
            using var readerStream = new StreamReader(new CryptoStream(new MemoryStream(Convert.FromBase64String(value)),
                transform, CryptoStreamMode.Read));

            return readerStream.ReadToEnd();
        }

        private byte[] AESIV() {
            using var derivedBytes = new Rfc2898DeriveBytes(AESPassIV, Encoding.Unicode.GetBytes(AESSaltIV));
            return derivedBytes.GetBytes(16);
        }

        private byte[] AESKey() {
            using var derivedBytes = new Rfc2898DeriveBytes(AESPassKey, Encoding.Unicode.GetBytes(AESSaltKey));
            return derivedBytes.GetBytes(32);
        }
        #endregion
    }
}