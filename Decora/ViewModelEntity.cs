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

using System.ComponentModel;

#endregion

namespace Decora
{
	 /// <summary>Standard viewmodel class base, simply allows property change notifications to be sent.</summary>
	 public class ViewModelEntity : INotifyPropertyChanged
	 {
		 /// <summary>The property changed event.</summary>
		 public event PropertyChangedEventHandler PropertyChanged;
		
		 /// <summary>Raises the property changed event.</summary>
		 /// <param name="propertyName">Name of the property.</param>
		 public virtual void NotifyPropertyChanged(string propertyName)
		 {
			 //  If the event has been subscribed to, fire it.
			 if (PropertyChanged != null)
				 PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		 }

		 public virtual void NotifyPropertiesChanged(params string[] propertyNames)
		 {
			 foreach (var p in propertyNames)
				 NotifyPropertyChanged(p);
		 }
	 }
}