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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using System.ComponentModel;

#endregion

namespace Behaviors
{
	public class ResizeBehavior : Behavior<FrameworkElement>
	{
		private enum ControlParts
		{
			None,
			Left,
			Right,
			Top,
			Bottom,
			LeftTop,
			LeftBottom,
			RightTop,
			RightBottom,
		}

		bool isResizing;
		ControlParts resizePart;

		#region  Properties

		#region Size Properties

		#region MinHeight

		public static readonly DependencyProperty MinHeightProperty =
			DependencyProperty.Register("MinHeight", typeof(Double), typeof(ResizeBehavior),
			new PropertyMetadata(15.0, OnSizeChanged));

		/// <summary>
		/// Set the minimum resize height
		/// </summary>
		[Category("Size"), Description("Set the minimum resize height")]
		public double MinHeight
		{
			get { return (double)GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}

		#endregion

		#region MaxHeight

		public static readonly DependencyProperty MaxHeightProperty =
			DependencyProperty.Register("MaxHeight", typeof(Double), typeof(ResizeBehavior),
			new PropertyMetadata(400.0, OnSizeChanged));

		/// <summary>
		/// Set the maximum resize height
		/// </summary>
		[Category("Size"), Description("Set the maximum resize height")]
		public double MaxHeight
		{
			get { return (double)GetValue(MaxHeightProperty); }
			set { SetValue(MaxHeightProperty, value); }
		}

		#endregion

		#region MinWidth

		public static readonly DependencyProperty MinWidthProperty =
			DependencyProperty.Register("MinWidth", typeof(Double), typeof(ResizeBehavior),
			new PropertyMetadata(15.0, OnSizeChanged));

		/// <summary>
		/// Set the minimum resize width
		/// </summary>
		[Category("Size"), Description("Set the minimum resize width")]
		public double MinWidth
		{
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}

		#endregion

		#region MaxWidth

		public static readonly DependencyProperty MaxWidthProperty =
			DependencyProperty.Register("MaxWidth", typeof(Double), typeof(ResizeBehavior),
			new PropertyMetadata(400.0, OnSizeChanged));

		/// <summary>
		/// Set the maximum resize width
		/// </summary>
		[Category("Size"), Description("Set the maximum resize width")]
		public double MaxWidth
		{
			get { return (double)GetValue(MaxWidthProperty); }
			set { SetValue(MaxWidthProperty, value); }
		}

		#endregion

		private static void OnSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ResizeBehavior source = (ResizeBehavior)obj;

			source.checkSizeValues();
		}

		#endregion

		#region Appareance Properties

		#region DragSpace

		public static readonly DependencyProperty DragSpaceProperty =
			DependencyProperty.Register("DragSpace", typeof(double), typeof(ResizeBehavior),
			new PropertyMetadata(7.0, OnAppareanceElementChanged));

		/// <summary>
		/// Set the space from border for start resize mode
		/// </summary>
		[Category("Appareance"), Description("Set the space from border for start resize mode")]
		public double DragSpace
		{
			get { return (double)GetValue(DragSpaceProperty); }
			set { SetValue(DragSpaceProperty, value); }
		}



		#endregion

		#region DragIndicatorsFill

		public static readonly DependencyProperty DragIndicatorsFillProperty =
			DependencyProperty.Register("DragIndicatorsFill", typeof(Brush), typeof(ResizeBehavior),
			new PropertyMetadata(new SolidColorBrush(Colors.Yellow), OnAppareanceElementChanged));

		/// <summary>
		/// Set the drags indicator BackGround
		/// </summary>
		[Category("Appareance"), Description("Set the drags indicator BackGround")]
		public Brush DragIndicatorsFill
		{
			get { return (Brush)GetValue(DragIndicatorsFillProperty); }
			set { SetValue(DragIndicatorsFillProperty, value); }
		}

		#endregion

		#region DragIndicatorsOpacity

		public static readonly DependencyProperty DragIndicatorsOpacityProperty =
			DependencyProperty.Register("DragIndicatorsOpacity", typeof(double), typeof(ResizeBehavior),
			new PropertyMetadata(0.8, OnAppareanceElementChanged));

