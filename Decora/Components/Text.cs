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
	class Text : ViewModelEntity, IComponent
	{
		#region Private Fields

		string _name = "";
		Point _location;
		int _width, _height;
		Colour _normalColour, _hoverColour, _pressedColour;
		string _content = "";
		int _dtsIndex;
		DockingMode _dock;
		System.Windows.HorizontalAlignment _hAlign;
		System.Windows.VerticalAlignment _vAlign;

		#endregion

		#region Public Fields

		public int ID;
		public int unk1;
		public byte unk2;
		public short unk3;

		#endregion

		#region Accessors

		public string Content
		{
			get { return _content; }
			set
			{
				if (_content != value)
				{
					_content = value;
					NotifyPropertyChanged("Content");
				}
			}
		}

		public DockingMode Dock
		{
			get { return _dock; }
			set
			{
				if (_dock != value)
				{
					_dock = value;

					if (_dock.HasFlag(DockingMode.VBottom))
						_hAlign = (System.Windows.HorizontalAlignment)(_dock ^ DockingMode.VBottom);
					else if (_dock.HasFlag(DockingMode.VCenter))
						_hAlign = (System.Windows.HorizontalAlignment)(_dock ^ DockingMode.VCenter);
					else
						_hAlign = System.Windows.HorizontalAlignment.Left;

					if (_dock.HasFlag(DockingMode.HRight))
						_vAlign = (System.Windows.VerticalAlignment)((int)(_dock ^ DockingMode.HRight) / 4);
					else if (_dock.HasFlag(DockingMode.VCenter))
						_vAlign = (System.Windows.VerticalAlignment)((int)(_dock ^ DockingMode.HCenter) / 4);
					else
						_vAlign = System.Windows.VerticalAlignment.Top;

					NotifyPropertiesChanged("Dock", "HAlign", "VAlign");
				}
			}
		}

		public System.Windows.HorizontalAlignment HAlign
		{
			get { return _hAlign; }
			set
			{
				if (_hAlign != value)
				{
					_hAlign = value;
					_dock = (DockingMode)_hAlign | (DockingMode)((int)_vAlign * 4);
					NotifyPropertiesChanged("HAlign", "Dock");
				}
			}
		}

		public System.Windows.VerticalAlignment VAlign
		{
			get { return _vAlign; }
			set
			{
				if (_vAlign != value)
				{
					_vAlign = value;
					_dock = (DockingMode)_hAlign | (DockingMode)((int)_vAlign * 4);
					NotifyPropertiesChanged("VAlign", "Dock");
				}
			}
		}

		public int DTSIndex
		{
			get { return _dtsIndex; }
			set
			{
				if (_dtsIndex != value)
				{
					_dtsIndex = value;
					NotifyPropertyChanged("DTSIndex");
				}
			}
		}

		public int LineHeight
		{
			get { return _height; }
			set
			{
				if (_height != value)
				{
					_height = value;
					NotifyPropertyChanged("LineHeight");
				}
			}
		}

		public Colour HoverColour
		{
			get { return _hoverColour; }
			set
			{
				if (_hoverColour != value)
				{
					_hoverColour = value;
					NotifyPropertyChanged("HoverColour");
				}
			}
		}

		public Point Location
		{
			get { return _location; }
			set
			{
				if (_location != value)
				{
					_location = value;
					NotifyPropertiesChanged("Location", "X", "Y");
				}
			}
		}

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

		public Colour NormalColour
		{
			get { return _normalColour; }
			set
			{
				if (_normalColour != value)
				{
					_normalColour = value;
					NotifyPropertyChanged("NormalColour");
				}
			}
		}

		public Colour PressedColour
		{
			get { return _pressedColour; }
			set
			{
				if (_pressedColour != value)
				{
					_pressedColour = value;
					NotifyPropertyChanged("PressedColour");
				}
			}
		}

		public int Width
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

		public int X
		{
			get { return _location.X; }
			set
			{
				if (_location.X != value)
				{
					_location.X = value;
					NotifyPropertiesChanged("Location", "X");
				}
			}
		}

		public int Y
		{
			get { return _location.Y; }
			set
			{
				if (_location.Y != value)
				{
					_location.Y = value;
					NotifyPropertiesChanged("Location", "Y");
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
				writer.Write(ID);
				writer.Write(_location.X);
				writer.Write(_location.Y);
				writer.Write(_width);
				writer.Write(_height);
				writer.Write(unk1);
				writer.Write(_normalColour.ToByteArray());
				writer.Write(_hoverColour.ToByteArray());
				writer.Write(_pressedColour.ToByteArray());
				writer.Write((byte)_dock);
				writer.Write(unk2);
				writer.Write(unk3);
				writer.Write(_dtsIndex);

				return true;
			}
			catch (System.Exception)
				{ return false; }
		}

		#endregion
	}
}