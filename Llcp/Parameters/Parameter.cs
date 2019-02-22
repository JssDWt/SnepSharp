namespace Llcp
{

    internal abstract class Parameter
    {
        public ParameterType Type { get; }
        public byte Length { get; }
        private readonly byte[] value;

        protected Parameter(ParameterType type, byte[] value)
        {
            this.Type = type;
            this.value = value;
            this.Length = (byte)(this.value?.Length ?? 0);
        }

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
