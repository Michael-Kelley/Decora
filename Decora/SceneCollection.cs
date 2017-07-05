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

using System.Collections.ObjectModel;

#endregion

namespace Decora
{
	class SceneCollection : ViewModelEntity
	{
		#region Private Fields

		ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
		Scene _selectedScene;

		#endregion

		#region Accessors

		public ObservableCollection<Scene> Scenes
		{
			get { return _scenes; }
			set { _scenes = value; }
		}

		public Scene SelectedScene
		{
			get { return _selectedScene; }
			set
			{
				if (_selectedScene != value)
				{
					_selectedScene = value;
					NotifyPropertyChanged("SelectedScene");
				}
			}
		}

		#endregion
	}
}