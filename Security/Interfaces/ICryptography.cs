using System.Text;

namespace Security.Interfaces {

    public interface ICryptography {

        string Encrypt(string value);
        string Encrypt(char[] value);
        string Encrypt(byte[] value, Encoding byteEncoding);

        string Decrypt(string value);
        string Decrypt(char[] value);
        string Decrypt(byte[] value, Encoding byteEncoding);
    }
}