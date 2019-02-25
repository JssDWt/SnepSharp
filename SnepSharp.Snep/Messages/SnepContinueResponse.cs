namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Snep continue response. Indicates to the client that it can continue to 
    /// send fragments of data.
    /// </summary>
    internal class SnepContinueResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnepContinueResponse"/> 
        /// class.
        /// </summary>
        public SnepContinueResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepContinueResponse"/> 
        /// class, with a specific <see cref="SnepVersion"/>.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepContinueResponse(SnepVersion version)
            : base(version, SnepResponseCode.Continue, null)
        {
        }
    }
}
