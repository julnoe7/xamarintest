using System;
using System.Globalization;
using System.Text;

namespace Security.Extensions {

    public static class StringExtensions {

        public static string ToHex(this int number) {
            return number.ToString("X");
        }

        public static int FromHex(this string hexNumber) {
            if (int.TryParse(hexNumber, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int result)) {
                return result;
            }

            throw new FormatException("not a hex number provided");
        }

        public static string ToHexString(this string str, Encoding encoding) {

            var sb = new StringBuilder();

            var bytes = encoding.GetBytes(str);

            foreach (byte t in bytes) {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string FromHexString(this string hexString, Encoding encoding) {

            var bytes = new byte[hexString.Length / 2];

            for (var i = 0; i < bytes.Length; i++) {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return encoding.GetString(bytes);
        }
    }
}