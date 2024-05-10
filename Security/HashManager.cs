using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Security {

    public class HashManager {

        #region properties
        private readonly HashAlgorithm passwordAlgorithm;
        #endregion

        #region constructor
        /// <summary>
        /// uses HMACSHA256 as the current algorithm to hash
        /// </summary>
        /// <param name="hashPasswordKey"></param>
        public HashManager(string hashPasswordKey) {
            passwordAlgorithm = new HMACSHA256(Encoding.ASCII.GetBytes(hashPasswordKey));
        }
        /// <summary>
        /// you can pass the actual algorithm to get the hasg with this overload
        /// </summary>
        /// <param name="algorithm"></param>
        public HashManager(HashAlgorithm algorithm) {
            passwordAlgorithm = algorithm;
        }
        #endregion

        #region public methods
        public string GetPassWordHash(string password) {
            return BitConverter.ToString(passwordAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(password))).Replace("-", string.Empty);
        }

        public static string HashHmac(string key, string message, Encoding encoding) {
            using var hmac = new HMACSHA256(encoding.GetBytes(key));
            var hash = hmac.ComputeHash(encoding.GetBytes(message));
            return BitConverter.ToString(hash).ToLower().Replace("-", string.Empty);
        }

        public static bool IsValidSignature(string signature, string body, string secretKey, Encoding encoding) {

            var hashString = new StringBuilder();

            using var crypto = new HMACSHA1(encoding.GetBytes(secretKey));

            byte[] hash = crypto.ComputeHash(Encoding.UTF8.GetBytes(body));

            foreach (byte item in hash) {
                hashString.Append(item.ToString("X2"));
            }

            return hashString.ToString().Equals(signature, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string DecodeSignedRequest(string signedRequest, string appSecret) {

            if (!signedRequest.Contains(".")) return string.Empty;

            string[] split = signedRequest.Split('.');

            string signatureRaw = FixBase64String(split[0]);
            string dataRaw = FixBase64String(split[1]);

            // the decoded signature
            byte[] signature = Convert.FromBase64String(signatureRaw);
            byte[] dataBuffer = Convert.FromBase64String(dataRaw);

            // JSON object
            string data = Encoding.UTF8.GetString(dataBuffer);

            byte[] appSecretBytes = Encoding.UTF8.GetBytes(appSecret);
            using var hmac = new HMACSHA256(appSecretBytes);
            byte[] expectedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(split[1]));

            return expectedHash.SequenceEqual(signature) ? data : string.Empty;
        }

        public static string GetHash512(string content, Encoding encoding) {
            return GetHash512(encoding.GetBytes(content));
        }

        public static string GetHash512(byte[] content) {
            using var sha512 = new SHA512Managed();
            var hash = sha512.ComputeHash(content);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        public static string GetHash(string content) {
            return GetHash(Encoding.ASCII.GetBytes(content));
        }

        public static string GetHash(byte[] content) {
            using var sha256 = SHA256.Create();
            return BitConverter.ToString(sha256.ComputeHash(content)).Replace("-", string.Empty);
        }

        public bool CompareFileContent(byte[] content1, byte[] content2) {
            return GetHash(content1).Equals(GetHash(content2));
        }

        public bool CheckSum(byte[] content1, byte[] content2) {

            using var sha256 = SHA256.Create();

            string f1 = BitConverter.ToString(sha256.ComputeHash(content1)).Replace("-", string.Empty);
            string f2 = BitConverter.ToString(sha256.ComputeHash(content2)).Replace("-", string.Empty);

            return f1.Equals(f2, StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion

        #region private methods
        private static string FixBase64String(string str) {
            while (str.Length % 4 != 0) {
                str = str.PadRight(str.Length + 1, '=');
            }
            return str.Replace("-", "+").Replace("_", "/");
        }
        #endregion
    }
}