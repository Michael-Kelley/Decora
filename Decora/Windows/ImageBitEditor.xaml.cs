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

using System.Collections.Generic;
using System.Windows;

#endregion

namespace Decora
{
	/// <summary>Interaction logic for ImageBitEditor.xaml</summary>
	public partial class ImageBitEditor : Window
	{
		public List<ImageBit> Bits;

		public ImageBitEditor(List<ImageBit> bits, string directory, List<Texture> textures)
		{
			this.InitializeComponent();

			Bits = new List<ImageBit>();
			var tex = new List<Texture>();
			
			foreach (var b in bits)
				Bits.Add(new ImageBit(b.TextureIndex, new Point(b.TopLeft.X, b.TopLeft.Y), b.Width, b.Height));
			
			foreach (var t in textures)
				tex.Add(new Texture(t.Name.Insert(0, directory + "\\"), t.Width, t.Height));

			comboBits.ItemsSource = Bits;
			comboTextures.ItemsSource = tex;
		}

		void Btn_AddImageBit_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Bits.Add(new ImageBit());
			comboBits.SelectedIndex = Bits.Count - 1;
		}

		void Btn_Save_Click(object sender, System.Windows.RoutedEventArgs e)
			{ DialogResult = true; }
	}
}