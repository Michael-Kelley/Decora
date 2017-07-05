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

namespace Decora
{
	enum DockingMode : int
	{
		None = 0,
		HCenter = 1,
		HRight = 2,
		VCenter = 4,
		VBottom = 8,
	}

	enum State : int
	{
		Hidden = 0,
		Visible = 1,
		Unk_2 = 2,
		Clickable = 3,
		Textbox = 4
	}
	
	enum Language : int
	{
		Korean = 1,
		English = 2,
		French = 5,
        Portuguese = 6,
		German = 12,
		Italian,
		Spanish,
		Turkish
	}
}