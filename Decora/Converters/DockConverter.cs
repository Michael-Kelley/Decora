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

#region Includes

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

#endregion

namespace Decora
{
	public class DockConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var d = (DockingMode)value;

			if (targetType == typeof(HorizontalAlignment))
			{
				if (d.HasFlag(DockingMode.HRight))
					return HorizontalAlignment.Right;
				else if (d.HasFlag(DockingMode.HCenter))
					return HorizontalAlignment.Center;
				else
					return HorizontalAlignment.Left;
			}
			else if (targetType == typeof(VerticalAlignment))
			{
				if (d.HasFlag(DockingMode.VBottom))
					return VerticalAlignment.Bottom;
				else if (d.HasFlag(DockingMode.VCenter))
					return VerticalAlignment.Center;
				else
					return VerticalAlignment.Top;
			}
			else
				return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var p = (DockingMode)parameter;

			if (value is HorizontalAlignment)
			{
				var a = (HorizontalAlignment)value;

				if (a == HorizontalAlignment.Stretch)
					return p;
				else
				{
					var result = (DockingMode)a;

					if (p.HasFlag(DockingMode.VBottom))
						result &= DockingMode.VBottom;
					else if (p.HasFlag(DockingMode.VCenter))
						result &= DockingMode.VCenter;

					return result;
				}
			}
			else if (value is VerticalAlignment)
			{
				var a = (VerticalAlignment)value;

				if (a == VerticalAlignment.Stretch)
					return p;
				else
				{
					var result = (DockingMode)((int)a * 4);

					if (p.HasFlag(DockingMode.HRight))
						result &= DockingMode.HRight;
					else if (p.HasFlag(DockingMode.HCenter))
						result &= DockingMode.HCenter;

					return result;
				}
			}
			else
				return null;
		}
	}
}