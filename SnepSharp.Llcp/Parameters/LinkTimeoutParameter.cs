namespace SnepSharp.Llcp.Parameters
{
    using System;

    /// <summary>
    /// The link timeout (LTO) parameter is a configuration parameter that 
    /// specifies the local link timeout interval guarantee.
    /// </summary>
    internal class LinkTimeoutParameter : Parameter
    {
        /// <summary>
        /// Gets the timeout in milliseconds.
        /// </summary>
        /// <value>The timeout milliseconds.</value>
        public int TimeoutMilliseconds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SnepSharp.Llcp.Parameters.LinkTimeoutParameter"/> class.
        /// </summary>
        /// <param name="milliseconds">Link timeout in milliseconds.</param>
        public LinkTimeoutParameter(int milliseconds)
            : base(ParameterType.LinkTimeout, AsBytes(milliseconds))
        {

            this.TimeoutMilliseconds = DetermineMilliseconds(milliseconds);
        }

        /// <summary>
        /// Creates a byte representation of the information field.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="milliseconds">Link timeout in milliseconds.</param>
        /// <exception cref="ArgumentException"></exception>
        private static byte[] AsBytes(int milliseconds)
        {
            int determined = DetermineMilliseconds(milliseconds);
            int divided = determined / 10;
            if (divided > 0xFF)
            {
                throw new ArgumentException(
                    $"Link timeout cannot be larger than {0xFF * 10} milliseconds.", 
                    nameof(milliseconds));
            }

            return new byte[] { (byte)divided };
        }

        /// <summary>
        /// Determines the timeout milliseconds based on the given value.
        /// </summary>
        /// <returns>The milliseconds.</returns>
        /// <param name="milliseconds">The actual timeout.</param>
        private static int DetermineMilliseconds(int milliseconds)
        {
            if (milliseconds < 1)
            {
                return Constants.DefaultTimeout;
            }

            return milliseconds;
        }
    }
}
