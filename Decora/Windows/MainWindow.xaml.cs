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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Expression.Interactivity.Layout;

using Behaviors;

#endregion

namespace Decora
{
	/// <summary>Interaction logic for MainWindow.xaml</summary>
	public partial class MainWindow : Window
	{
		string dir;

		Header header;
		List<Texture> textures;
		List<int> unknowns;
		List<ImageBit> images;
		List<ElementPair> elements;
		List<Text> texts;
		List<UIControl> controls;
		ObservableCollection<string> dts;
		
		Language language;

		public MainWindow()
		{
			this.InitializeComponent();

			textures = new List<Texture>();
			unknowns = new List<int>();
			images = new List<ImageBit>();
			elements = new List<ElementPair>();
			texts = new List<Text>();
			controls = new List<UIControl>();
			dts = new ObservableCollection<string>();

			comboBits.Items.Clear();
			comboScenes.Items.Clear();
			comboTexts.Items.Clear();
		}

		private void Window_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
		{
			var zoom = ((MatrixTransform)canvas.RenderTransform).Matrix;
			zoom.ScaleAt(1 + e.Delta / 1000D, 1 + e.Delta / 1000D, e.GetPosition(this).X, e.GetPosition(this).Y);

			canvas.RenderTransform = new MatrixTransform(zoom);
		}

		private void ToolBar_Open_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var open = new Microsoft.Win32.OpenFileDialog();
			open.Title = "Select your ui.dat...";
			open.FileName = "ui.dat";
			open.DefaultExt = ".dat";
			open.Filter = "CABAL Online UI Database|ui.dat";

			bool? result = open.ShowDialog();

			if (result == true)
			{
				OpenUI(open.FileName);

				UIScenes.Scenes.Clear();
				var parents = controls.Where(c => c.ParentID == 1);

				foreach (var p in parents)
				{
					var children = controls.Where(c => c.ParentID == p.ID);
					var scene = new Scene(p, children.ToArray());
					UIScenes.Scenes.Add(scene);
				}

				if (comboBits.ItemsSource == null)
				{
					comboBits.Items.Clear();
					comboScenes.Items.Clear();
					comboTexts.Items.Clear();
				}

				comboBits.ItemsSource = images;
				comboScenes.ItemsSource = UIScenes.Scenes;
				comboTexts.ItemsSource = texts;
				comboDTS.ItemsSource = dts;
			}
		}

		private void ToolBar_Save_Click(object sender, RoutedEventArgs e)
			{ SaveUI(); }

		private void ToolBar_TextureEditor_Click(object sender, RoutedEventArgs e)
		{
			var editor = new TextureEditor(textures);
			editor.Owner = this;

			if (editor.ShowDialog() == true)
				textures = editor.Textures;

			e.Handled = true;
		}

		private void ToolBar_ImageBitEditor_Click(object sender, RoutedEventArgs e)
		{
			var editor = new ImageBitEditor(images, dir, textures);
			editor.Owner = this;

			if (editor.ShowDialog() == true)
			{
				int index = comboScenes.SelectedIndex;
				images = editor.Bits;
				comboScenes.SelectedIndex = index;
			}

			e.Handled = true;
		}

		private void ToolBar_TextEditor_Click(object sender, RoutedEventArgs e)
		{
			var editor = new TextEditor(dts.ToList());
			editor.Owner = this;

			if (editor.ShowDialog() == true)
				dts = new ObservableCollection<string>(editor.DTS);

			e.Handled = true;
		}

		private void ToolBar_About_Click(object sender, RoutedEventArgs e)
		{
			var about = new AboutWindow();
			about.Owner = this;

			about.ShowDialog();

			e.Handled = true;
		}

		void control_MouseDown(object sender, MouseButtonEventArgs e)
		{
			var control = (Image)sender;
			var selected = (UIControl)control.Tag;
			UIScenes.SelectedScene.SelectedControl = selected;
			UIScenes.SelectedScene.SelectedAsNative = control;

			e.Handled = true;
		}

		private void comboScenes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			for (int i = 0; i < canvas.Children.Count; i++)
			{
				var c = canvas.Children[i];
				
				if (c is Image && (c as Image).Source != null)
				{
					((c as Image).Source as CroppedBitmap).Source = null;
					(c as Image).Source = null;
					c = null;
				}
			}

			canvas.Children.Clear();

			if (e.AddedItems.Count == 0 && e.RemovedItems.Count > 0) return;

			AddControl(UIScenes.SelectedScene.Parent);

			foreach (var c in UIScenes.SelectedScene.Children)
				AddControl(c);

