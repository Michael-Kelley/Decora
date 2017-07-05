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
using System.Windows;
using System.Windows.Documents;

#endregion

namespace Decora
{
	/// <summary>Interaction logic for AboutWindow.xaml</summary>
	public partial class AboutWindow : Window
	{
		#region Constructor

		public AboutWindow()
		{
			this.InitializeComponent();

			var soundEngine = NAudioEngine.Instance;
			spectrumAnalyzer.RegisterSoundPlayer(soundEngine);
			var waveformAnalyzer = new WPFSoundVisualizationLib.WaveformTimeline();
			waveformAnalyzer.RegisterSoundPlayer(soundEngine);

			var stream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Sounds/tdp_music.mp3")).Stream;
			soundEngine.OpenStream(stream);
			soundEngine.Play();
		}

		#endregion

		#region Control Events

		void TDP_Navigate(object sender, RoutedEventArgs e)
			{ System.Diagnostics.Process.Start(((Hyperlink)sender).NavigateUri.ToString()); }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
			{ NAudioEngine.Instance.Stop(); }

		#endregion
	}
}