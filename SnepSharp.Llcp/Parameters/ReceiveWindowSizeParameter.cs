namespace SnepSharp.Llcp.Parameters
{
    using System;

    /// <summary>
    /// The receive window size (RW) is a data link connection parameter that 
    /// MAY be transmitted with a CONNECT or a CC PDU and applies to the sender 
    /// of the CONNECT or CC PDU.
    /// </summary>
    internal class ReceiveWindowSizeParameter : Parameter
    {
        /// <summary>
        /// Gets the receive window size.
        /// </summary>
        /// <remarks>A receive window size of zero indicates that the local LLC 
        /// will not accept I PDUs on that data link connection. A receive 
        /// window size of one indicates that the local LLC will acknowledge 
        /// every I PDU before accepting additional I PDUs.</remarks>
        /// <value>The size of the window.</value>
        public int WindowSize { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SnepSharp.Llcp.Parameters.ReceiveWindowSizeParameter"/> class.
        /// </summary>
        /// <param name="windowSize">Window size.</param>
        public ReceiveWindowSizeParameter(int windowSize)
            : base(ParameterType.ReceiveWindowSize, new byte[] { (byte)windowSize })
        {
            if (windowSize < 0 || windowSize > 15)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(windowSize), 
                    "windowSize must be between 0 and 15 inclusive.");
            }

            this.WindowSize = windowSize;
        }
    }
}
