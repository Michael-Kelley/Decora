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
	public class ElementPair : ViewModelEntity, IComponent
	{
		#region Public Fields

		public uint Unknown;
		public string Element1;
		public string Element2;

		#endregion

		#region Interface Implementations

		public bool Write(System.IO.BinaryWriter writer)
		{
			try
			{
				writer.Write(Unknown);
				writer.Write((ushort)Element1.Length);
				writer.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(Element1));
				writer.Write((ushort)Element2.Length);
				writer.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(Element2));

				return true;
			}
			catch (System.Exception)
				{ return false; }
		}

		#endregion
	}
}