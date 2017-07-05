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
using System.Collections.ObjectModel;

#endregion

namespace Decora
{
	class Scene : ViewModelEntity
	{
		#region Private Fields

		ObservableCollection<UIControl> _controls = new ObservableCollection<UIControl>();
		ObservableCollection<UIControl> _children = new ObservableCollection<UIControl>();
		UIControl _parent;
		UIControl _selected;
		System.Windows.UIElement _native;

		#endregion

		#region Constructors

		public Scene(UIControl parent)
		{
			_parent = parent;
		}

		public Scene(UIControl parent, UIControl[] children) : this(parent)
		{
			_controls = new ObservableCollection<UIControl>(children);
			_controls.Insert(0, parent);

			_children = new ObservableCollection<UIControl>(children);
		}

		#endregion

		#region Accessors

		public ObservableCollection<UIControl> Controls
		{
			get { return _controls; }
			set { _controls = value; }
		}

		public ObservableCollection<UIControl> Children
		{
			get { return _children; }
			set { _children = value; }
		}
		
		public UIControl Parent
		{
			get { return _parent; }
			set
			{
				if (_parent != value)
				{
					_parent = value;
					NotifyPropertyChanged("Parent");
				}
			}
		}

		public UIControl SelectedControl
		{
			get { return _selected; }
			set
			{
				if (_selected != value)
				{
					_selected = value;
					NotifyPropertyChanged("SelectedControl");
				}
			}
		}

		public System.Windows.UIElement SelectedAsNative
		{
			get { return _native; }
			set
			{
				if (_native != value)
				{
					_native = value;
					NotifyPropertyChanged("SelectedAsNative");
				}
			}
		}

		#endregion
	}
}
