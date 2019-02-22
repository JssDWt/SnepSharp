namespace Snep
{
    /// <summary>
    /// Snep excess data response. Indicates the resource would exceed the maximum acceptable length. 
    /// </summary>
    internal class SnepExcessDataResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepExcessDataResponse"/> class.
        /// </summary>
        public SnepExcessDataResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepExcessDataResponse"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepExcessDataResponse(SnepVersion version)
            : base(version, SnepResponseCode.ExcessData, null)
        {
        }
    }
}
