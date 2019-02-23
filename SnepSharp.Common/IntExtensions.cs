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

        public static int FromByteArray(byte[] bytes, int startIndex = 0)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (BitConverter.IsLittleEndian)
            {
                // Reverse the bytes in a copy
                var copy = new byte[]
                {
                    bytes[startIndex + 1],
                    bytes[startIndex]
                };

                return BitConverter.ToInt16(copy, 0);
            }

            return BitConverter.ToInt16(bytes, startIndex);
        }
    }
}
