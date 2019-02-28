//
//  SnepResponseCode.cs
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

namespace SnepSharp.Snep.Messages
{
    /// <summary>
    /// Enumaration containing snep response status codes.
    /// </summary>
    internal enum SnepResponseCode
    {
        Continue = 0x80,
        Success = 0x81,
        NotFound = 0xC0,
        ExcessData = 0xC1,
        BadRequest = 0xC2,
        NotImplemented = 0xE0,
        UnsupportedVersion = 0xE1,
        Reject = 0xFF
    }
}
