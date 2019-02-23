namespace SnepSharp.Snep
{
    using System;
    using System.IO;
    using SnepSharp.Snep.Messages;

    /// <summary>
    /// Snep client to interchange <see cref="NdefMessage"/> with a SNEP server.
    /// </summary>
    public class SnepClient
    {
        // TODO: Determine fragment size. (depends on underlying protocol.)
        int fragmentSize = 256;

        /// <summary>
        /// Send the specified message to the server.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <exception cref="SnepException">Thrown when the message could not be transmitted.</exception>
        public void Send(NdefMessage message)
        {
            var snepMessage = new SnepRequest(SnepVersion.V10, SnepRequestCode.Put, message);

        }

        private void SendContinue()
        {

        }

        public NdefMessage Get(NdefMessage request, int maxResponseLength = int.MaxValue)
        {
            throw new NotImplementedException();
        }





    }
}
