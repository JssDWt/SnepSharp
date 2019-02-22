namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Snep bad request response. Indicates the request could not be understood by the server due to malformed syntax.
    /// </summary>
    internal class SnepBadRequestResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepBadRequestResponse"/> class.
        /// </summary>
        public SnepBadRequestResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepBadRequestResponse"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepBadRequestResponse(SnepVersion version)
            : base(version, SnepResponseCode.BadRequest, null)
        {
        }
    }
}
