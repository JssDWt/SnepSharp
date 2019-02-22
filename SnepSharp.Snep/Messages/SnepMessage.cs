namespace SnepSharp.Snep.Messages
{
    using SnepSharp.Common;

    /// <summary>
    /// Snep message base class.
    /// </summary>
    internal abstract class SnepMessage
    {
        /// <summary>
        /// The length of the snep header.
        /// </summary>
        private const int HeaderLength = 6;

        /// <summary>
        /// Field containing the header for the snep message.
        /// </summary>
        protected byte[] snepHeader;

        /// <summary>
        /// Gets the snep protocol version.
        /// </summary>
        /// <value>The snep protocol version.</value>
        public SnepVersion Version { get; }

        /// <summary>
        /// Gets the length of the message content.
        /// </summary>
        /// <value>The length of the content.</value>
        public int ContentLength { get; }

        /// <summary>
        /// Gets the length of the entire snep message.
        /// </summary>
        /// <value>The length of the message.</value>
        public int MessageLength => ContentLength + HeaderLength;

        /// <summary>
        /// Gets the message content.
        /// </summary>
        /// <value>The information.</value>
        public NdefMessage Information { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepMessageBase"/> class,
        /// with the default snep protocol version.
        /// </summary>
        /// <param name="command">The snep request or response.</param>
        /// <param name="message">The snep content.</param>
        protected SnepMessage(byte command, NdefMessage message)
            : this(Constants.DefaultSnepVersion, command, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepMessageBase"/> class.
        /// </summary>
        /// <param name="version">The snep protocol version.</param>
        /// <param name="command">The snep request or response.</param>
        /// <param name="message">The snep content.</param>
        protected SnepMessage(SnepVersion version, byte command, NdefMessage message)
        {
            this.Version = version;
            this.Information = message;
            this.ContentLength = message?.Length ?? 0;

            this.snepHeader = new byte[6];
            this.snepHeader[0] = (byte)this.Version;
            this.snepHeader[1] = command;
            this.ContentLength.ToByteArray().CopyTo(this.snepHeader, 2);
        }
    }
}
