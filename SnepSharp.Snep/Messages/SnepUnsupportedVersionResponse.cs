namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Snep unsupported version response. Indicates the request protocol version is not supported by the server.
    /// </summary>
    internal class SnepUnsupportedVersionResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepUnsupportedVersionResponse"/> class.
        /// </summary>
        public SnepUnsupportedVersionResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepUnsupportedVersionResponse"/> class.
        /// </summary>
        /// <param name="version">Snep Protocol version.</param>
        public SnepUnsupportedVersionResponse(SnepVersion version)
            : base(version, SnepResponseCode.UnsupportedVersion, null)
        {
        }
    }
}
