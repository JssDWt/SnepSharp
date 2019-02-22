using System;
namespace Llcp
{
    /// <summary>
    /// The version number (VERSION) parameter is a configuration parameter that 
    /// MUST be transmitted during the link activation. It SHALL denote the 
    /// major and minor release levels of the LLCP specification implemented by 
    /// the sending LLC.
    /// </summary>
    internal class VersionParameter : Parameter
    {
        /// <summary>
        /// Gets the LLCP version.
        /// </summary>
        /// <value>The LLCP version.</value>
        public LlcpVersion Version { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.VersionParameter"/> class.
        /// </summary>
        /// <param name="version">The LLCP version.</param>
        public VersionParameter(LlcpVersion version)
            : base(ParameterType.Version, new byte[] { (byte)version })
        {
            this.Version = version;
        }
    }
}
