namespace SnepSharp.Llcp.Parameters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of parameters.
    /// </summary>
    internal class ParameterList : List<Parameter>
    {
        /// <summary>
        /// Creates a byte representation of the parameter list.
        /// </summary>
        /// <returns>The bytes.</returns>
        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the bytes to create an instance of <see cref="ParameterList"/>.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="data">Parsed list.</param>
        public static ParameterList FromBytes(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
