namespace SnepSharp.Llcp.Parameters
{
    /// <summary>
    /// Represents an LLCP parameter.
    /// </summary>
    internal abstract class Parameter
    {
        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public ParameterType Type { get; }

        /// <summary>
        /// Gets the length of the parameter bytes.
        /// </summary>
        /// <value>The length of the parameter bytes.</value>
        public byte Length { get; }

        /// <summary>
        /// The parameter value as byte array.
        /// </summary>
        private readonly byte[] value;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SnepSharp.Llcp.Parameters.Parameter"/> class.
        /// </summary>
        /// <param name="type">Parameter type.</param>
        /// <param name="value">Parameter value.</param>
        protected Parameter(ParameterType type, byte[] value)
        {
            this.Type = type;
            this.value = value;
            this.Length = (byte)(this.value?.Length ?? 0);
        }

        /// <summary>
        /// Creates a byte representation of the parameter, to send over LLCP.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] ToByteArray()
        {
            var result = new byte[this.Length + 2];
            result[0] = (byte)this.Type;
            result[1] = this.Length;
            if (this.value != null)
            {
                this.value.CopyTo(result, 2);
            }

            return result;
        }
    }
}
