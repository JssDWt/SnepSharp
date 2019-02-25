using System;
using System.IO;

namespace SnepSharp.Ndef
{
    public interface INdefParser
    {
        /// <summary>
        /// Gets the maximum buffer size for <see cref="INdefMessage"/> 
        /// creation.
        /// </summary>
        /// <remarks>
        /// For received message sizes below the <see cref="MaxBufferSize"/>, 
        /// <see cref="ParseMessage(byte[], int, int)"/> will be called. Above 
        /// that, <see cref="ParseMessage(Stream)"/> will be called.</remarks>
        /// <value>The maximum buffer size.</value>
        int MaxBufferSize { get; }

        /// <summary>
        /// Parses an <see cref="INdefMessage"/> from the specified 
        /// <see cref="Stream"/>. 
        /// </summary>
        /// <remarks>The <see cref="Stream"/> will be readonly, and may be 
        /// empty. If the <see cref="Stream"/> is empty, <c>null</c> should be 
        /// returned.</remarks>
        /// <returns>The <see cref="Stream"/> containing the message.</returns>
        /// <param name="stream">Message stream, or null is the stream is empty.
        /// </param>
        INdefMessage ParseMessage(Stream stream);

        /// <summary>
        /// Creates a <see cref="INdefMessage"/> from the specified 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <remarks>If the <see cref="byte[]"/> is empty in the specified 
        /// interval, <c>null</c> should be returned.</remarks>
        /// <returns>The message, or null if the interval is empty.</returns>
        /// <param name="bytes">Message bytes.</param>
        INdefMessage ParseMessage(byte[] bytes, int offset, int count);
    }
}
