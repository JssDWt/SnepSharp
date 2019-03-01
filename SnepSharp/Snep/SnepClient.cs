//
//  SnepClient.cs
//
//  Author:
//       Jesse de Wit <witdejesse@hotmail.com>
//
//  Copyright (c) 2019 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace SnepSharp.Snep
{
    using System;
    using System.IO;
    using SnepSharp.Llcp;
    using SnepSharp.Snep.Messages;
    using SnepSharp.Ndef;

    /// <summary>
    /// Snep client to interchange <see cref="INdefMessage"/> with a SNEP 
    /// server.
    /// </summary>
    public class SnepClient : IDisposable
    {
        /// <summary>
        /// The messenger to send and receive snep messages over.
        /// </summary>
        private SnepMessenger messenger;

        /// <summary>
        /// The ndef parser.
        /// </summary>
        private readonly INdefParser ndefParser;

        /// <summary>
        /// The logical link control.
        /// </summary>
        private LogicalLinkControl llc;

        /// <summary>
        /// The maximum allowed response size for GET requests.
        /// </summary>
        private int maxResponseSize = Constants.DefaultMaxResponseSize;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:SnepSharp.Snep.SnepClient"/> class, that connects to
        /// the default snep server.
        /// </summary>
        public SnepClient(INdefParser ndefParser, LogicalLinkControl llc)
            : this(SnepServer.DefaultServiceName, ndefParser, llc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:SnepSharp.Snep.SnepClient"/> class, that connects to 
        /// the specified service name.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public SnepClient(
            string serviceName, 
            INdefParser ndefParser,
            LogicalLinkControl llc)
        {
            this.llc = llc ?? throw new ArgumentNullException(nameof(llc));

            this.ServiceName = serviceName
                ?? throw new ArgumentNullException(nameof(serviceName));

            if (serviceName == SnepServer.DefaultServiceName)
            {
                this.SapAddress = SnepServer.DefaultSapAddress;
            }

            this.ndefParser = ndefParser
                ?? throw new ArgumentNullException(nameof(ndefParser));
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
                        nameof(MaxResponseSize),
                        $"Must be greater than {Constants.SnepHeaderLength}.");
                }

                this.maxResponseSize = value;
            }
        }

        /// <summary>
        /// Puts the specified message to the server.
        /// </summary>
        /// <param name="message">Message to put.</param>
        /// <exception cref="SnepException">Thrown if any protocol related issue
        /// is encountered.</exception>
        public void Put(INdefMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.Connect();
            var snepMessage = new SnepPutRequest(message);
            this.messenger.SendMessage(snepMessage);

            var response = (SnepResponse)this.messenger.GetMessage();
            switch (response.Response)
            {
                case SnepResponseCode.Success:
                    break;
                case SnepResponseCode.UnsupportedVersion:
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
        /// <see cref="INdefParser.MaxBufferSize"/>, the data is fetched from 
        /// the remote server while the <see cref="INdefMessage"/> is read. 
        /// Closing the <see cref="SnepClient"/> will also close the underlying 
        /// <see cref="Stream"/> in the <see cref="INdefMessage"/>. So do not 
        /// close the <see cref="SnepClient"/> until the message was read.
        /// </remarks>
        public INdefMessage Get(INdefMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            this.Connect();
            var snepMessage = new SnepGetRequest(request, this.MaxResponseSize);
            this.messenger.SendMessage(snepMessage);
            var response = (SnepResponse)this.messenger.GetMessage();

            INdefMessage result;
            switch (response.Response)
            {
                case SnepResponseCode.Success:
                    result = response.Information;
                    break;
                case SnepResponseCode.NotFound:
                    result = null;
                    break;
                case SnepResponseCode.ExcessData:
                    throw new SnepException(
                        "Getting the specified message would result in " +
                        "exceeding the MaxResponseSize.");
                case SnepResponseCode.BadRequest:
                    throw new SnepException(
                        "Server indicates the request has malformed syntax.");
                case SnepResponseCode.NotImplemented:
                    throw new SnepException(
                        "Server does not implement the required functionality " 
                        + "to process the request.");
                default:
                    throw new SnepException(
                        "Received unexpected response code from server: '" +
                        $"{response.Response.ToString()}'.");
            }

            return result;
        }

        /// <summary>
        /// Connect the client to the snep server.
        /// </summary>
        private void Connect()
        {
            if (this.IsConnected) return;

            var socket = this.llc.CreateSocket();
            if (this.SapAddress == -1)
            {
                socket.Connect(this.ServiceName);
            }
            else
            {
                socket.Bind((LinkAddress)this.SapAddress);
            }

            this.messenger = new SnepMessenger(
                true,
                socket,
                this.ndefParser,
                this.MaxResponseSize);
            this.IsConnected = true;
        }

        /// <summary>
        /// Closes the connection to the snep server.
        /// </summary>
        public void Close()
        {
            if (this.messenger != null)
            {
                this.messenger.Close();
                this.messenger = null;
            }

            // Release the parser, in case it is IDisposable.
            this.IsConnected = false;
        }

        /// <summary>
        /// Releases all managed and unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Closes the underlying socket.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
                this.llc = null;
            }
        }
    }
}
