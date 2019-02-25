namespace SnepSharp.Snep.Messages
{
    using System;
    using SnepSharp.Common;
    using SnepSharp.Ndef;

    /// <summary>
    /// Snep get request message. To request a resource from the snep server.
    /// </summary>
    internal class SnepGetRequest : SnepRequest
    {
        /// <summary>
        /// Gets the maximum accepted response length.
        /// </summary>
        /// <value>The maximum accepted response length.</value>
        public int MaxResponseLength { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepGetRequest"/> 
        /// class.
        /// </summary>
        /// <param name="request">The snep GET request information.</param>
        /// <param name="maxResponseLength">Maximum accepted response length.
        /// </param>
        public SnepGetRequest(INdefMessage request, int maxResponseLength)
            : this(Constants.DefaultSnepVersion, request, maxResponseLength)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepGetRequest"/> 
        /// class.
        /// </summary>
        /// <param name="version">The snep protocol version.</param>
        /// <param name="request">The snep GET request information.</param>
        /// <param name="maxResponseLength">Maximum accepted response length.
        /// </param>
        public SnepGetRequest(
            SnepVersion version, 
            INdefMessage request, 
            int maxResponseLength)
            : base(
                version, 
                SnepRequestCode.Get, 
                request, 
                maxResponseLength.ToByteArray())
        {
            if (maxResponseLength < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxResponseLength), 
                    "maxResponseLength must be positive.");
            }

            this.MaxResponseLength = maxResponseLength;
        }
    }
}
