namespace SnepSharp.Snep.Messages
{
    using System.IO;
    using System.Linq;
    using SnepSharp.Common;

    /// <summary>
    /// Snep request message.
    /// </summary>
    internal class SnepRequest : SnepMessage
    {
        /// <summary>
        /// Prefix for the information field.
        /// </summary>
        private readonly byte[] informationPrefix;

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
            this.informationPrefix = informationPrefix;
        }

        /// <summary>
        /// Appends the information prefix to the base information stream.
        /// </summary>
        /// <returns>The stream.</returns>
        protected override Stream InformationAsStream()
        {
            Stream baseStream = base.InformationAsStream();

            if (this.informationPrefix == null || baseStream == null)
            {
                return baseStream;
            }

            return new ByteHeaderStream(baseStream, this.informationPrefix, disposeInner: true);
        }
    }
}
