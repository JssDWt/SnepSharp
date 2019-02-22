namespace Llcp
{
    /// <summary>
    /// Constants for the Logical Link Control Protocol.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The maximum information units (octets) to send in a PDU.
        /// </summary>
        public const int MaximumInformationUnit = 0;

        public const LlcpVersion DefaultLlcpVersion = LlcpVersion.V10;
    }
}
