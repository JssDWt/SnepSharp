namespace SnepSharp.Llcp.Pdus
{
    using System;

    /// <summary>
    /// Frame reject (FRMR) unit. Is used to report the receipt of a malformed 
    /// or inappropriate PDU on a data link connection.
    /// </summary>
    internal class FrameRejectUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Gets the State variable for this data link connection..
        /// </summary>
        /// <value>The state.</value>
        public SequenceNumber State { get; }

        /// <summary>
        /// Gets the Acknowledgement state variable for this data link connection.
        /// </summary>
        /// <value>The state of the acknowledgement.</value>
        public SequenceNumber AcknowledgementState { get; }

        /// <summary>
        /// Gets the Optional rejected pdu sequence number.
        /// </summary>
        /// <value>The rejected pdu sequence.</value>
        public SequenceNumber? RejectedPduSequence { get; }

        /// <summary>
        /// Gets a value indicating whether the rejected pdu was malformed.
        /// </summary>
        /// <value><c>true</c> if is malformed; otherwise, <c>false</c>.</value>
        public bool IsMalformed { get; }

        /// <summary>
        /// Gets a value indicating whether the rejected pdu contained incorrect information.
        /// </summary>
        /// <value><c>true</c> if incorrect information; otherwise, <c>false</c>.</value>
        public bool IsIncorrectInformation { get; }

        /// <summary>
        /// Gets a value indicating whether the rejected pdu contained an invalid receive sequence.
        /// </summary>
        /// <value><c>true</c> if invalid receive sequence; otherwise, <c>false</c>.</value>
        public bool IsInvalidReceiveSequence { get; }

        /// <summary>
        /// Gets a value indicating whether the rejected pdu contained an invalid send sequence.
        /// </summary>
        /// <value><c>true</c> if invalid send sequence; otherwise, <c>false</c>.</value>
        public bool IsInvalidSendSequence { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.FrameRejectUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="rejectedPduType">Rejected pdu type.</param>
        /// <param name="state">State variable for this data link connection.</param>
        /// <param name="ackState">Acknowledgement state variable for this data link connection.</param>
        /// <param name="rejectedPduSequence">Optional rejected pdu sequence number.</param>
        /// <param name="malformed">If set to <c>true</c>, indicate the rejected pdu was malformed.</param>
        /// <param name="incorrectInformation">If set to <c>true</c>, indicate the rejected pdu contained incorrect information.</param>
        /// <param name="invalidReceiveSequence">If set to <c>true</c>, indicate the rejected pdu contained an invalid receive sequence.</param>
        /// <param name="invalidSendSequence">If set to <c>true</c>, indicate the rejected pdu contained an invalid send sequence.</param>
        public FrameRejectUnit(
            DataLink connection, 
            ProtocolDataUnitType rejectedPduType,
            SequenceNumber state,
            SequenceNumber ackState,
            SequenceNumber? rejectedPduSequence = null,
            bool malformed = false, 
            bool incorrectInformation = false, 
            bool invalidReceiveSequence = false,
            bool invalidSendSequence = false)
            : base(
                  connection, 
                  ProtocolDataUnitType.FrameReject, 
                  null, 
                  ToBytes(
                      rejectedPduType,
                      state,
                      ackState,
                      rejectedPduSequence,
                      malformed,
                      incorrectInformation,
                      invalidReceiveSequence,
                      invalidSendSequence))
        {
            this.State = state;
            this.AcknowledgementState = ackState;
            this.RejectedPduSequence = rejectedPduSequence;
            this.IsMalformed = malformed;
            this.IsIncorrectInformation = incorrectInformation;
            this.IsInvalidReceiveSequence = invalidReceiveSequence;
            this.IsInvalidSendSequence = invalidSendSequence;
        }

        /// <summary>
        /// Creates the information field for this pdu.
        /// </summary>
        /// <returns>The information bytes.</returns>
        /// <param name="rejectedPduType">Rejected pdu type.</param>
        /// <param name="state">State variable for this data link connection.</param>
        /// <param name="ackState">Acknowledgement state variable for this data link connection.</param>
        /// <param name="rejectedPduSequence">Optional rejected pdu sequence number.</param>
        /// <param name="malformed">If set to <c>true</c>, indicate the rejected pdu was malformed.</param>
        /// <param name="incorrectInformation">If set to <c>true</c>, indicate the rejected pdu contained incorrect information.</param>
        /// <param name="invalidReceiveSequence">If set to <c>true</c>, indicate the rejected pdu contained an invalid receive sequence.</param>
        /// <param name="invalidSendSequence">If set to <c>true</c>, indicate the rejected pdu contained an invalid send sequence.</param>
        private static byte[] ToBytes(
            ProtocolDataUnitType rejectedPduType,
            SequenceNumber state,
            SequenceNumber ackState,
            SequenceNumber? rejectedPduSequence = null,
            bool malformed = false,
            bool incorrectInformation = false,
            bool invalidReceiveSequence = false,
            bool invalidSendSequence = false)
        {
            bool anyReason = malformed || incorrectInformation || invalidReceiveSequence || invalidSendSequence;
            if (!anyReason) throw new ArgumentException("No reject reason supplied");

            var info = new byte[4];
            info[0] = 0x00;
            if (malformed) info[0] |= 0x80;
            if (incorrectInformation) info[0] |= 0x40;
            if (invalidReceiveSequence) info[0] |= 0x20;
            if (invalidSendSequence) info[0] |= 0x10;

            info[0] |= (byte)rejectedPduType;
            if (rejectedPduSequence.HasValue)
            {
                info[1] = rejectedPduSequence.Value.Sequence;
            }
            else
            {
                info[1] = 0x00;
            }

            info[2] = state.Sequence;
            info[3] = ackState.Sequence;

            return info;
        }
    }
}
