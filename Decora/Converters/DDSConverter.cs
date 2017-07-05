/*
 * Date: 01/09/2014
 * Time: 16:13
 */

using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

using DevIL;

namespace Decora
{
	public class DDSConverter : IValueConverter
	{

		[DllImport("gdi32")]
		static extern int DeleteObject(IntPtr o);
   
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			else if (value is string)
			{
				return DDSConverter.Convert((string)value);
			}
			else
			{
				throw new NotSupportedException(string.Format("{0} cannot convert from {1}.", this.GetType().FullName, value.GetType().FullName));
			}
		}
	
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException(string.Format("{0} does not support converting back.", this.GetType().FullName));
		}
	
		public static ImageSource Convert(string filePath)
		{
			Bitmap source = DevIL.DevIL.LoadBitmap(filePath);
			//var source = new Bitmap(1024, 1024);
			
			IntPtr ip = source.GetHbitmap();
			BitmapSource bs = null;
			
			bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, 
					IntPtr.Zero, Int32Rect.Empty, 
					System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
			
			source.Dispose();
			DeleteObject(ip);
			
			source = null;
	
			return bs;
		}
	}
}