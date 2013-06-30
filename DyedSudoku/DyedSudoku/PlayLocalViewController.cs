using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Utility
{
	public partial class PlayLocalViewController : UIViewController
	{
        public PlayLocalViewController () : base ("PlayLocalViewController", null)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

        partial void done (MonoTouch.Foundation.NSObject sender)
		{
			if (Done != null)
				Done (this, EventArgs.Empty);
		}

		public event EventHandler Done;
	}
}

