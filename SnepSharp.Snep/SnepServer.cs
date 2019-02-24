﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SnepSharp.Llcp;
using SnepSharp.Snep.Messages;

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
            /// Callback method invoked when a snep client requests a resource 
            /// from the server.
            /// </summary>
            NdefMessage OnGet(NdefMessage message);

            /// <summary>
            /// Callback method invoked when a snep client puts a resource to 
            /// the server.
            /// </summary>
            /// <remarks>If the callback method returns successfully, it is 
            /// assumed the message was successfully processed by the 
            /// application. If an exception is thrown, it is assumed the 
            /// request was a bad request.</remarks>
            void OnPut(NdefMessage message);
        }

        /// <summary>
        /// The Service Access Point address of a default snep server.
        /// </summary>
        public const int DefaultSapAddress = 4;

        /// <summary>
        /// The service name of the default snep server.
        /// </summary>
        public const string DefaultServiceName = "urn:nfc:sn:snep";

        /// <summary>
        /// Minimum amount of bytes the default snep server should accept.
        /// </summary>
        public const int DefaultMimimumPutAcceptSize = 1024;

        /// <summary>
        /// Padlock object for synchronization.
        /// </summary>
        private object padlock = new object();

        /// <summary>
        /// Value indicating whether the current object has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The callback handler.
        /// </summary>
        private ICallBack callback;

        /// <summary>
        /// The maximum receive buffer size.
        /// </summary>
        private int maxReceiveBufferSize =
            Constants.DefaultMaxReceiveBufferSize;

        /// <summary>
        /// The maximum allowed request size for PUT requests.
        /// </summary>
        private int maxRequestSize = Constants.DefaultMaxResponseSize;

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
        /// The server socket.
        /// </summary>
        private LlcpServerSocket serverSocket;

        /// <summary>
        /// Tasks for active client connections.
        /// </summary>
        private List<Task> clientTasks;

        /// <summary>
        /// Gets the Service Access Point address for the current 
        /// <see cref="SnepServer"/>.
        /// </summary>
        /// <value>The Service Access Point address.</value>
        public int SapAddress { get; }

        /// <summary>
        /// Gets the servicename of the snep server.
        /// </summary>
        /// <value>The servicename.</value>
        public string ServiceName { get; }

        /// <summary>
        /// Gets or sets the maximum receive buffer size.
        /// </summary>
        /// <remarks>If a PUT request request exceeds the receive
        /// buffer size, the data is fetched from the snep client while the
        /// <see cref="NdefMessage"/> is read. Closing the 
        /// <see cref="SnepServer"/> will also close the underlying 
        /// <see cref="Stream"/> in the <see cref="NdefMessage"/>. So do not 
        /// close the <see cref="SnepServer"/> until the message was read.
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
        public int MaxRequestSize
        {
            get => this.maxRequestSize;
            set
            {
                if (value < Constants.SnepHeaderLength)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(MaxReceiveBufferSize),
                        $"Must be greater than {Constants.SnepHeaderLength}.");
                }

                if (this.ServiceName == DefaultServiceName
                    && this.SapAddress == DefaultSapAddress
                    && value < DefaultMimimumPutAcceptSize)
                {
                    throw new ArgumentException("The default snep server should "
                        + $"accept at least {DefaultMimimumPutAcceptSize} bytes "
                        + "in a PUT request.");
                }

                this.maxRequestSize = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:SnepSharp.Snep.SnepServer"/> class.
        /// </summary>
        /// <param name="callback">Callback implementation.</param>
        /// <param name="sapAddress">Service Access Point address for the snep 
        /// server.</param>
        /// <param name="serviceName">Snep server service name.</param>
        public SnepServer(ICallBack callback, 
            int sapAddress = DefaultSapAddress,
            string serviceName = DefaultServiceName)
        {
            this.SapAddress = sapAddress;
            this.ServiceName = serviceName;
            this.callback = callback ?? throw new ArgumentNullException(
                nameof(callback));
        }

        /// <summary>
        /// Start the <see cref="SnepServer"/>, listening for calls from snep 
        /// clients.
        /// </summary>
        public void Start()
        {
            this.ThrowIfDisposed();
            if (isRunning) return;

            this.cancel = new CancellationTokenSource();
            this.serverTask = Task.Factory.StartNew(
                () => RunServer(cancel.Token), 
                cancel.Token, 
                TaskCreationOptions.LongRunning, 
                TaskScheduler.Current);
            this.isRunning = true;
        }

        /// <summary>
        /// Stop the snep server. Waiting for any unfinished business to 
        /// complete.
        /// </summary>
        /// <returns>The stop.</returns>
        public async Task Stop()
        {
            this.ThrowIfDisposed();
            if (!isRunning) return;

            try
            {
                this.cancel.Cancel();
                await Task.WhenAll(this.clientTasks);
                await this.serverTask;
            }
            finally
            {
                lock (this.padlock)
                {
                    this.cancel.Dispose();
                    this.cancel = null;
                    this.clientTasks = null;
                    this.serverTask = null;
                    this.isRunning = false;
                }
            }
        }

        private void RunServer(CancellationToken token)
        {
            // While loop to handle exceptions
            while (!token.IsCancellationRequested)
            {
                try
                {
                    lock (this.padlock)
                    {
                        if (this.serverSocket == null)
                        {
                            this.serverSocket = LogicalLinkControl.GetInstance()
                                .CreateLlcpServerSocket();
                        }
                    }

                    while (!token.IsCancellationRequested)
                    {
                        var clientSocket = serverSocket.Accept(token);
                        var clientTask = Task.Factory.StartNew(
                            () => RunClientConnection(clientSocket, token),
                            token,
                            TaskCreationOptions.LongRunning,
                            TaskScheduler.Current);
                        lock (this.padlock)
                        {
                            this.clientTasks.Add(clientTask);
                        }
                    }


                }
                catch
                {
                    // TODO: log
                    // squelch, running the server should never throw
                }
                finally
                {
                    lock (this.padlock)
                    {
                        if (this.serverSocket != null)
                        {
                            this.serverSocket.Close();
                            this.serverSocket = null;
                        }
                    }
                }
            }
        }

        private void RunClientConnection(
            LlcpSocket socket, 
            CancellationToken token)
        {
            var messenger = new SnepMessenger(
                false, 
                socket, 
                this.maxReceiveBufferSize, 
                this.maxRequestSize);

            bool close = false;
            while (!close && !token.IsCancellationRequested)
            {
                try
                {
                    SnepRequest request = null;

                    try
                    {
                        request = (SnepRequest)messenger.GetMessage();
                    }
                    catch (SnepException ex)
                    {
                        try
                        {
                            messenger.SendMessage(new SnepBadRequestResponse());
                        }
                        catch
                        {
                            // squelch
                            // TODO: log
                        }

                        // TODO: log
                        continue;
                    }

                    if (request.Header.Version != Constants.DefaultSnepVersion)
                    {
                        messenger.SendMessage(
                            new SnepUnsupportedVersionResponse());
                        continue;
                    }

                    SnepMessage response = null;
                    switch (request.Request)
                    {
                        case SnepRequestCode.Get:
                            NdefMessage appMessage = null;

                            try
                            {
                                lock (this.padlock)
                                {
                                    appMessage = this.callback.OnGet(
                                        request.Information);
                                }

                                if (appMessage == null)
                                {
                                    response = new SnepNotFoundResponse();
                                }
                                else
                                {
                                    response = new SnepSuccessResponse(
                                        appMessage);
                                }
                            }
                            catch (Exception ex)
                            {
                                response = new SnepBadRequestResponse();
                            }

                            break;
                        case SnepRequestCode.Put:
                            try
                            {
                                lock (this.padlock)
                                {
                                    this.callback.OnPut(request.Information);
                                }

                                response = new SnepSuccessResponse(null);
                            }
                            catch (Exception ex)
                            {
                                response = new SnepBadRequestResponse();
                            }

                            break;
                        default:
                            throw new SnepException(
                                "Unidentified request message code '" +
                                $"{request.Request.ToString()}'.");
                    }

                    messenger.SendMessage(response);
                }
                catch (Exception ex)
                {
                    // TODO: log.
                }
            }
        }

        /// <summary>
        /// Releases all resources used by the 
        /// <see cref="T:SnepSharp.Snep.SnepServer"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the 
        /// <see cref="T:SnepSharp.Snep.SnepServer"/>. The <see cref="Dispose"/> 
        /// method leaves the <see cref="T:SnepSharp.Snep.SnepServer"/> in an 
        /// unusable state. After calling <see cref="Dispose"/>, you must 
        /// release all references to the 
        /// <see cref="T:SnepSharp.Snep.SnepServer"/> so the garbage collector 
        /// can reclaim the memory that the 
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
