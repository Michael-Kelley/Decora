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

#endregion

namespace Decora
{
	public class RectBindingConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 2)
			{
				if ((double)values[0] == 0)
					return new System.Windows.Rect(0, 0, 1, 1);
				else
					return new System.Windows.Rect(0, 0, (double)values[0], (double)values[1]);
			}
			else
			{
				if (values[0] != DependencyProperty.UnsetValue)
					return new System.Windows.Rect((double)((int)values[0]), (double)((int)values[1]), (double)((int)values[2]), (double)((int)values[3]));
				else
					return new System.Windows.Rect(0, 0, 1, 1);
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{ return null; }
	}
}