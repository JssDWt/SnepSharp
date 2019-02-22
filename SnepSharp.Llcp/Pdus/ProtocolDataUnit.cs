namespace SnepSharp.Llcp.Pdus
{
    using System;

    /// <summary>
    /// Protocol data unit (PDU) that is exchanged between <see cref="DataLink"/> (LLC).
    /// </summary>
    internal abstract class ProtocolDataUnit
    {
        /// <summary>
        /// Gets the data link connection this PDU is transmitted over.
        /// </summary>
        /// <value>The data link connection.</value>
        public DataLink DataLinkConnection { get; }

        /// <summary>
        /// Gets the protocol data unit type.
        /// </summary>
        /// <value>The protocol data unit type.</value>
        public ProtocolDataUnitType Type { get; }

        /// <summary>
        /// Gets the sequence number.
        /// </summary>
        /// <value>The sequence number.</value>
        public SequenceNumber? Sequence { get; }

        /// <summary>
        /// Gets the information/content.
        /// </summary>
        /// <value>The information/content.</value>
        public byte[] Information { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.ProtocolDataUnit"/> class.
        /// </summary>
        /// <param name="connection">The datalink connection.</param>
        /// <param name="type">The data unit type.</param>
        /// <param name="sequence">Optional sequence number.</param>
        /// <param name="information">Optional information/content.</param>
        protected ProtocolDataUnit(
            DataLink connection, 
            ProtocolDataUnitType type, 
            SequenceNumber? sequence,
            byte[] information)
        {
            this.DataLinkConnection = connection;
            this.Type = type;
            this.Sequence = sequence;
            this.Information = information;
        }

        /// <summary>
        /// Creates the header Protocol Data Unit header.
        /// </summary>
        /// <returns>The header.</returns>
        private byte[] CreateHeader()
        {
            byte ptype = (byte)this.Type;
            int h0 = (this.DataLinkConnection.Destination << 2) | (ptype >> 2);
            int h1 = (this.DataLinkConnection.Source          ) | (ptype << 6);

            byte[] header;
            if (this.Sequence.HasValue)
            {
                header = new byte[3];
                header[2] = this.Sequence.Value.Sequence;
            }
            else
            {
                header = new byte[2];
            }

            header[0] = (byte)h0;
            header[1] = (byte)h1;
            return header;
        }
    }
}
