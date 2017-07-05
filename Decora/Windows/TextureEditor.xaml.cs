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
	/// <summary>Interaction logic for TextureEditor.xaml</summary>
	public partial class TextureEditor : Window
	{
		public List<Texture> Textures;

		public TextureEditor(List<Texture> textures)
		{
			this.InitializeComponent();

			Textures = textures;

			comboTextures.ItemsSource = Textures;
		}

		private void Btn_AddTexture_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Textures.Add(new Texture());
			comboTextures.SelectedIndex = Textures.Count - 1;
		}

		private void Btn_OK_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}