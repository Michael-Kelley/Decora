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
using System.Windows.Data;
using System.Windows.Media;

#endregion

namespace Decora
{
	public class NonEqualityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object p = null;

			if (value is int)
				p = int.Parse((string)parameter);

			return !(value.Equals(p));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{ return null; }
	}
}