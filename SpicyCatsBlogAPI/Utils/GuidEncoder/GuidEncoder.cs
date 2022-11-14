using System.Text;

namespace SpicyCatsBlogAPI.Utils.GuidEncoder
{
    public class GuidEncoder : IGuidEncoder
    {
        public string Encode(string guidText)
        {
            Guid guid = new Guid(guidText);
            return Encode(guid);
        }

        public string Encode(Guid guid)
        {
            string enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }

        public Guid Decode(string encoded)
        {
            byte[] buffer = DecodeToByte(encoded);
            return new Guid(buffer);
        }
        public string Decode2Str(string encoded)
        {
            return Decode(encoded).ToString();
        }
        private byte[] DecodeToByte(string encoded)
        {
            encoded = encoded.Replace("_", "/");
            encoded = encoded.Replace("-", "+");
            return Convert.FromBase64String(encoded + "==");
        }
    }
}
