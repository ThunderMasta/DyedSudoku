using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading;

namespace DyedSudoku
{
    public partial class PlayLocalViewController : UIViewController
    {
        // Need update thread for custom animation
        private volatile bool needUpdateGameField;

        private GameFieldViewModel gameFieldViewModel;

        public PlayLocalViewController() : base ("PlayLocalViewController", null)
        {
            InitGameFieldView();

            StartUpdateThread();
        }

        private void InitGameFieldView()
        {
            var width = View.Frame.Width - 10;
            var frame = new RectangleF(5, gameFieldView.Frame.Y, width, width);

            gameFieldView.Frame = frame;
            gameFieldViewModel = new GameFieldViewModel(frame);
            gameFieldView.SetDataSource(gameFieldViewModel);
        }

        private void StartUpdateThread()
        {
            needUpdateGameField = true;
            ThreadPool.QueueUserWorkItem(UpdateGameField);
        }

        private void StopUpdateThread()
        {
            needUpdateGameField = false;
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
            StopUpdateThread();

            if (gameFieldViewModel != null)
                gameFieldViewModel.Cancel();

            if (Done != null)
                Done(this, EventArgs.Empty);
        }

        private void UpdateGameField(object obj)
        {
            while (needUpdateGameField)
            {
                //Sleep for 60 fps
                Thread.Sleep(15);
                BeginInvokeOnMainThread(() => gameFieldView.SetNeedsDisplay());
            }
        }

        partial void singleTap(NSObject sender)
        {
            gameFieldViewModel.Cancel();
        }
    }
}

