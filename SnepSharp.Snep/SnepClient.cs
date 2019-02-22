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
            int offset = 0;
            bool isFirstFragment = true;
            bool isAcknowledged = false;
            bool fullMessageSent = false;
            using (var requestStream = snepMessage.AsStream())
            { 
                while ((isFirstFragment || isAcknowledged)
                    && !fullMessageSent)
                {
                    int currentFragmentSize = Math.Min(fragmentSize, snepMessage.MessageLength - offset);
                    SendFragment(requestStream, currentFragmentSize);
                    offset += currentFragmentSize;
                    fullMessageSent = offset != snepMessage.MessageLength;

                    if (isFirstFragment)
                    {
                        isFirstFragment = false;
                        var ackResponse = this.WaitForResponse();
                        if (ackResponse.Response == SnepResponseCode.Continue)
                        {
                            isAcknowledged = true;
                        }
                        else if (ackResponse.Response == SnepResponseCode.Success)
                        {
                            if (fullMessageSent)
                            {
                                return;
                            }

                            throw new SnepException("Received success response before full message was transmitted.");
                        }
                        else
                        {
                            throw new SnepException($"Received snep response {ackResponse.Response.ToString()}");
                        }
                    }
                }
            }

            // Full message was sent.
            var snepResponse = this.WaitForResponse();
            if (snepResponse.Response != SnepResponseCode.Success)
            {
                throw new SnepException($"Expected success after transmitting full message, but received {snepResponse.Response.ToString()}");
            }
        }

        private void SendContinue()
        {

        }

        public NdefMessage Get(NdefMessage request, int maxResponseLength = int.MaxValue)
        {
            throw new NotImplementedException();
        }


        private void SendFragment(Stream data, int length)
        {

        }

        private SnepResponse WaitForResponse()
        {
            throw new NotImplementedException();
        }
    }
}
