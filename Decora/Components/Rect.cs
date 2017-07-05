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
	public class Rect : ViewModelEntity
	{
		#region Private Fields

		int _left, _top, _right, _bottom;

		#endregion

		#region Constructor

		public Rect(int left, int top, int right, int bottom)
		{
			_left = left;
			_top = top;
			_right = right;
			_bottom = bottom;
		}

		#endregion

		#region Accessors

		public int Bottom
		{
			get { return _bottom; }
			set
			{
				if (_bottom != value)
				{
					_bottom = value;
					NotifyPropertyChanged("Bottom");
				}
			}
		}

		public int Left
		{
			get { return _left; }
			set
			{
				if (_left != value)
				{
					_right += value - _left;
					_left = value;

					NotifyPropertyChanged("Left");
				}
			}
		}

		public int Right
		{
			get { return _right; }
			set
			{
				if (_right != value)
				{
					_right = value;
					NotifyPropertyChanged("Right");
				}
			}
		}

		public int Top
		{
			get { return _top; }
			set
			{
				if (_top != value)
				{
					_bottom += value - _top;
					_top = value;

					NotifyPropertyChanged("Top");
				}
			}
		}

		#endregion
	}
}