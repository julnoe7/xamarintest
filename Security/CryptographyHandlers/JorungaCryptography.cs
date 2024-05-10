using Security.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security.CryptographyHandlers {

    public class JorungaCryptography: ICryptography {

        #region properties
        private readonly Dictionary<int, int> JorungaKeys;
        private readonly Dictionary<char, char> JorungaValues;
        #endregion

        #region constructor
        public JorungaCryptography(Dictionary<int, int> jorungaKeys, Dictionary<char, char> jorungaValues) {
            JorungaKeys = jorungaKeys;
            JorungaValues = jorungaValues;
        }
        #endregion

        #region interface implementation

        #region encrypt
        public string Encrypt(string value) {
            return JorEncrypt(value);
        }

        public string Encrypt(char[] value) {
            return JorEncrypt(value.ToString());
        }

        public string Encrypt(byte[] value, Encoding byteEncoding) {
            return JorEncrypt(byteEncoding.GetString(value));
        }
        #endregion

        #region decrypt
        public string Decrypt(string value) {
            return JorDecrypt(value);
        }

        public string Decrypt(char[] value) {
            return JorDecrypt(value.ToString());
        }

        public string Decrypt(byte[] value, Encoding byteEncoding) {
            return JorDecrypt(byteEncoding.GetString(value));
        }
        #endregion

        #endregion

        #region private methods
        private string JorEncrypt(string value) {

            string retValue = string.Empty;

            if (value.Length == 16 && long.TryParse(value, out _)) {

                for (int indexChar = 1; indexChar <= value.Length; indexChar++) {
                    // Get the index of the char which represents the current char index of the return string
                    int indexOrigin = JorungaKeys.Values.ToList().IndexOf(indexChar);
                    // Get the value of the origin index char, and return the char value of the selected char
                    retValue += JorungaValues[value[indexOrigin]];
                }
            }

            return retValue;
        }

        private string JorDecrypt(string value) {

            string retValue = string.Empty;

            if (value.Length == 16 && long.TryParse(value, out _)) {

                for (int indexChar = 1; indexChar <= value.Length; indexChar++) {
                    // Get the index of the char which represents the current char index of the return string
                    int indexOrigin = JorungaKeys[indexChar] - 1;
                    // Get the value of the origin index char, and return the char value of the selected char
                    retValue += JorungaKeys.Keys.ToList()[JorungaValues.Values.ToList().IndexOf(value[indexOrigin])];
                }
            }

            return retValue;
        }
        #endregion
    }
}