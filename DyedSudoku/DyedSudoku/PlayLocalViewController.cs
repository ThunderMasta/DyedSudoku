using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading;
using System.Threading.Tasks;

namespace DyedSudoku
{
	public partial class PlayLocalViewController : UIViewController
	{
		// Need update thread for custom animation
		private volatile bool needUpdateGameField;
		private GameFieldViewModel gameFieldViewModel;
		private const int defaultDelay = 100;
		private const float defaultOffset = 5f;
		private bool isPaused;

		public PlayLocalViewController() : base("PlayLocalViewController", null)
		{
			InitGameFieldView();

			StartUpdateThread();
		}

		private void InitGameFieldView()
		{
			var width = View.Frame.Width - 2 * defaultOffset;
			var frame = new RectangleF(defaultOffset, gameFieldView.Frame.Y, width, width);

			gameFieldView.Frame = frame;
			gameFieldViewModel = new GameFieldViewModel(frame);
			gameFieldView.SetDataSource(gameFieldViewModel);
		}

		private void StartUpdateThread()
		{
			needUpdateGameField = true;
			UpdateGameField();
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
			FireDone();
		}

		private void FireDone()
		{
			StopUpdateThread();

			if (gameFieldViewModel != null)
				gameFieldViewModel.Cancel();

			if (Done != null)
				Done(this, EventArgs.Empty);
		}

		private async void UpdateGameField()
		{
			while (needUpdateGameField)
			{
				//Sleep for 10 fps
				await Task.Delay(defaultDelay);

				if (gameFieldViewModel.NeedRefresh)
				{
					Refresh();
					isPaused = false;
				}
				else
				{
					if (!isPaused)
					{
						Refresh();
						isPaused = true;
					}
				}
			}
		}

		private void Refresh()
		{
			BeginInvokeOnMainThread(gameFieldView.SetNeedsDisplay);
		}

		partial void singleTap(NSObject sender)
		{
			if (gameFieldViewModel.IsEnd)
			{
				FireDone();
				return;
			}

			var tapRecognizer = (UITapGestureRecognizer)sender;

			var point = tapRecognizer.LocationInView(View);
			var gameFieldPoint = tapRecognizer.LocationInView(gameFieldView);

			if (gameFieldView.Frame.Contains(point))
				gameFieldViewModel.UpdateByTap(gameFieldPoint);
			else
				gameFieldViewModel.UpdateByOffboardTap();

			Refresh();
		}
	}
}

