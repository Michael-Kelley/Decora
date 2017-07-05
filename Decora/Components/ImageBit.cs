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
	public class ImageBit : ViewModelEntity, IComponent
	{
		#region Private Fields

		int _textureIndex;
		Point _topLeft;
		int _width, _height;

		#endregion

		#region Constructors

		public ImageBit()
			{ _topLeft = new Point(0, 0); }

		public ImageBit(int textureindex, Point topleft, int width, int height)
		{
			_textureIndex = textureindex;
			_topLeft = topleft;
			_width = width;
			_height = height;
		}

		#endregion

		#region Accessors

		public int Height
		{
			get { return _height; }
			set
			{
				if (_height != value && (int)value > 0)
				{
					_height = value;
					NotifyPropertiesChanged("Height", "Name");
				}
			}
		}

		public string Name
			{ get { return System.String.Format("{0} - {1}, {2}, {3}, {4}", _textureIndex, _topLeft.X, _topLeft.Y, _width, _height); } }

		public int TextureIndex
		{
			get { return _textureIndex; }
			set
			{
				if (_textureIndex != value)
				{
					_textureIndex = value;
					NotifyPropertiesChanged("TextureIndex", "Name");
				}
			}
		}

		public Point TopLeft
		{
			get { return _topLeft; }
			set
			{
				if (_topLeft != value)
				{
					_topLeft = value;
					NotifyPropertiesChanged("TopLeft", "X", "Y", "Name");
				}
			}
		}

		public int Width
		{
			get { return _width; }
			set
			{
				if (_width != value && (int)value > 0)
				{
					_width = value;
					NotifyPropertiesChanged("Width", "Name");
				}
			}
		}

		public int X
		{
			get { return _topLeft.X; }
			set
			{
				if (_topLeft.X != value)
				{
					_topLeft.X = value;
					NotifyPropertiesChanged("X", "TopLeft", "Name");
				}
			}
		}

		public int Y
		{
			get { return _topLeft.Y; }
			set
			{
				if (_topLeft.Y != value)
				{
					_topLeft.Y = value;
					NotifyPropertiesChanged("Y", "TopLeft", "Name");
				}
			}
		}

		#endregion

		#region Interface Implementations

		public bool Write(System.IO.BinaryWriter writer)
		{
			try
			{
				writer.Write(_textureIndex);
				writer.Write(_topLeft.X);
				writer.Write(_topLeft.Y);
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