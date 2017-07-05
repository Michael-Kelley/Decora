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
	public class Texture : ViewModelEntity, IComponent
	{
		#region Private Fields

		string _name;
		uint _width, _height;

		#endregion

		#region Constructors

		public Texture() { }

		public Texture(string name, uint width, uint height)
		{
			_name = name;
			_width = width;
			_height = height;
		}

		#endregion

		#region Accessors

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		public uint Width
		{
			get { return _width; }
			set
			{
				if (_width != value)
				{
					_width = value;
					NotifyPropertyChanged("Width");
				}
			}
		}

		public uint Height
		{
			get { return _height; }
			set
			{
				if (_height != value)
				{
					_height = value;
					NotifyPropertyChanged("Height");
				}
			}
		}

		#endregion

		#region Interface Implementations

		public bool Write(System.IO.BinaryWriter writer)
		{
			try
			{
				writer.Write((ushort)_name.Length);
				writer.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(_name));
				writer.Write(_width);
				writer.Write(_height);

				return true;
			}
			catch (System.Exception)
				{ return false; }
		}

		#endregion
	}
}