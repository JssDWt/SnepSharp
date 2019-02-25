namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Snep not implemented response. The server does not support the 
    /// functionality required to fulfill the request. For example when a 
    /// request code is not recognized.
    /// </summary>
    internal class SnepNotImplementedResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepNotImplementedResponse"/> class.
        /// </summary>
        public SnepNotImplementedResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepNotImplementedResponse"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepNotImplementedResponse(SnepVersion version)
            : base(version, SnepResponseCode.NotImplemented, null)
        {
        }
    }
}
