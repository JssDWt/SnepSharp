namespace SnepSharp.Snep
{
    public class DefaultSnepServer : SnepServer
    {


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
