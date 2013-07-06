using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading;

namespace DyedSudoku
{
    public partial class PlayLocalViewController : UIViewController
    {
        private Thread updateThread;

        public PlayLocalViewController() : base ("PlayLocalViewController", null)
        {
            gameFieldView = new GameFieldView();
            updateThread = new Thread(UpdateGameField);
            updateThread.Start();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public event EventHandler Done;

        partial void done(MonoTouch.Foundation.NSObject sender)
        {
            updateThread.Abort();
            updateThread = null;

            if (Done != null)
                Done(this, EventArgs.Empty);
        }

        private void UpdateGameField()
        {
            while (true)
            {
                BeginInvokeOnMainThread(() => gameFieldView.SetNeedsDisplay());
            }
        }
    }
}

