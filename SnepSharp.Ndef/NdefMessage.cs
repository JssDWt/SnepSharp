namespace SnepSharp.Snep
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Ndef message.
    /// </summary>
    public class NdefMessage
    {
        /// <summary>
        /// Gets the byte length of the message.
        /// </summary>
        /// <value>The length.</value>
        public int Length { get; }

        public IEnumerable<NdefRecord> Records { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.NdefMessage"/> class.
        /// </summary>
        public NdefMessage(params NdefRecord[] records)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.NdefMessage"/> class.
        /// </summary>
        public NdefMessage(IEnumerable<NdefRecord> records)
        {
        }

        /// <summary>
        /// Returns a <see cref="Stream"/> representation of the message.
        /// </summary>
        /// <returns>The stream representation of the message.</returns>
        public Stream AsStream()
        {
            throw new NotImplementedException();
        }

        public static NdefMessage FromStream(Stream message)
        {

        }
    }
}
