namespace Llcp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Aggregated frame (AGF) unit. MAY be used by the LLC Link Management 
    /// component to aggregate and transfer multiple LLC PDUs to the remote LLC 
    /// Link Management component in a single transmission.
    /// </summary>
    internal class AggregatedFrameUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.AggregatedFrameUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="aggregate">Aggregate of protocol data units.</param>
        public AggregatedFrameUnit(
            DataLink connection, 
            ICollection<ProtocolDataUnit> aggregate)
            : base(connection, ProtocolDataUnitType.AggregatedFrame, null, )
        {

        }

        /// <summary>
        /// Converts the aggregate Protocol Data units to bytes.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="aggregate">The aggragate data units.</param>
        private static byte[] ToBytes(ICollection<ProtocolDataUnit> aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }

            if (aggregate.Count < 2)
            {
                throw new ArgumentException("At least two subunits should be supplied.", nameof(aggregate));
            }

            if (aggregate.Any(pdu => pdu is AggregatedFrameUnit || pdu is SymmetryUnit))
            {
                throw new ArgumentException("Aggregate cannot contain aggregate or symmetry pdus.", nameof(aggregate));
            }

            // NOTE: first two bytes of each unit contain length, then data.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the specified bytes to Protocol Data units.
        /// </summary>
        /// <returns>The protocol data units.</returns>
        /// <param name="data">bytes containing protocol data units.</param>
        private static ICollection<ProtocolDataUnit> FromBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            throw new NotImplementedException();
        }


    }
}
