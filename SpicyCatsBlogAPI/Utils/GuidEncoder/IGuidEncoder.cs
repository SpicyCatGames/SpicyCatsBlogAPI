namespace SpicyCatsBlogAPI.Utils.GuidEncoder
{
    public interface IGuidEncoder
    {
        public string Encode(string guidText);

        public string Encode(Guid guid);

        public Guid Decode(string encoded);
        public string Decode2Str(string encoded);
    }
}
