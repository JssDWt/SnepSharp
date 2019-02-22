using System;
using System.Collections.Generic;

namespace Llcp
{
    /// <summary>
    /// A Logical Link Control (LLC)
    /// </summary>
    public class LogicalLinkControl
    {
        private const int MaxUnacknowledgedUnits = 15;
        private HashSet<DataLink> connections;
        private Dictionary<DataLink, Dictionary<int, ProtocolDataUnit>> unacknowledgedUnits;

        public LogicalLinkControl()
        {
        }

        private void Close(DataLink link)
        {
            // TODO Send close to remote llc

            this.unacknowledgedUnits.Remove(link);
            this.connections.Remove(link);
        }
    }
}
