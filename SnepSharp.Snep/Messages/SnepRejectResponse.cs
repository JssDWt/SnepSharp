namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Indicates the snep server is unable to receive remaining fragments.
    /// Returned after receipt of the first fragment of a fragmented snep 
    /// message.
    /// </summary>
    internal class SnepRejectResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnepRejectResponse"/> 
        /// class.
        /// </summary>
        public SnepRejectResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepRejectResponse"/> 
        /// class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepRejectResponse(SnepVersion version)
            : base(version, SnepResponseCode.Reject, null)
        {
        }
    }
}
