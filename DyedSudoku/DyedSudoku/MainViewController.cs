using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Utility
{
    public partial class MainViewController : UIViewController
    {
        public MainViewController() : base ("MainViewController", null)
        {
            // Custom initialization
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

        partial void showInfo(NSObject sender)
        {
            var controller = new InfoViewController { ModalTransitionStyle = UIModalTransitionStyle.PartialCurl };
            controller.Done += delegate
            {
                DismissViewController(true, null);
            };
            PresentViewController(controller, true, null);
        }

        partial void start(NSObject sender)
        {
            var controller = new PlayLocalViewController { ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve };
            controller.Done += delegate
            {
                DismissViewController(true, null);
            };
            PresentViewController(controller, true, null);
        }
    }
}

