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
	class UIControl : ViewModelEntity
	{
		#region Private Fields

		int _imageIndex, _textIndex, _textLines;
		Rect _rectangle;
		State _elementState, _imageState;

		#endregion

		#region Public Fields

		public string ParentName;
		public string Description;
		public int unk0;
		public int ID;
		public int ParentID;
		public int Style;
		public int unk1;
		public int unk2;
		public int unk3;
		public int unk4;
		public int unk5;
		public int ElementIndex;
		public DockingMode Dock;

		#endregion

		#region Accessors

		public Text Content { get; set; }

		public int Height
		{
			get { return _rectangle.Bottom - _rectangle.Top; }
			set
			{
				if (_rectangle.Bottom != _rectangle.Top + value)
				{
					_rectangle.Bottom = _rectangle.Top + value;
					NotifyPropertyChanged("Height");
				}
			}
		}

		public int ImageIndex
		{
			get { return _imageIndex; }
			set
			{
				if (_imageIndex != value)
				{
					_imageIndex = value;
					NotifyPropertyChanged("ImageIndex");
				}
			}
		}

		public State ImageState
		{
			get { return _imageState; }
			set
			{
				if (_imageState != value)
				{
					_imageState = value;
					NotifyPropertyChanged("ImageState");
				}
			}
		}

		public State ElementState
		{
			get { return _elementState; }
			set
			{
				if (_elementState != value)
				{
					_elementState = value;
					NotifyPropertyChanged("ElementState");
				}
			}
		}

		public string Name { get; set; }

		public UIControl Parent { get; set; }

		public Rect Rectangle
		{
			get { return _rectangle; }
			set
			{
				if (_rectangle != value)
				{
					if (value.Right != -1 && value.Bottom != -1)
						_rectangle = value;
					else
					{
						_rectangle.Left = value.Left;
						_rectangle.Top = value.Top;
					}

					NotifyPropertiesChanged("Rectangle", "X", "Y");
				}
			}
		}

		public object Tag { get; set; }

		public int TextIndex
		{
			get { return _textIndex; }
			set
			{
				if (_textIndex != value)
				{
					_textIndex = value;
					NotifyPropertyChanged("TextIndex");
				}
			}
		}

		public int TextLines
		{
			get { return _textLines; }
			set
			{
				if (_textLines != value)
				{
					_textLines = value;
					NotifyPropertyChanged("TextLines");
				}
			}
		}

		public int Width
		{
			get { return _rectangle.Right - _rectangle.Left; }
			set
			{
				if (_rectangle.Right != _rectangle.Left + value)
				{
					_rectangle.Right = _rectangle.Left + value;
					NotifyPropertyChanged("Width");
				}
			}
		}

		public int X
		{
			get { return _rectangle.Left; }
			set
			{
				if (_rectangle.Left != value)
				{
					_rectangle.Left = value;
					NotifyPropertyChanged("Rectangle");
				}
			}
		}

		public int Y
		{
			get { return _rectangle.Top; }
			set
			{
				if (_rectangle.Top != value)
				{
					_rectangle.Top = value;
					NotifyPropertyChanged("Rectangle");
				}
			}
		}

		#endregion

		#region Interface Implementations

		public bool Write(System.IO.BinaryWriter writer)
		{
			try
			{
				writer.Write((ushort)Name.Length);
				writer.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(Name));
				writer.Write((ushort)ParentName.Length);
				writer.Write(System.Text.ASCIIEncoding.ASCII.GetBytes(ParentName));
				var desc = System.Text.Encoding.GetEncoding(949).GetBytes(Description);	// Korean
				writer.Write((ushort)desc.Length);
				writer.Write(desc);
				writer.Write(unk0);
				writer.Write(ID);
				writer.Write(ParentID);
				writer.Write(Style);
				writer.Write(_rectangle.Left);
				writer.Write(_rectangle.Top);
				writer.Write(_rectangle.Right);
				writer.Write(_rectangle.Bottom);
				writer.Write(unk1);
				writer.Write(unk2);
				writer.Write(unk3);
				writer.Write(unk4);
				writer.Write(unk5);
				writer.Write(ElementIndex);
				writer.Write((int)_elementState);
				writer.Write(_imageIndex);
				writer.Write((int)_imageState);
				writer.Write(_textIndex);
				writer.Write(_textLines);
				writer.Write((int)Dock);

				return true;
			}
			catch (System.Exception)
				{ return false; }
		}

		#endregion
	}
}