		/// <summary>
		/// Set the drags indicator Opacity
		/// </summary>
		[Category("Appareance"), Description("Set the drags indicators Opacity")]
		public Double DragIndicatorsOpacity
		{
			get { return (Double)GetValue(DragIndicatorsOpacityProperty); }
			set { SetValue(DragIndicatorsOpacityProperty, value); }
		}

		#endregion


		private static void OnAppareanceElementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{

			ResizeBehavior source = (ResizeBehavior)obj;

			if (source.AssociatedObject == null) return;

			source.setRectangle(source.resizePart);
		}

		#endregion

		#region Comportment

		#region IsLeftDraggable

		public static readonly DependencyProperty IsLeftDraggableProperty =
			DependencyProperty.Register("IsLeftDraggable", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true));

		/// <summary>
		/// Indicate if is possible resize dragging the left part
		/// </summary>
		[Category("Comportment"), Description("Indicate if is possible resize dragging the left part")]
		public bool IsLeftDraggable
		{
			get { return (bool)GetValue(IsLeftDraggableProperty); }
			set { SetValue(IsLeftDraggableProperty, value); }
		}

		#endregion

		#region IsRightDraggable

		public static readonly DependencyProperty IsRightDraggableProperty =
			DependencyProperty.Register("IsRightDraggable", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true));

		/// <summary>
		/// Indicate if is possible resize dragging the right part
		/// </summary>
		[Category("Comportment"), Description("Indicate if is possible resize dragging the right part")]
		public bool IsRightDraggable
		{
			get { return (bool)GetValue(IsRightDraggableProperty); }
			set { SetValue(IsRightDraggableProperty, value); }
		}

		#endregion

		#region IsTopDraggable

		public static readonly DependencyProperty IsTopDraggableProperty =
			DependencyProperty.Register("IsTopDraggable", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true));

		/// <summary>
		/// Indicate if is possible resize dragging the top part
		/// </summary>
		[Category("Comportment"), Description("Indicate if is possible resize dragging the top part")]
		public bool IsTopDraggable
		{
			get { return (bool)GetValue(IsTopDraggableProperty); }
			set { SetValue(IsTopDraggableProperty, value); }
		}

		#endregion

		#region IsBottomDraggable

		public static readonly DependencyProperty IsBottomDraggableProperty =
			DependencyProperty.Register("IsBottomDraggable", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true));

		/// <summary>
		/// Indicate if is possible resize dragging the bottom part
		/// </summary>
		[Category("Comportment"), Description("Indicate if is possible resize dragging the bottom part")]
		public bool IsBottomDraggable
		{
			get { return (bool)GetValue(IsBottomDraggableProperty); }
			set { SetValue(IsBottomDraggableProperty, value); }
		}

		#endregion

		#region IsTopLeftDraggable

		public static readonly DependencyProperty IsTopLeftDraggableProperty =
			DependencyProperty.Register("IsTopLeftDraggable", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true));

		/// <summary>
		/// Indicate if is possible resize dragging the top left corner
		/// </summary>
		[Category("Comportment"), Description("Indicate if is possible resize dragging the top left corner")]
		public bool IsTopLeftDraggable
		{
			get { return (bool)GetValue(IsTopLeftDraggableProperty); }
			set { SetValue(IsTopLeftDraggableProperty, value); }
		}

		#endregion

		#region IsTopRightDraggable

		public static readonly DependencyProperty IsTopRightDraggableProperty =
			DependencyProperty.Register("IsTopRightDraggable", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true));

		/// <summary>
		/// Indicate if is possible resize dragging the top right corner
		/// </summary>
		[Category("Comportment"), Description("Indicate if is possible resize dragging the top right corner")]
		public bool IsTopRightDraggable
		{
			get { return (bool)GetValue(IsTopRightDraggableProperty); }
			set { SetValue(IsTopRightDraggableProperty, value); }
		}

		#endregion

		#region IsBottomLeftDraggable

		public static readonly DependencyProperty IsBottomLeftDraggableProperty =
			DependencyProperty.Register("IsBottomLeftDraggable", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true));

		/// <summary>
		/// Indicate if is possible resize dragging the bottom left corner
		/// </summary>
		[Category("Comportment"), Description("Indicate if is possible resize dragging the bottom left corner")]
		public bool IsBottomLeftDraggable
		{
			get { return (bool)GetValue(IsBottomLeftDraggableProperty); }
			set { SetValue(IsBottomLeftDraggableProperty, value); }
		}

		#endregion

		#region IsBottomRightDraggable

		public static readonly DependencyProperty IsBottomRightDraggableProperty =
			DependencyProperty.Register("IsBottomRightDraggable", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true));

		/// <summary>
		/// Indicate if is possible resize dragging the bottom right corner
		/// </summary>
		[Category("Comportment"), Description("Indicate if is possible resize dragging the bottom right corner")]
		public bool IsBottomRightDraggable
		{
			get { return (bool)GetValue(IsBottomRightDraggableProperty); }
			set { SetValue(IsBottomRightDraggableProperty, value); }
		}

		#endregion


		#region StayInParent

		public static readonly DependencyProperty StayInParentProperty =
			DependencyProperty.Register("StayInParent", typeof(bool), typeof(ResizeBehavior),
			new PropertyMetadata(true, StayInParentPropertyChanged));

		/// <summary>
		/// Indicate if the element stay or not into parent bounds
		/// </summary>
		[Category("Comportment"), Description("Indicate if the element stay or not into parent bounds")]
		public bool StayInParent
		{
			get { return (bool)GetValue(StayInParentProperty); }
			set { SetValue(StayInParentProperty, value); }
		}


		private static void StayInParentPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ResizeBehavior source = (ResizeBehavior)obj;

			source.checkParentBounds();
		}

		#endregion

		#endregion

		#endregion

		#region Events

		/// <summary>Expose this event because when size is changed by behavior the FrameworkElement.SizeChanged event is not raised.</summary>
		[Category("Size"), Description("Expose this event because when size is changed by behavior the FrameworkElement.SizeChanged event is not raised")]
		public event EventHandler SizeChanged;

		private void raiseEvent(EventHandler e)
		{
			raiseEvent(e, EventArgs.Empty);
		}

		private void raiseEvent(EventHandler e, EventArgs args)
		{
			if (e != null) e(this, args);
		}

		#endregion

		public ResizeBehavior()
		{
		}

		#region behaviour elements implementation

		protected override void OnAttached()
		{
			base.OnAttached();

			isResizing = false;
			resizePart = ControlParts.None;
			this.setRectangle(ControlParts.None);

			this.checkSizeValues();
			this.checkParentBounds();

			this.AssociatedObject.MouseLeftButtonDown += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
			this.AssociatedObject.MouseLeftButtonUp += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp);
			this.AssociatedObject.MouseMove += new MouseEventHandler(AssociatedObject_MouseMove);
			this.AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);

			// bind to this event because is possible retrive the parent only when associated object is loaded
			this.AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);

		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			this.AssociatedObject.MouseLeftButtonDown -= new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
			this.AssociatedObject.MouseLeftButtonUp -= new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp);
			this.AssociatedObject.MouseMove -= new MouseEventHandler(AssociatedObject_MouseMove);
			this.AssociatedObject.MouseLeave -= new MouseEventHandler(AssociatedObject_MouseLeave);
		}

		#endregion

		#region Associated object events

		void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
		{
			if (isResizing)
			{
				resize(e.GetPosition(this.AssociatedObject), e.GetPosition((FrameworkElement)this.AssociatedObject.Parent));
				setRectangle(this.resizePart);
			}
			else
			{
				setRectangle(this.getResizePart(e.GetPosition(this.AssociatedObject)));
				setCursor(this.getResizePart(e.GetPosition(this.AssociatedObject)));
			}
		}

		void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			this.AssociatedObject.ReleaseMouseCapture();
			this.isResizing = false;
			this.resizePart = ControlParts.None;
			this.setRectangle(ControlParts.None);
		}

		void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.AssociatedObject.CaptureMouse();
			this.resizePart = getResizePart(e.GetPosition(this.AssociatedObject));
			this.isResizing = (this.resizePart == ControlParts.None) ? false : true;

			e.Handled = this.isResizing;
		}

		void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
		{
			this.setRectangle(ControlParts.None);
		}


		void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
		{
			this.AssociatedObject.Loaded -= new RoutedEventHandler(AssociatedObject_Loaded);

			BehaviorCollection parent_attached_behaviors = Interaction.GetBehaviors(this.AssociatedObject.Parent);

			ResizeBehavior parent_resize_behavior = null;

			foreach(Behavior b in parent_attached_behaviors)
			{
				if (b.GetType()==typeof(ResizeBehavior))
				{
					parent_resize_behavior = (ResizeBehavior)b;
					break;
				}
			}

			if (parent_resize_behavior == null)
			(this.AssociatedObject.Parent as FrameworkElement).SizeChanged+=new SizeChangedEventHandler(ParentSizeChanged);
			
			else
				//attach to parent's resize behavior to notice size changes
				parent_resize_behavior.SizeChanged += new EventHandler(ParentBehaviorSizeChanged);
			

		}

		#endregion

		#region Associated object parent events

		void ParentSizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.checkParentBounds();
		}

		void ParentBehaviorSizeChanged(object sender, EventArgs e)
		{
			this.checkParentBounds();
		}

		#endregion

		#region resizing functions

		ControlParts getResizePart(Point mousePosition)
		{
			Size size = new Size(
				this.AssociatedObject.ActualWidth,
				this.AssociatedObject.ActualHeight);

			Point center = new Point(size.Width / 2, size.Height / 2);

			if (mousePosition.X > center.X && mousePosition.X > size.Width - this.DragSpace &&
				mousePosition.Y < center.Y && mousePosition.Y < this.DragSpace &&
				this.IsTopRightDraggable)
				return ControlParts.RightTop;

			if (mousePosition.X > center.X && mousePosition.X > size.Width - this.DragSpace &&
				mousePosition.Y > center.Y && mousePosition.Y > size.Height - this.DragSpace &&
				this.IsBottomRightDraggable)
				return ControlParts.RightBottom;

			if (mousePosition.X < center.X && mousePosition.X < this.DragSpace &&
				mousePosition.Y < center.Y && mousePosition.Y < this.DragSpace &&
				this.IsTopLeftDraggable)
				return ControlParts.LeftTop;

			if (mousePosition.X < center.X && mousePosition.X < this.DragSpace &&
				mousePosition.Y > center.Y && mousePosition.Y > size.Height - this.DragSpace &&
				this.IsBottomLeftDraggable)
				return ControlParts.LeftBottom;

			if (mousePosition.X > center.X && mousePosition.X > size.Width - this.DragSpace &&
				this.IsRightDraggable)
				return ControlParts.Right;

			if (mousePosition.X < center.X && mousePosition.X < this.DragSpace && this.IsLeftDraggable)
				return ControlParts.Left;

			if (mousePosition.Y < center.Y && mousePosition.Y < this.DragSpace && this.IsTopDraggable)
				return ControlParts.Top;

			if (mousePosition.Y > center.Y && mousePosition.Y > size.Height - this.DragSpace && this.IsBottomDraggable)
				return ControlParts.Bottom;

			return ControlParts.None;
		}

		void setCursor(ControlParts part)
		{
			switch (part)
			{
				case ControlParts.None:
					this.AssociatedObject.Cursor = Cursors.Arrow;
					break;

				case ControlParts.Top:
				case ControlParts.Bottom:
					this.AssociatedObject.Cursor = Cursors.SizeNS;
					break;

				case ControlParts.LeftBottom:
				case ControlParts.RightTop:
					this.AssociatedObject.Cursor = Cursors.SizeNESW;
					break;

				case ControlParts.LeftTop:
				case ControlParts.RightBottom:
					this.AssociatedObject.Cursor = Cursors.SizeNWSE;
					break;

				case ControlParts.Right:
				case ControlParts.Left:
					this.AssociatedObject.Cursor = Cursors.SizeWE;
					break;
			}

		}

		void setRectangle(ControlParts part)
		{
			Canvas parent = this.AssociatedObject.Parent as Canvas;

			if (parent == null) return;

			for (int i = 0; i < parent.Children.Count; i++)
			{
				FrameworkElement e = parent.Children[i] as FrameworkElement;

				if (e == null) continue;

				if (e.Name == "__RESIZE_AUTO_GEN_RECTANGLE" + this.GetHashCode())
				{
					e.Visibility = Visibility.Collapsed;
					parent.Children.RemoveAt(i);
					break;
				}
			}

			Rectangle rect = new Rectangle();

			rect.Name = "__RESIZE_AUTO_GEN_RECTANGLE" + this.GetHashCode();
			rect.Fill = this.DragIndicatorsFill;
			rect.Opacity = this.DragIndicatorsOpacity;
			rect.SetValue(Canvas.ZIndexProperty, (int)this.AssociatedObject.GetValue(Canvas.ZIndexProperty) + 1);
			rect.IsHitTestVisible = false;
			//rect.SetValue(Canvas.RenderTransformProperty, this.AssociatedObject.GetValue(Canvas.RenderTransformProperty));

			var x = this.AssociatedObject.RenderTransform.Value.OffsetX;
			var y = this.AssociatedObject.RenderTransform.Value.OffsetY;

			switch (part)
			{
				case ControlParts.None:
					rect = null;
					break;

				case ControlParts.Top:
					rect.Width = this.AssociatedObject.ActualWidth;
					rect.Height = this.DragSpace;
					rect.SetValue(Canvas.TopProperty, y);
					rect.SetValue(Canvas.LeftProperty, x);
					break;

				case ControlParts.Bottom:
					rect.Width = this.AssociatedObject.ActualWidth;
					rect.Height = this.DragSpace;
					rect.SetValue(Canvas.TopProperty, this.AssociatedObject.ActualHeight - this.DragSpace + y);
					rect.SetValue(Canvas.LeftProperty, x);
					break;

				case ControlParts.Right:
					rect.Width = this.DragSpace;
					rect.Height = this.AssociatedObject.ActualHeight;
					rect.SetValue(Canvas.TopProperty, y);
					rect.SetValue(Canvas.LeftProperty, this.AssociatedObject.ActualWidth - this.DragSpace + x);
					break;

				case ControlParts.Left:
					rect.Width = this.DragSpace;
					rect.Height = this.AssociatedObject.ActualHeight;
					rect.SetValue(Canvas.TopProperty, y);
					rect.SetValue(Canvas.LeftProperty, x);
					break;

				case ControlParts.RightTop:
					rect.Width = this.DragSpace * 2;
					rect.Height = this.DragSpace * 2;
					rect.SetValue(Canvas.TopProperty, y);
					rect.SetValue(Canvas.LeftProperty, this.AssociatedObject.ActualWidth - rect.Width + x);
					break;

				case ControlParts.RightBottom:
					rect.Width = this.DragSpace * 2;
					rect.Height = this.DragSpace * 2;
					rect.SetValue(Canvas.TopProperty, this.AssociatedObject.ActualHeight - rect.Height + y);
					rect.SetValue(Canvas.LeftProperty, this.AssociatedObject.ActualWidth - rect.Width + x);
					break;

				case ControlParts.LeftBottom:
					rect.Width = this.DragSpace * 2;
					rect.Height = this.DragSpace * 2;
					rect.SetValue(Canvas.TopProperty, this.AssociatedObject.ActualHeight - rect.Height + y);
					rect.SetValue(Canvas.LeftProperty, x);
					break;

				case ControlParts.LeftTop:
					rect.Width = this.DragSpace * 2;
					rect.Height = this.DragSpace * 2;
					rect.SetValue(Canvas.TopProperty, y);
					rect.SetValue(Canvas.LeftProperty, x);
					break;
			}

			if (rect == null) return;

			parent.Children.Add(rect);
		}

		void resize(Point insideMousePosition, Point parentMousePosition)
		{
			double newHeight = this.AssociatedObject.ActualHeight;
			double newWidth = this.AssociatedObject.ActualWidth;

			Point newPosition = new Point(
				(double)this.AssociatedObject.RenderTransform.Value.OffsetX,
				(double)this.AssociatedObject.RenderTransform.Value.OffsetY);

			switch (this.resizePart)
			{
				case ControlParts.RightTop:
					newWidth = insideMousePosition.X;
					newHeight -= parentMousePosition.Y - newPosition.Y;
					newPosition.Y = parentMousePosition.Y;
					break;

				case ControlParts.RightBottom:
					newWidth = insideMousePosition.X;
					newHeight = insideMousePosition.Y;
					break;

				case ControlParts.LeftTop:
					newWidth -= parentMousePosition.X - newPosition.X;
					newPosition.X = parentMousePosition.X;
					newHeight -= parentMousePosition.Y - newPosition.Y;
					newPosition.Y = parentMousePosition.Y;
					break;

				case ControlParts.LeftBottom:
					newWidth -= parentMousePosition.X - newPosition.X;
					newPosition.X = parentMousePosition.X;
					newHeight = insideMousePosition.Y;
					break;

				case ControlParts.Right:
					newWidth = insideMousePosition.X;
					break;

				case ControlParts.Left:
					newWidth += newPosition.X - parentMousePosition.X;
					newPosition.X = parentMousePosition.X;
					break;

				case ControlParts.Top:
					newHeight -= parentMousePosition.Y - newPosition.Y;
					newPosition.Y = parentMousePosition.Y;
					break;

				case ControlParts.Bottom:
					newHeight = insideMousePosition.Y;
					break;
			}

			bool has_changes = false;

			if (newWidth >= this.MinWidth && newWidth <= this.MaxWidth)
			{
				this.AssociatedObject.Width = newWidth;
				//this.AssociatedObject.SetValue(Canvas.LeftProperty, newPosition.X);

				var matrix = AssociatedObject.RenderTransform.Value;
				matrix.OffsetX = newPosition.X;

				AssociatedObject.RenderTransform = new MatrixTransform(matrix);

				has_changes = true;
			}

			if (newHeight >= this.MinHeight && newHeight <= this.MaxHeight)
			{
				this.AssociatedObject.Height = newHeight;

				var matrix = AssociatedObject.RenderTransform.Value;
				matrix.OffsetY = newPosition.Y;

				AssociatedObject.RenderTransform = new MatrixTransform(matrix);

				has_changes = true;
			}

			// call this to notify size change
			if (has_changes)
				raiseEvent(this.SizeChanged);

			checkParentBounds();
		}

		#endregion

		#region check functions

		void checkParentBounds()
		{
			if (!this.StayInParent || this.AssociatedObject == null) return;

			Canvas parent = this.AssociatedObject.Parent as Canvas;

			// check if parent exist and is a canvas
			if (parent == null) return;

			bool has_changes = false;

			try
			{
				if (this.AssociatedObject.RenderTransform.Value.OffsetX < 0)
				{
					double prv_left = this.AssociatedObject.RenderTransform.Value.OffsetX;

					var matrix = AssociatedObject.RenderTransform.Value;
					matrix.OffsetX = 0;

					AssociatedObject.RenderTransform.SetValue(MatrixTransform.MatrixProperty, matrix);

					this.AssociatedObject.Width += prv_left;
					has_changes = true;
				}

				if (this.AssociatedObject.RenderTransform.Value.OffsetY < 0)
				{
					double prv_top = this.AssociatedObject.RenderTransform.Value.OffsetY;

					var matrix = AssociatedObject.RenderTransform.Value;
					matrix.OffsetY = 0;

					AssociatedObject.RenderTransform.SetValue(MatrixTransform.MatrixProperty, matrix);

					this.AssociatedObject.Height += prv_top;
					has_changes = true;
				}

				if (this.AssociatedObject.Width + this.AssociatedObject.RenderTransform.Value.OffsetX > parent.ActualWidth)
				{
					if (parent.ActualWidth == 0) return;

					this.AssociatedObject.Width = parent.ActualWidth - this.AssociatedObject.RenderTransform.Value.OffsetX;
					has_changes = true;
				}

				if (this.AssociatedObject.Height + this.AssociatedObject.RenderTransform.Value.OffsetY> parent.ActualHeight)
				{
					if (parent.ActualHeight == 0) return;

					this.AssociatedObject.Height = parent.ActualHeight - this.AssociatedObject.RenderTransform.Value.OffsetY;
					has_changes = true;
				}

			}
			catch (Exception)
			{
				has_changes = false;
			}

			// call this to notify size change
			if (has_changes)
				raiseEvent(this.SizeChanged);

		}

		void checkSizeValues()
		{
			FrameworkElement element = this.AssociatedObject;

			if (element == null) return;

			bool has_changes = false;

			if (element.Width > this.MaxWidth)
			{
				element.Width = this.MaxWidth;
				has_changes = true;
			}

			if (element.Width < this.MinWidth)
			{
				element.Width = this.MinWidth;
				has_changes = true;
			}

			if (element.Height > this.MaxHeight)
			{
				element.Height = this.MaxHeight;
				has_changes = true;
			}

			if (element.Height < this.MinHeight)
			{
				element.Height = this.MinHeight;
				has_changes = true;
			}

			// call this to notify size change
			if (has_changes)
				raiseEvent(this.SizeChanged);
		}

		#endregion

	}
}