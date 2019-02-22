namespace Snep
{
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Snep request message.
    /// </summary>
    internal class SnepRequest : SnepMessage
    {
        /// <summary>
        /// Full snep header.
        /// </summary>
        private readonly byte[] fullHeader;

        /// <summary>
        /// Gets the request operation/command.
        /// </summary>
        /// <value>The request operation/command.</value>
        public SnepRequestCode Request { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepRequestMessage"/> class.
        /// </summary>
        /// <param name="request">The request code.</param>
        /// <param name="content">The request content.</param>
        /// <param name="informationPrefix">An optional prefix field to the information field.</param>
        public SnepRequest(SnepRequestCode request, NdefMessage content, byte[] informationPrefix = null)
            : this(Constants.DefaultSnepVersion, request, content, informationPrefix)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepRequestMessage"/> class.
        /// </summary>
        /// <param name="version">The snep protocol version.</param>
        /// <param name="request">The request code.</param>
        /// <param name="content">The request content.</param>
        /// <param name="informationPrefix">An optional prefix field to the information field.</param>
        public SnepRequest(SnepVersion version, SnepRequestCode request, NdefMessage content, byte[] informationPrefix = null)
            :base(version, (byte)request, content)
        {
            this.Request = request;

            if (informationPrefix == null)
            {
                this.fullHeader = this.snepHeader;
            }
            else
            {
                this.fullHeader = this.snepHeader.Concat(informationPrefix).ToArray();
            }
        }

        /// <summary>
        /// Creates a <see cref="Stream"/> representation of the message.
        /// </summary>
        /// <returns>The stream.</returns>
        public virtual Stream AsStream()
        {
            Stream result;
            if (this.Information == null)
            {
                result = new MemoryStream(this.snepHeader);
            }
            else
            {
                result = new ByteHeaderStream(this.Information.AsStream(), this.fullHeader);
            }

            return result;
        }
    }
}
