// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace DyedSudoku
{
	[Register ("FlipsideViewController")]
	partial class PlayLocalViewController
	{
		[Outlet]
		GameFieldView gameFieldView { get; set; }

		[Action ("done:")]
		partial void done (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (gameFieldView != null) {
				gameFieldView.Dispose ();
				gameFieldView = null;
			}
		}
	}
}