            SaveUI();
            textures.RemoveAt(0);
            
		}

		private void comboControls_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (UIScenes.SelectedScene.SelectedControl == null) return;

			var children = new UIElement[canvas.Children.Count];
			canvas.Children.CopyTo(children, 0);
			UIScenes.SelectedScene.SelectedAsNative = children.FirstOrDefault(c => c is Image && ((Image)c).Tag == UIScenes.SelectedScene.SelectedControl);
		}

		private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (UIScenes.SelectedScene == null) return;

			UIScenes.SelectedScene.SelectedControl = null;
			UIScenes.SelectedScene.SelectedAsNative = null;
		}

		private void Txt_Lines_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if ((sender as TextBox).CaretIndex > 0 && e.Changes.ToArray()[0].AddedLength > 0)
			{
				canvas.Children.Clear();

				AddControl(UIScenes.SelectedScene.Parent);

				foreach (var c in UIScenes.SelectedScene.Children)
					AddControl(c);
			}
		}

		void OpenUI(string file)
		{
			dir = System.IO.Path.GetDirectoryName(file);

			var ui = new BinaryReader(File.OpenRead(file));

			header = new Header() { Version = ui.ReadInt16() };
			var shorts = new List<short>();

			for (int i = 0; i < 5; i++)
				shorts.Add(ui.ReadInt16());

			header.Unknown = shorts.ToArray();

			var count = ui.ReadUInt32();

			for (int i = 0; i < count; i++)
			{
				var length = ui.ReadUInt16();
				string name = Encoding.ASCII.GetString(ui.ReadBytes(length), 0, length);
				textures.Add(new Texture(name, ui.ReadUInt32(), ui.ReadUInt32()));
			}

			count = ui.ReadUInt32();

			for (int i = 0; i < count; i++)
				unknowns.Add(ui.ReadInt32());

			count = ui.ReadUInt32();

			for (int i = 0; i < count; i++)
				images.Add(new ImageBit(ui.ReadInt32(),
										new Point(ui.ReadInt32(), ui.ReadInt32()),
										ui.ReadInt32(),
										ui.ReadInt32()));

			count = ui.ReadUInt32();

			for (int i = 0; i < count; i++)
			{
				var unknown = ui.ReadUInt32();

				var length = ui.ReadUInt16();
				string name1 = Encoding.ASCII.GetString(ui.ReadBytes(length), 0, length);

				length = ui.ReadUInt16();
				string name2 = Encoding.ASCII.GetString(ui.ReadBytes(length), 0, length);

				elements.Add(new ElementPair() { Unknown = unknown, Element1 = name1, Element2 = name2 });
			}

			count = ui.ReadUInt32();

			for (int i = 0; i < count; i++)
			{
				var text = new Text();

				var length = ui.ReadUInt16();
				text.Name = Encoding.ASCII.GetString(ui.ReadBytes(length), 0, length);
				text.ID = ui.ReadInt32();
				text.Location = new Point(ui.ReadInt32(), ui.ReadInt32());
				text.Width = ui.ReadInt32();
				text.LineHeight = ui.ReadInt32();
				text.unk1 = ui.ReadInt32();
				text.NormalColour = (Colour)ui.ReadBytes(4).ToStructure<Colour>();
				text.HoverColour = (Colour)ui.ReadBytes(4).ToStructure<Colour>();
				text.PressedColour = (Colour)ui.ReadBytes(4).ToStructure<Colour>();
				text.Dock = (DockingMode)ui.ReadByte();
				text.unk2 = ui.ReadByte();
				text.unk3 = ui.ReadInt16();
				text.DTSIndex = ui.ReadInt32();

				texts.Add(text);
			}

			count = ui.ReadUInt32();

			for (int i = 0; i < count; i++)
			{
				var control = new UIControl();

				var length = ui.ReadUInt16();
				control.Name = Encoding.ASCII.GetString(ui.ReadBytes(length), 0, length);
				length = ui.ReadUInt16();
				control.ParentName = Encoding.ASCII.GetString(ui.ReadBytes(length), 0, length);
				length = ui.ReadUInt16();
				control.Description = Encoding.GetEncoding(949).GetString(ui.ReadBytes(length), 0, length); // Korean
				control.unk0 = ui.ReadInt32();
				control.ID = ui.ReadInt32();
				control.ParentID = ui.ReadInt32();
				control.Style = ui.ReadInt32();
				control.Rectangle = new Rect(ui.ReadInt32(), ui.ReadInt32(), ui.ReadInt32(), ui.ReadInt32());
				control.unk1 = ui.ReadInt32();
				control.unk2 = ui.ReadInt32();
				control.unk3 = ui.ReadInt32();
				control.unk4 = ui.ReadInt32();
				control.unk5 = ui.ReadInt32();
				control.ElementIndex = ui.ReadInt32();
				control.ElementState = (State)ui.ReadInt32();
				control.ImageIndex = ui.ReadInt32();
				control.ImageState = (State)ui.ReadInt32();
				control.TextIndex = ui.ReadInt32();
				control.TextLines = ui.ReadInt32();
				control.Dock = (DockingMode)ui.ReadInt32();

				controls.Add(control);
			}

			ui.Close();
			
			// Get current client language
			var mainex = new BinaryReader(File.OpenRead(dir + "\\..\\mainex.dat"));
			
			mainex.BaseStream.Seek(0x75, SeekOrigin.Begin);
			language = (Language)mainex.ReadInt32();
			
			mainex.Dispose();
			                           
			TextReader reader = File.OpenText(dir + "\\language\\" + language.ToString() + "\\ui.dts");

			string line;

			for (int i = 0; (line = reader.ReadLine()) != null; i++ )
			{
				line = line.Substring(line.IndexOf('@') + 2);
				dts.Add(line);
			}

			reader.Close();
		}

		void SaveUI()
		{
			var file = File.Create(dir + "\\ui.dat");
			var writer = new BinaryWriter(file);

			// Header
			writer.Write(header.Version);
			foreach (var u in header.Unknown)
				writer.Write(u);

			// Textures
			writer.Write(textures.Count);
			foreach (var t in textures)
				t.Write(writer);

			// Unknowns
			writer.Write(unknowns.Count);
			foreach (var u in unknowns)
				writer.Write(u);

			// Image Bits
			writer.Write(images.Count);
			foreach (var i in images)
				i.Write(writer);

			// Elements
			writer.Write(elements.Count);
			foreach (var e in elements)
				e.Write(writer);

			// Texts
			writer.Write(texts.Count);
			foreach (var t in texts)
				t.Write(writer);

			// Controls
			writer.Write(controls.Count);
			foreach (var c in controls)
				c.Write(writer);

			writer.Close();
			file.Close();

			var saved = new System.Media.SoundPlayer(Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Sounds/saved.wav")).Stream);
			saved.PlaySync();
			saved.Dispose();
		}

		void AddControl(UIControl control)
		{
			if (control.ImageIndex >= images.Count)
				return;
			
			var imagebit = images[control.ImageIndex];
			var texture = textures[(int)imagebit.TextureIndex];
			var text = (control.TextIndex < texts.Count) ? texts[control.TextIndex] : null;

			control.Content = text;
			var image = new Image();

			image.Stretch = Stretch.Fill;
			image.SetBinding(Image.WidthProperty, CreateTwoWayBinding("Width", control));
			image.SetBinding(Image.HeightProperty, CreateTwoWayBinding("Height", control));

			var matrix = image.RenderTransform.Value;

			switch (control.Dock)
			{
				// TBD

				/*case DockingMode.CenterCenter:
					matrix.OffsetX = (canvas.ActualWidth - image.Width) / 2;
					matrix.OffsetY = (canvas.ActualHeight - image.Height) / 2;
					break;*/

				default:
					matrix.OffsetX = control.Rectangle.Left;
					matrix.OffsetY = control.Rectangle.Top;
					break;
			}

			image.SetBinding(Image.RenderTransformProperty, CreateTwoWayBinding("Rectangle", control, new RectConverter()));

			if (control.ImageState != State.Hidden)
			{
				/*var bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.UriSource = new Uri(dir + '\\' + texture.Name);
				bitmap.DecodePixelWidth = (int)texture.Width;
				bitmap.DecodePixelHeight = (int)texture.Height;
				bitmap.EndInit();*/
				
				var bitmap = (BitmapSource)DDSConverter.Convert(dir + '\\' + texture.Name);

				var rect = new Int32Rect(imagebit.TopLeft.X, imagebit.TopLeft.Y, imagebit.Width, imagebit.Height);

				if (rect.X + rect.Width > texture.Width)
					rect.Width = (int)texture.Width - rect.X;
				if (rect.Y + rect.Height > texture.Height)
					rect.Height = (int)texture.Height - rect.Y;

				image.Source = new CroppedBitmap(bitmap, rect);
				
				bitmap = null;

				#region Memory Leak!

				/*var write = new WriteableBitmap(bitmap);

				write.Lock();

				if (rect.Height != 0 && rect.Width != 0)
				{
					if (rect.X + rect.Width > write.PixelWidth)
					{
						var pixels = new byte[write.PixelHeight * write.PixelWidth * write.Format.BitsPerPixel];

						write.CopyPixels(pixels, write.BackBufferStride, 0);
						write = new WriteableBitmap(write.PixelWidth * 2, write.PixelHeight, write.DpiX, write.DpiY, write.Format, write.Palette);
						write.WritePixels(new Int32Rect(0, 0, write.PixelWidth, write.PixelHeight), pixels, write.BackBufferStride, 0);
						write.WritePixels(new Int32Rect(write.PixelWidth, 0, write.PixelWidth, write.PixelHeight), pixels, write.BackBufferStride, 0);

						pixels = null;
					}

					if (rect.Y + rect.Height > write.PixelHeight)
					{
						var pixels = new byte[write.PixelHeight * write.PixelWidth * write.Format.BitsPerPixel];

						var blah = new BitmapImage();

						write.CopyPixels(pixels, write.BackBufferStride, 0);
						write = new WriteableBitmap(write.PixelWidth, write.PixelHeight * 2, write.DpiX, write.DpiY, write.Format, write.Palette);
						write.WritePixels(new Int32Rect(0, 0, write.PixelWidth, write.PixelHeight / 2), pixels, write.BackBufferStride, 0);
						write.WritePixels(new Int32Rect(0, write.PixelHeight / 2, write.PixelWidth, write.PixelHeight / 2), pixels, write.BackBufferStride, 0);

						pixels = null;
					}

					image.Source = new CroppedBitmap(write, rect);
				}

				write.Unlock();

				bitmap = null;
				write = null;
				
				GC.Collect();
				GC.WaitForPendingFinalizers();*/

				#endregion
			}

			var behaviours = Interaction.GetBehaviors(image);

			var resize = new ResizeBehavior();
			resize.MaxWidth = 1024;
			resize.MaxHeight = 768;
			resize.MinWidth = 2;
			resize.MinHeight = 2;
			resize.StayInParent = true;
			resize.DragSpace = 4;
			resize.DragIndicatorsOpacity = 0.75;
			resize.DragIndicatorsFill = Brushes.White;
			behaviours.Add(resize);

			var drag = new MouseDragElementBehavior();
			drag.ConstrainToParentBounds = true;
			behaviours.Add(drag);

			image.MouseDown += control_MouseDown;

			image.Tag = control;

			canvas.Children.Add(image);

			if (text == null || text.DTSIndex == -1 || control.TextLines == 0) return;

			var stack = new StackPanel();
			stack.SetBinding(StackPanel.RenderTransformProperty, CreateTwoWayBinding("Location", text, new PointConverter()));

			var lineWidthBinding = CreateTwoWayBinding("Width", text);
			var lineHeightBinding = CreateTwoWayBinding("LineHeight", text);
			var hAlignBinding = CreateTwoWayBinding("HAlign", text);
			var vAlignBinding = CreateTwoWayBinding("VAlign", text);

			var brush = new SolidColorBrush();
			BindingOperations.SetBinding(brush, SolidColorBrush.ColorProperty, CreateTwoWayBinding("NormalColour", text, new ColourConverter()));

			for (int i = 0; i < control.TextLines; i++)
			{
				var label = new Label();

				var contentBinding = new Binding();
				contentBinding.Source = dts[text.DTSIndex + i];
				contentBinding.Mode = BindingMode.OneWay;
				contentBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

				label.SetBinding(Label.ContentProperty, contentBinding);

				label.Foreground = brush;

				label.SetBinding(Label.WidthProperty, lineWidthBinding);
				label.SetBinding(Label.HeightProperty, lineHeightBinding);

				label.SetBinding(Label.HorizontalContentAlignmentProperty, hAlignBinding);
				label.SetBinding(Label.VerticalContentAlignmentProperty, vAlignBinding);

				label.IsHitTestVisible = false;
				label.FontFamily = new FontFamily("Tahoma");
				label.FontSize = 11.0d;
				label.Padding = new Thickness(0);

				label.Tag = image;

				stack.Children.Add(label);
			}

			canvas.Children.Add(stack);
		}

		Binding CreateTwoWayBinding(string path, object source)
			{ return CreateTwoWayBinding(path, source, null, null); }

		Binding CreateTwoWayBinding(string path, object source, IValueConverter converter)
			{ return CreateTwoWayBinding(path, source, converter, null); }

		Binding CreateTwoWayBinding(string path, object source, IValueConverter converter, object parameter)
		{
			var result = new Binding(path);
			result.Source = source;
			result.Converter = converter;
			result.ConverterParameter = parameter;
			result.Mode = BindingMode.TwoWay;
			result.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

			return result;
		}
	}
}