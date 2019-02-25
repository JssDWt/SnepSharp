using System.IO;

namespace SnepSharp.Ndef
{
    /// <summary>
    /// Represents an NDEF message.
    /// </summary>
    public interface INdefMessage
    {
        /// <summary>
        /// Gets the length of the byte encoded message in bytes.
        /// </summary>
        /// <value>The length of the message in bytes.</value>
        int Length { get; }

        /// <summary>
        /// Creates a <see cref="Stream"/> representation of the message.
        /// </summary>
        /// <returns>The message stream.</returns>
        Stream AsStream();
    }
}
