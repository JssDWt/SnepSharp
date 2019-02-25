namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Snep excess data response. Indicates the resource would exceed the 
    /// maximum acceptable length. 
    /// </summary>
    internal class SnepExcessDataResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepExcessDataResponse"/> class.
        /// </summary>
        public SnepExcessDataResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepExcessDataResponse"/> class, with a specific
        /// <see cref="SnepVersion"/>.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepExcessDataResponse(SnepVersion version)
            : base(version, SnepResponseCode.ExcessData, null)
        {
        }
    }
}
