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
            InitDataSource();
        }

        public GameFieldView(IntPtr handle) : base(handle)
        {
            InitDataSource();
        }

        public GameFieldView(GameFieldViewModel viewModel)
        {
            dataSource = viewModel;
        }

        private void InitDataSource()
        {
            dataSource = new GameFieldViewModel(Bounds);
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);

            using (CGContext context = UIGraphics.GetCurrentContext())
                dataSource.Draw(context);
        }
    }
}