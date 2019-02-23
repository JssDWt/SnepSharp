
namespace SnepSharp.Llcp
{
    using System;
    using SnepSharp.Llcp.Parameters;

    public class MacMapping
    {
        public ParameterList Parameters { get; }

        /// <summary>
        /// Gets a value indicating whether the MAC allows parameter exchange.
        /// </summary>
        /// <value><c>true</c> if parameter exchange allowed; otherwise, <c>false</c>.</value>
        public bool ParameterExchangeAllowed { get; }

        /// <summary>
        /// Gets a value indicating whether the local LLCP is initiator.
        /// </summary>
        /// <value><c>true</c> if is initiator; <c>false</c> if is target.</value>
        public bool IsInitiator { get; }

        public MacMapping()
        {
        }
    }
}
