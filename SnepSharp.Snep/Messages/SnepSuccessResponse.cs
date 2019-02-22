﻿namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Snep success response. As a response to a GET or PUT request.
    /// Response to a PUT request should have no content. Response to a GET request SHALL have content.
    /// </summary>
    internal class SnepSuccessResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepSuccessResponse"/> class.
        /// </summary>
        /// <param name="content">Content for the GET request.</param>
        public SnepSuccessResponse(NdefMessage content)
            : this(Constants.DefaultSnepVersion, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.SnepSuccessResponse"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        /// <param name="content">Content for the GET request.</param>
        public SnepSuccessResponse(SnepVersion version, NdefMessage content)
            : base(version, SnepResponseCode.Success, content)
        {

        }
    }
}
