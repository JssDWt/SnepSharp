namespace SnepSharp.Snep
{
    /// <summary>
    /// Class containing constants to use thoughout the library.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The default snep version.
        /// </summary>
        public const SnepVersion DefaultSnepVersion = SnepVersion.V10;

        /// <summary>
        /// The length of the snep header.
        /// </summary>
        public const int SnepHeaderLength = 6;

        /// <summary>
        /// The default maximum receive buffer size.
        /// Defaults to 16 KiB.
        /// </summary>
        public const int DefaultMaxReceiveBufferSize = 1024 * 16;

        /// <summary>
        /// The default maximum response size. Defaults to 1 GiB.
        /// </summary>
        public const int DefaultMaxResponseSize = 1024 * 1024 * 1024;
    }
}
