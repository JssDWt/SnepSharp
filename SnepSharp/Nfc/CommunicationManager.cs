//
//  CommunicationManager.cs
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

namespace SnepSharp.Nfc
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using SnepSharp.Llcp;
    using SnepSharp.Mac;

    /// <summary>
    /// Manages communication between NFC devices.
    /// </summary>
    public class CommunicationManager : IDisposable
    {
        /// <summary>
        /// Roles to iterate over.
        /// </summary>
        private static readonly Role[] Roles = { Role.Target, Role.Initiator };

        /// <summary>
        /// Value indicating whether the <see cref="CommunicationManager"/>
        /// is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The LLCP options.
        /// </summary>
        private LlcpOptions options;

        /// <summary>
        /// The nfc device.
        /// </summary>
        private INfcDevice device;

        /// <summary>
        /// The on startup function.
        /// </summary>
        private LlcHandler onStartup = (llc) => false;

        /// <summary>
        /// The on connect function.
        /// </summary>
        private LlcHandler onConnect = (llc) => false;

        /// <summary>
        /// The on release function.
        /// </summary>
        private LlcHandler onRelease = (llc) => false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationManager"/> 
        /// class.
        /// </summary>
        /// <param name="device">Device.</param>
        public CommunicationManager(INfcDevice device)
            : this(device, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationManager"/> 
        /// class.
        /// </summary>
        /// <param name="device">Device.</param>
        public CommunicationManager(INfcDevice device, LlcpOptions options)
        {
            this.device = device 
                ?? throw new ArgumentNullException(nameof(device));

            this.options = options ?? new LlcpOptions();
        }

        /// <summary>
        /// This function is called before any attempt to establish peer to peer
        /// communication.
        /// </summary>
        /// <remarks>The llc argument provides the 
        /// <see cref="LogicalLinkControl"/> that may be used to allocate and 
        /// bind listen sockets for local services. The function should return
        /// <c>true</c> if LLCP should be used.</remarks>
        /// <value>The OnStartup function.</value>
        public LlcHandler OnStartup
        {
            get => this.onStartup;
            set => this.onStartup = value ?? ((llc) => false);
        }

        /// <summary>
        /// This function is called when peer to peer communication is 
        /// successfully established. Defaults to return <c>true</c>.
        /// </summary>
        /// <remarks>The llc argument provides the now activated 
        /// <see cref="LogicalLinkControl"/> ready for allocation of client 
        /// communication sockets and data exchange in separate work threads.
        /// The function should return <c>true</c> more or less immediately, 
        /// unless it wishes to handle the logical link control run loop by 
        /// itself and anytime later return a false value.</remarks>
        /// <value>The OnConnect function.</value>
        public LlcHandler OnConnect
        {
            get => this.onConnect;
            set => this.onConnect = value ?? ((llc) => true);
        }

        /// <summary>
        /// This function is called when the symmetry loop was run (the 
        /// <see cref="OnConnect"/> function returned <c>true</c>) and 
        /// determined that communication with the remote peer has become 
        /// impossible, or when the <see cref="CancellationToken"/> requested 
        /// cancellation. The <see cref="LogicalLinkControl"/> may be used for 
        /// cleanup actions but not for communication. Defaults to return 
        /// <c>true</c>.
        /// </summary>
        /// <value>The OnRelease function.</value>
        public LlcHandler OnRelease
        {
            get => this.onRelease;
            set => this.onRelease = value ?? ((llc) => true);
        }

        /// <summary>
        /// Gets the role of the LLCP instance.
        /// </summary>
        /// <value>The role.</value>
        public Role Role { get; set; } = Role.Both;

        /// <summary>
        /// Connect with a target or initiator. 
        /// </summary>
        /// <remarks>Connects until </remarks>
        /// <returns>An awaitable <see cref="Task{Boolean}"/> that completes when a 
        /// single activation and deactivation has completed, or the oparation 
        /// is cancelled. If a successful connection was made, it returns the
        /// value returned by the <see cref="OnRelease"/> function.</returns>
        public Task<bool> Connect(CancellationToken token) 
            => Task<bool>.Factory.StartNew(() => 
            {
                var llc = new LogicalLinkControl(this.options);
                bool shouldStart = this.OnStartup(llc);

                while (shouldStart 
                    && !this.isDisposed 
                    && !token.IsCancellationRequested)
                {
                    foreach (Role role in Roles)
                    {
                        if (this.Role.HasFlag(role))
                        {
                            var mac = GetMacMapping(role);
                            if (llc.Activate(mac))
                            {
                                if (this.OnConnect(llc))
                                {
                                    llc.Run(token);
                                    return this.OnRelease(llc);
                                }

                                return false;
                            }
                        }
                    }
                }

                // May we end up here, return false.
                return false;
            },
            token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Current);

        /// <summary>
        /// Gets the mac mapping for the specified role.
        /// </summary>
        /// <returns>The mac mapping.</returns>
        /// <param name="role">Role.</param>
        private MacMapping GetMacMapping(Role role)
        {
            switch (role)
            {
                case Role.Target:
                    return new Target(this);
                case Role.Initiator:
                    return new Initiator(this);
                default:
                    throw new InvalidOperationException(
                        "Could not define mac mapping for role.");
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="CommunicationManager"/> 
        /// object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose()"/> when you are finished using 
        /// the <see cref="CommunicationManager"/>. The <see cref="Dispose()"/> 
        /// method leaves the <see cref="CommunicationManager"/> in an unusable 
        /// state. After calling <see cref="Dispose()"/>, you must release all 
        /// references to the <see cref="CommunicationManager"/> so the garbage 
        /// collector can reclaim the memory that the 
        /// <see cref="CommunicationManager"/> was occupying.</remarks>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases all managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.device != null)
                {
                    this.device.Dispose();
                    this.device = null;
                }

                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Throws if disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(CommunicationManager));
            }
        }
    }
}
