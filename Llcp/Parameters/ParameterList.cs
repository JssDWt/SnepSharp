namespace Llcp
{
    using System;
    using System.Collections.Generic;

    internal class ParameterList : List<Parameter>
    {
        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        public static ParameterList FromBytes(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
