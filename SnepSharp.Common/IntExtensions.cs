namespace SnepSharp.Common
{
    using System;

    /// <summary>
    /// Class providing useful extension methods for <see cref="int"/>.
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Creates a byte[] representation of the specified <see cref="int"/>,
        /// with the most significant digits first.
        /// </summary>
        /// <returns>The byte representation of the <see cref="int"/>.</returns>
        /// <param name="intValue"><see cref="int"/> value.</param>
        public static byte[] ToByteArray(this int intValue)
        {
            byte[] intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }
    }
}
