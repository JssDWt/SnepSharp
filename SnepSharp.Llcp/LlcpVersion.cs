namespace SnepSharp.Llcp
{
    internal struct LlcpVersion
    {
        public const LlcpVersion V10 = 0x10;

        public int Major { get; }
        public int Minor { get; }
        public byte Version { get; }

        public LlcpVersion(byte version)
        {
            this.Version = version;
            this.Major = (version & 0xF0) >> 4;
            this.Minor = (version & 0x0F);
        }

        public LlcpVersion(int major, int minor)
        {
            this.Major = major;
            this.Minor = minor;
            this.Version = (byte)((major << 4) | minor);
        }
    }
}
