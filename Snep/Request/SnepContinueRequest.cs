namespace Snep
{
    /// <summary>
    /// Snep continue request. Indicates the server can continue to send response fragments.
    /// </summary>
    internal class SnepContinueRequest : SnepRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepContinueRequest"/> class.
        /// </summary>
        public SnepContinueRequest()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepContinueRequest"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepContinueRequest(SnepVersion version)
            : base(version, SnepRequestCode.Continue, null)
        {
        }
    }
}
