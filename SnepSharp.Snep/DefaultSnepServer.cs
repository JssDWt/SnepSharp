namespace SnepSharp.Snep
{
    public class DefaultSnepServer : SnepServer
    {
        /// <summary>
        /// The Service Access Point address of a default snep server.
        /// </summary>
        public const int DefaultSAPAddress = 4;

        /// <summary>
        /// The service name of the default snep server.
        /// </summary>
        public const string DefaultServiceName = "urn:nfc:sn:snep";

        /// <summary>
        /// Minimum amount of bytes the default snep server should accept.
        /// </summary>
        public const int MimimumPutAcceptSize = 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.DefaultSnepServer"/> class.
        /// </summary>
        public DefaultSnepServer()
            : base(DefaultSAPAddress, DefaultServiceName)
        {
        }

        // TODO: Accept put request of at least MimimumPutAcceptSize.
        // TODO: Reject GET requests with notimplemented.
    }
}
