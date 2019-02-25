namespace SnepSharp.Snep.Messages
{
    using SnepSharp.Ndef;

    /// <summary>
    /// Snep response message.
    /// </summary>
    internal abstract class SnepResponse : SnepMessage
    {
        /// <summary>
        /// Gets the response status code.
        /// </summary>
        /// <value>The response status code.</value>
        public SnepResponseCode Response { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepResponse"/> class.
        /// </summary>
        /// <param name="response">The snep response code.</param>
        /// <param name="content">The response content.</param>
        protected SnepResponse(SnepResponseCode response, INdefMessage content)
            : this(Constants.DefaultSnepVersion, response, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepResponse"/> class.
        /// </summary>
        /// <param name="version">The snep protocol version.</param>
        /// <param name="response">The snep response code.</param>
        /// <param name="content">The response content.</param>
        protected SnepResponse(
            SnepVersion version, 
            SnepResponseCode response, 
            INdefMessage content)
            : base(version, (byte)response, content)
        {
            this.Response = response;
        }
    }
}
