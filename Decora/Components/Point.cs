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
	public class Point : ViewModelEntity
	{
		#region Private Fields

		int _x, _y;

		#endregion

		#region Constructors

		public Point() { }

		public Point(int x, int y)
		{
			_x = x;
			_y = y;
		}

		#endregion

		#region Accessors

		public int X
		{
			get { return _x; }
			set
			{
				if (_x != value)
				{
					_x = value;
					NotifyPropertyChanged("X");
				}
			}
		}

		public int Y
		{
			get { return _y; }
			set
			{
				if (_y != value)
				{
					_y = value;
					NotifyPropertyChanged("Y");
				}
			}
		}

		#endregion

		#region Overrides

		public override bool Equals(object obj)
		{
			if (obj is Point)
			{
				var c = (Point)obj;

				return (_x == c.X && _y == c.Y);
			}

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{ return base.GetHashCode(); }

		#endregion

		#region Operators

		public static bool operator ==(Point p1, Point p2)
		{ return ((object)p1 != null) ? p1.Equals(p2) : false; }

		public static bool operator !=(Point p1, Point p2)
		{ return ((object)p1 != null) ? !p1.Equals(p2) : true; }

		#endregion
	}
}