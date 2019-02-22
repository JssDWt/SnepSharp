namespace SnepSharp.Llcp.Parameters
{
    using System;

    /// <summary>
    /// The service name (SN) parameter MAY be transmitted with a CONNECT PDU to
    /// the well- known destination service access point address 01h and SHALL 
    /// then indicate that the sending LLC intends to establish a data link 
    /// connection with the named service registered in the remote service 
    /// environment.
    /// </summary>
    internal class ServiceNameParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SnepSharp.Llcp.Parameters.ServiceNameParameter"/> class.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public ServiceNameParameter(string serviceName)
            : base(ParameterType.ServiceName, AsBytes(serviceName))
        {
        }

        /// <summary>
        /// Creates a byte representation of the servicename.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="serviceName">Service name.</param>
        private static byte[] AsBytes(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException(
                    "serviceName cannot be null or empty", 
                    nameof(serviceName));
            }

            return Constants.DefaultEncoding.GetBytes(serviceName);
        }
    }
}
