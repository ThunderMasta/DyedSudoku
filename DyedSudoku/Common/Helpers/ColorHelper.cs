using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;

namespace Common
{
	public static class ColorHelper
    {
		public static CGColor Red
		{
			get { return new CGColor(((float)0xff/0xff), ((float)0x66/0xff), ((float)0x66/0xff), ((float)0xff/0xff)); }
		}

		public static CGColor Green
		{
			get { return new CGColor(((float)0x33/0xff), ((float)0xff/0xff), ((float)0x66/0xff), ((float)0xff/0xff)); }
		}

		public static CGColor Blue
		{
			get { return new CGColor(((float)0x00/0xff), ((float)0x99/0xff), ((float)0xff/0xff), ((float)0xff/0xff)); }
		}

		public static CGColor Orange
		{
			get { return UIColor.Orange.CGColor; }
		}

		public static CGColor White
		{
			get { return UIColor.White.CGColor; }
		}

		public static CGColor Black
		{
			get { return UIColor.Black.CGColor; }
		}
    }
}

