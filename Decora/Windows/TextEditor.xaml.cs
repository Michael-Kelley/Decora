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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#endregion

namespace Decora
{
	/// <summary>Interaction logic for TextureEditor.xaml</summary>
	public partial class TextEditor : Window
	{
		public List<string> DTS;

		public TextEditor(List<string> dts)
		{
			this.InitializeComponent();

			DTS = dts;

			txtDTS.Text = String.Join("\r\n", dts);
		}

		private void Btn_OK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}

		private void txtText_SelectionChanged(object sender, RoutedEventArgs e)
		{
			int charIndex = txtDTS.CaretIndex;
			int lineIndex = txtDTS.GetLineIndexFromCharacterIndex(charIndex);
			var rect = txtDTS.GetRectFromCharacterIndex(charIndex);
			lblLine.Content = lineIndex;
			Matrix matrix = lblLine.RenderTransform.Value;

			if (Math.Ceiling(rect.Top) >= txtDTS.ViewportHeight - lblLine.ActualHeight)
				matrix.OffsetY = txtDTS.ViewportHeight - Math.Floor(lblLine.ActualHeight);
			else if (Math.Ceiling(rect.Top) >= 4)
				matrix.OffsetY = rect.Top;

			lblLine.RenderTransform = new MatrixTransform(matrix);
		}
	}
}