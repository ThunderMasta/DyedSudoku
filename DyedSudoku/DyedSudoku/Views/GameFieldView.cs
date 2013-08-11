using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Common;

namespace DyedSudoku
{
    [Register("GameFieldView")]
    public class GameFieldView : UIView
    {
        GameFieldViewModel dataSource;

        public GameFieldView()
        {
        }

        public GameFieldView(IntPtr handle) : base(handle)
        {
        }

        public GameFieldView(GameFieldViewModel viewModel)
        {
            SetDataSource(viewModel);
        }

        private void InitDataSource()
        {
            SetDataSource(new GameFieldViewModel(Bounds));
        }

        public void UpdateFrame()
        {
            Frame = dataSource.Frame;
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);

            if (dataSource == null)
                return;

            using (CGContext context = UIGraphics.GetCurrentContext())
                dataSource.Draw(context);
        }

        public void SetDataSource(GameFieldViewModel viewModel)
        {
            dataSource = viewModel;
        }
    }
}