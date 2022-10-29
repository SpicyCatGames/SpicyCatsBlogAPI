using System.Text;

namespace SpicyCatsBlogAPI.Utils
{
    public static class GuidEncoder
    {
        public static string Encode(string guidText)
        {
            Guid guid = new Guid(guidText);
            return Encode(guid);
        }

        public static string Encode(Guid guid)
        {
            string enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }

        public static Guid Decode(string encoded)
        {
            byte[] buffer = DecodeToByte(encoded);
            return new Guid(buffer);
        }
        public static string Decode2Str(string encoded)
        {
            byte[] buffer = DecodeToByte(encoded);
            return Encoding.UTF8.GetString(buffer);
        }
        private static byte[] DecodeToByte(string encoded)
        {
            encoded = encoded.Replace("_", "/");
            encoded = encoded.Replace("-", "+");
            return Convert.FromBase64String(encoded + "==");
        }
    }
}
