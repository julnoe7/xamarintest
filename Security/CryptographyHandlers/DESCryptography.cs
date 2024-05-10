using Security.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Security.CryptographyHandlers {

    public class DesCryptography : ICryptography {

        #region properties
        private readonly string desKey;
        private readonly byte[] desiv;
        #endregion

        #region constructor
        public DesCryptography() {
            desKey = "64454023";
            desiv = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 };
        }

        public DesCryptography(string desKey) {
            this.desKey = desKey;
            desiv = new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 };
        }
        #endregion

        #region interface implementation

        #region encrypt
        public string Encrypt(string value) {
            return DesEncrypt(value);
        }

        public string Encrypt(char[] value) {
            return DesEncrypt(value.ToString());
        }

        public string Encrypt(byte[] value, Encoding byteEncoding) {
            return DesEncrypt(byteEncoding.GetString(value));
        }
        #endregion

        #region decrypt
        public string Decrypt(string value) {
            return DesDecrypt(value);
        }

        public string Decrypt(char[] value) {
            return DesDecrypt(value.ToString());
        }

        public string Decrypt(byte[] value, Encoding byteEncoding) {
            return DesDecrypt(byteEncoding.GetString(value));
        }
        #endregion

        #endregion

        #region private methods
        private string DesEncrypt(string value) {

            using var memStream = new MemoryStream();
            using (var des = new DESCryptoServiceProvider()) {
                using ICryptoTransform decryptor = des.CreateEncryptor(DesKeyToByte(), desiv);
                using var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Write);
                using var writerStream = new StreamWriter(cryptoStream);
                writerStream.Write(value);
            }

            return Convert.ToBase64String(memStream.ToArray());
        }

        private string DesDecrypt(string value) {

            using var des = new DESCryptoServiceProvider();
            using ICryptoTransform decryptor = des.CreateDecryptor(DesKeyToByte(), desiv);
            using var str = new MemoryStream(Convert.FromBase64String(value));
            using var cryptoStream = new CryptoStream(str, decryptor, CryptoStreamMode.Read);
            using var strReader = new StreamReader(cryptoStream);

            return strReader.ReadToEnd();
        }

        private byte[] DesKeyToByte() {
            return Encoding.UTF8.GetBytes(desKey);
        }
        #endregion
    }
}