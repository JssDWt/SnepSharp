﻿namespace SnepSharp.Snep.Messages
{
    using System;
    using System.IO;
    using SnepSharp.Common;

    /// <summary>
    /// Snep message base class.
    /// </summary>
    internal class SnepMessage
    {
        /// <summary>
        /// Gets the snep header of the message.
        /// </summary>
        /// <value>The header.</value>
        public SnepHeader Header { get; }

        /// <summary>
        /// Gets the length of the entire snep message.
        /// </summary>
        /// <value>The length of the message.</value>
        public int MessageLength => this.Header.ContentLength + Constants.SnepHeaderLength;

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
            this.Information = message;
            this.Header = new SnepHeader(version, command, message?.Length ?? 0);
        }

        /// <summary>
        /// Creates a <see cref="Stream"/> representation of the message.
        /// </summary>
        /// <returns>The stream.</returns>
        public Stream AsStream()
        {
            Stream informationStream = this.InformationAsStream();
            if (informationStream == null)
            {
                return new MemoryStream(this.Header.AsBytes());
            }

            return new ByteHeaderStream(informationStream, this.Header.AsBytes());
        }

        /// <summary>
        /// Creates a stream representation of the information field.
        /// Can be overridden in subclasses to append information.
        /// </summary>
        /// <returns>The information stream.</returns>
        protected virtual Stream InformationAsStream() => this.Information?.AsStream();

        public static SnepMessage Parse(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return Parse(bytes, bytes.Length);
        }

        public static SnepMessage Parse(byte[] bytes, int length)
        {
            throw new NotImplementedException();
        }

        public static SnepMessage FromHeader(SnepHeader header)
        {
            throw new NotImplementedException();
        }

        public static SnepMessage FromNdef(SnepHeader header, NdefMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
