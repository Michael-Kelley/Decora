/*
	Copyright © 2014, Forge Development
	All rights reserved.
	http://forge-dev.com


	This file is part of Decora.

	Decora is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	any later version.

	Decora is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Decora.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace Decora
{
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
	struct Colour
	{
		public byte Blue, Green, Red, Alpha;

		public override bool Equals(object obj)
		{
			if (obj is Colour)
			{
				var c = (Colour)obj;

				return (Blue == c.Blue &&
						Green == c.Green &&
						Red == c.Red &&
						Alpha == c.Alpha);
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{ return base.GetHashCode(); }

		public static bool operator ==(Colour c1, Colour c2)
		{ return ((object)c1 != null) ? c1.Equals(c2) : false; }

		public static bool operator !=(Colour c1, Colour c2)
		{ return ((object)c1 != null) ? !c1.Equals(c2) : true; }
	}
}