namespace SnepSharp.Snep.Messages
{
    using System;

    /// <summary>
    /// Snep put request. To put a resource to the server.
    /// </summary>
    internal class SnepPutRequest : SnepRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepPutRequest"/> class.
        /// </summary>
        /// <param name="content">Content to put to the server.</param>
        public SnepPutRequest(NdefMessage content)
            : this(Constants.DefaultSnepVersion, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepPutRequest"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        /// <param name="content">Content to put to the server.</param>
        public SnepPutRequest(SnepVersion version, NdefMessage content)
            : base(version, SnepRequestCode.Put, content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content), "Snep put requests MUST have information");
            }
        }
    }
}
