namespace SnepSharp.Snep
{
    using System;
    using System.IO;
    using SnepSharp.Llcp;
    using SnepSharp.Snep.Messages;

    /// <summary>
    /// Snep client to interchange <see cref="NdefMessage"/> with a SNEP server.
    /// </summary>
    public class SnepClient : IDisposable
    {
        /// <summary>
        /// The messenger to send and receive snep messages over.
        /// </summary>
        private SnepMessenger messenger;

        /// <summary>
        /// The maximum receive buffer size.
        /// </summary>
        private int maxReceiveBufferSize =
            Constants.DefaultMaxReceiveBufferSize;

        /// <summary>
        /// The maximum allowed response size for GET requests.
        /// </summary>
        private int maxResponseSize = Constants.DefaultMaxResponseSize;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:SnepSharp.Snep.SnepClient"/> class, that connects to
        /// the default snep server.
        /// </summary>
        public SnepClient()
        {
            this.ServiceName = SnepServer.DefaultServiceName;
            this.SapAddress = SnepServer.DefaultSapAddress;
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:SnepSharp.Snep.SnepClient"/> class, that connects to 
        /// the specified service name.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public SnepClient(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException(
                    "Cannot be null or empty", 
                    nameof(serviceName));
            }

            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the service name of the snep server.
        /// </summary>
        /// <value>The service name of the snep server.</value>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the SAP Address of the snep server.
        /// </summary>
        /// <value>The SAP Address of the snep server.</value>
        public int SapAddress { get; } = -1;

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="T:SnepSharp.Snep.SnepClient"/> is connected to the snep
        /// server.
        /// </summary>
        /// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets or sets the maximum receive buffer size.
        /// </summary>
        /// <remarks>If a response to a GET request exceeds the receive
        /// buffer size, the data is fetched from the remote server while the
        /// <see cref="NdefMessage"/> is read. Closing the 
        /// <see cref="SnepClient"/> will also close the underlying 
        /// <see cref="Stream"/> in the <see cref="NdefMessage"/>. So do not 
        /// close the <see cref="SnepClient"/> until the message was read.
        /// </remarks>
        /// <value>The size of the max receive buffer.</value>
        public int MaxReceiveBufferSize 
        {
            get => this.maxReceiveBufferSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(MaxReceiveBufferSize),
                        "Must be positive.");
                }

                this.maxReceiveBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed response size for GET requests.
        /// </summary>
        /// <value>The maximum response size.</value>
        public int MaxResponseSize
        {
            get => this.maxResponseSize;
            set
            {
                if (value < Constants.SnepHeaderLength)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(MaxReceiveBufferSize),
                        $"Must be greater than {Constants.SnepHeaderLength}.");
                }

                this.maxResponseSize = value;
            }
        }

        /// <summary>
        /// Connect the client to the snep server.
        /// </summary>
        public void Connect()
        {
            var socket = LogicalLinkControl.GetInstance().CreateLlcpSocket();
            if (this.SapAddress == -1)
            {
                socket.ConnectToService(this.ServiceName);
            }
            else
            {
                socket.ConnectToSap(this.SapAddress);
            }

            this.messenger = new SnepMessenger(true, socket, this.maxReceiveBufferSize);
            this.IsConnected = true;
        }

        /// <summary>
        /// Puts the specified message to the server.
        /// </summary>
        /// <param name="message">Message to put.</param>
        /// <exception cref="SnepException">Thrown if any protocol related issue
        /// is encountered.</exception>
        public void Put(NdefMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var snepMessage = new SnepPutRequest(message);
            this.messenger.SendMessage(snepMessage);

            var response = (SnepResponse)this.messenger.GetMessage();
            switch (response.Response)
            {
                case SnepResponseCode.Success:
                    break;
                case SnepResponseCode.UnsopportedVersion:
                    throw new SnepException(
                        "Server does not support our snep version.");
                case SnepResponseCode.BadRequest:
                    throw new SnepException(
                        "Server indicates the request has malformed syntax.");
                case SnepResponseCode.NotImplemented:
                    throw new SnepException(
                        "Server does not implement the required functionality "
                        + "to process the request.");
                default:
                    throw new SnepException("Received unexpected response code "
                        + $"from server: {response.Response.ToString()}");
            }
        }

        /// <summary>
        /// Gets a resource from the server, specified by the request.
        /// </summary>
        /// <returns>The resource from the server, or <c>null</c> if the 
        /// resource was not found.</returns>
        /// <param name="request">Get request.</param>
        /// <exception cref="T:SnepSharp.Snep.SnepException">Thrown if any 
        /// protocol related issue is encountered.</exception>
        /// <remarks>If a response to a GET request exceeds the 
        /// <see cref="MaxReceiveBufferSize"/>, the data is fetched from the 
        /// remote server while the <see cref="NdefMessage"/> is read. Closing 
        /// the <see cref="SnepClient"/> will also close the underlying 
        /// <see cref="Stream"/> in the <see cref="NdefMessage"/>. So do not 
        /// close the <see cref="SnepClient"/> until the message was read.
        /// </remarks>
        public NdefMessage Get(NdefMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var snepMessage = new SnepGetRequest(request, this.MaxResponseSize);
            this.messenger.SendMessage(snepMessage);
            var response = (SnepResponse)this.messenger.GetMessage();

            NdefMessage result;
            switch (response.Response)
            {
                case SnepResponseCode.Success:
                    result = response.Information;
                    break;
                case SnepResponseCode.NotFound:
                    result = null;
                    break;
                case SnepResponseCode.ExcessData:
                    throw new SnepException("Getting the specified message would result in exceeding the MaxResponseSize.");
                case SnepResponseCode.BadRequest:
                    throw new SnepException("Server indicates the request has malformed syntax.");
                case SnepResponseCode.NotImplemented:
                    throw new SnepException("Server does not implement the required functionality to process the request.");
                default:
                    throw new SnepException($"Received unexpected response code from server: {response.Response.ToString()}");
            }

            return result;
        }



        /// <summary>
        /// Closes the connection to the snep server.
        /// </summary>
        public void Close()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases all managed and unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Closes the underlying socket.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.messenger != null)
                {
                    this.messenger.Close();
                    this.messenger = null;
                }
            }

            this.IsConnected = false;
        }
    }
}
