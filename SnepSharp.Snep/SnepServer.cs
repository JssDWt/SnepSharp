namespace SnepSharp.Snep
{
    /// <summary>
    /// Snep client to interchange <see cref="NdefMessage"/> with a SNEP client.
    /// </summary>
    public class SnepServer
    {
        /// <summary>
        /// Gets the Service Access Point address for the current <see cref="SnepServer"/>.
        /// </summary>
        /// <value>The Service Access Point address.</value>
        public int SAPAddress { get; }

        /// <summary>
        /// Gets the servicename of the snep server.
        /// </summary>
        /// <value>The servicename.</value>
        public string ServiceName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepServer"/> class,
        /// using the specified address and service name.
        /// </summary>
        /// <param name="sapAddress">Service Access Point address for the snep server.</param>
        /// <param name="serviceName">Snep server service name.</param>
        public SnepServer(int sapAddress, string serviceName)
        {
            this.SAPAddress = sapAddress;
            this.ServiceName = serviceName;
        }


        public void ReceiveFragment(byte[] fragment, bool isFirstFragment)
        {

        }


    }
}
