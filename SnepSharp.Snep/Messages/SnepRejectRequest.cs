namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Snep reject request. Indicates that the server response is rejected.
    /// </summary>
    internal class SnepRejectRequest : SnepRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepRejectRequest"/> class.
        /// </summary>
        public SnepRejectRequest()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepRejectRequest"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepRejectRequest(SnepVersion version)
            : base(version, SnepRequestCode.Reject, null)
        {
        }
    }
}
