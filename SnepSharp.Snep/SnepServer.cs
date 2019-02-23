using System;
using System.Threading;
using System.Threading.Tasks;

namespace SnepSharp.Snep
{
    /// <summary>
    /// Snep client to interchange <see cref="NdefMessage"/> with a SNEP client.
    /// </summary>
    public class SnepServer : IDisposable
    {
        /// <summary>
        /// Interface containing callbacks for a service hosting a snep server.
        /// </summary>
        public interface ICallBack
        {
            /// <summary>
            /// Callback method invoked when a snep client requests a resource from the server.
            /// </summary>
            NdefMessage OnGet(NdefMessage message);

            /// <summary>
            /// Callback method invoked when a snep client puts a resource to the server.
            /// </summary>
            /// <remarks>If the callback method returns successfully, it is assumed
            /// the message was successfully processed by the application.</remarks>
            void OnPut(NdefMessage message);
        }

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
        public const int DefaultMimimumPutAcceptSize = 1024;

        /// <summary>
        /// Value indicating whether the current object has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The callback handler.
        /// </summary>
        private ICallBack callback;

        /// <summary>
        /// The server task.
        /// </summary>
        private Task serverTask;

        /// <summary>
        /// The cancellation token source to stop the server.
        /// </summary>
        private CancellationTokenSource cancel;

        /// <summary>
        /// Value indicating whether the server is currently running.
        /// </summary>
        private bool isRunning;

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
        /// Gets the maximum accepted size of PUT messages in bytes.
        /// </summary>
        /// <value>The maximum size of PUT messages.</value>
        public int MaxPutAcceptSize { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SnepSharp.Snep.SnepServer"/> class.
        /// </summary>
        /// <param name="callback">Callback implementation.</param>
        /// <param name="sapAddress">Service Access Point address for the snep server.</param>
        /// <param name="serviceName">Snep server service name.</param>
        /// <param name="maxPutAcceptSize">Maximum accepted size of PUT messages.</param>
        public SnepServer(ICallBack callback, 
            int sapAddress = DefaultSAPAddress,
            string serviceName = DefaultServiceName,
            int maxPutAcceptSize = DefaultMimimumPutAcceptSize)
        {
            if (sapAddress == DefaultSAPAddress 
                && serviceName == DefaultServiceName 
                && maxPutAcceptSize < DefaultMimimumPutAcceptSize)
            {
                throw new ArgumentException(
                    $"Default snep server should accept at least {DefaultMimimumPutAcceptSize} bytes.");
            }

            this.SAPAddress = sapAddress;
            this.ServiceName = serviceName;
            this.MaxPutAcceptSize = maxPutAcceptSize;
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        /// <summary>
        /// Start the <see cref="SnepServer"/>, listening for calls from snep clients.
        /// </summary>
        public void Start()
        {
            this.ThrowIfDisposed();
            if (isRunning) return;

            this.cancel = new CancellationTokenSource();
            this.serverTask = new Task(RunServer, cancel.Token, TaskCreationOptions.LongRunning);
            this.serverTask.Start();
            this.isRunning = true;
        }

        /// <summary>
        /// Stop the snep server. Waiting for any unfinished business to complete.
        /// </summary>
        /// <returns>The stop.</returns>
        public async Task Stop()
        {
            this.ThrowIfDisposed();
            if (!isRunning) return;

            try
            {
                this.cancel.Cancel();
                await this.serverTask;
            }
            finally
            {
                this.cancel.Dispose();
                this.cancel = null;
                this.serverTask = null;
                this.isRunning = false;
            }
        }

        private void RunServer()
        {
            try
            {

            }
            catch
            {
                // squelch, running the server should never 
            }
        }

        private void ReceiveFragment(byte[] fragment, bool isFirstFragment)
        {

        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:SnepSharp.Snep.SnepServer"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:SnepSharp.Snep.SnepServer"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="T:SnepSharp.Snep.SnepServer"/> in an unusable state.
        /// After calling <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:SnepSharp.Snep.SnepServer"/> so the garbage collector can reclaim the memory that the
        /// <see cref="T:SnepSharp.Snep.SnepServer"/> was occupying.</remarks>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    if (this.cancel != null)
                    {
                        this.cancel.Dispose();
                        this.cancel = null;
                    }
                }

                this.callback = null;
                this.serverTask = null;
                isDisposed = true;
            }
        }

        /// <summary>
        /// Throws if disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(SnepServer));
            }
        }
    }
}
