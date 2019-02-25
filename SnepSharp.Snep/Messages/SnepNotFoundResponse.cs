namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Snep not found response. Indicates the requested resource cannot be found.
    /// </summary>
    internal class SnepNotFoundResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepNotFoundResponse"/> class.
        /// </summary>
        public SnepNotFoundResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepNotFoundResponse"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepNotFoundResponse(SnepVersion version)
            : base(version, SnepResponseCode.NotFound, null)
        {
        }
    }
}
