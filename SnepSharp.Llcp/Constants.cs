using System.Text;

namespace SnepSharp.Llcp
{
    /// <summary>
    /// Constants for the Logical Link Control Protocol.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The maximum information units (octets) to send in a PDU.
        /// </summary>
        public const int MaximumInformationUnit = 128;

        /// <summary>
        /// The default timeout in milliseconds.
        /// </summary>
        public const int DefaultTimeout = 100;

        /// <summary>
        /// The default receive window size.
        /// </summary>
        public const int DefaultReceiveWindowSize = 1;

        /// <summary>
        /// The default encoding to encode strings with.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    }
}
