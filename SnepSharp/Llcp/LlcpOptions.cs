//
//  ILlcpOptions.cs
//
//  Author:
//       Jesse de Wit <witdejesse@hotmail.com>
//
//  Copyright (c) 2019 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace SnepSharp.Llcp
{
    using System;
    using System.Threading;

    /// <summary>
    /// Llcp options.
    /// </summary>
    public class LlcpOptions
    {
        /// <summary>
        /// The maximum information unit (MIU).
        /// </summary>
        private int maximumInformationUnit = 248;

        /// <summary>
        /// Gets or sets the maximum information unit (MIU).
        /// </summary>
        /// <remarks>The maximum information unit is announced during link 
        /// activation. The default and also smallest possible value is 128 
        /// bytes. Note that the actual used MIU may be smaller than this value,
        /// if the remote peer negotiates a lower MIU.</remarks>
        /// <value>The maximum information unit size.</value>
        public int MaximumInformationUnit
        {
            get => this.maximumInformationUnit;
            set
            {
                if (value < Constants.MaximumInformationUnit)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(MaximumInformationUnit),
                        "Cannot be smaller than " +
                            "{Constants.MaximumInformationUnit} bytes.");
                }

                this.maximumInformationUnit = value;
            }
        }

        /// <summary>
        /// Gets or sets the link timeout that is announced to the remote device 
        /// during link activation.
        /// </summary>
        /// <remarks>It informs the remote device that if the local device does 
        /// not return a protocol data unit before the timeout expires, the 
        /// communication link is broken and can not be recovered. This value is 
        /// an important part of the user experience, it ultimately tells when 
        /// the user should no longer expect communication to continue. Defaults
        /// to 500 milliseconds.</remarks>
        /// <value>The link timeout.</value>
        public TimeSpan LinkTimeout { get; set; }
            = TimeSpan.FromMilliseconds(500);
    }
}
