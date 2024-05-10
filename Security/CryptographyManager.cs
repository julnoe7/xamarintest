using Security.Interfaces;

namespace Security {

    public class CryptographyManager {

        #region Public Methods
        public string Encrypt(string value, ICryptography cryptographicHandler) {
            return cryptographicHandler.Encrypt(value);
        }

        public string Decrypt(string value, ICryptography cryptographicHandler) {
            return cryptographicHandler.Decrypt(value);
        }
        #endregion
    }
}