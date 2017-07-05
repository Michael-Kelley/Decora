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
	[ValueConversion(typeof(Colour), typeof(Color))]
	public class ColourConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var c = (Colour)value;

			return Color.FromArgb((byte)(0xFF - c.Alpha), c.Red, c.Green, c.Blue);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var c = (Color)value;

			return new Colour() { Blue = c.B, Green = c.G, Red = c.R, Alpha = (byte)(0xFF - c.A) };
		}
	}
}