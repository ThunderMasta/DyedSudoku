using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace DyedSudoku
{
    [Register ("GameFieldView")] 
    public class GameFieldView : UIView
    {
        public GameFieldView() : base()
        {
        }

        public GameFieldView(IntPtr handle) : base(handle)
        {
        }

        private void UpdateView()
        {
            while (true)
            {
                BeginInvokeOnMainThread(SetNeedsDisplay);
            }
        }

        public override void Draw(System.Drawing.RectangleF rect)
        {
            base.Draw(rect);

            DrawLines(rect);
            DrawText(rect);
        }

        private void DrawLines(System.Drawing.RectangleF rect)
        {
            using (var context = UIGraphics.GetCurrentContext())
            {
                context.SetLineWidth(4);

                for (int i = 0; i < Bounds.Height; i = i + 50)
                {
                    context.AddLines(new [] { new PointF(0, i), new PointF(Bounds.Width, i) });
                }

                context.DrawPath(CGPathDrawingMode.Stroke);
            }
        }

        private ulong ticks;
        private ulong lastFPS;
        private DateTime last;

        private void DrawText(System.Drawing.RectangleF rect)
        {
            using (var context = UIGraphics.GetCurrentContext())
            {
                context.ScaleCTM(1, -1);
                context.TranslateCTM(0, -Bounds.Height);

                context.SetLineWidth(1f);

                context.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
                context.SelectFont("Helvetica", 8f, CGTextEncoding.MacRoman);

                context.ShowTextAtPoint(200, 300, DateTime.Now.ToLongTimeString());

                ticks++;
                if (DateTime.Now.Second != last.Second)
                {
                    last = DateTime.Now;
                    lastFPS = ticks;
                    ticks = 0;
                }
                context.ShowTextAtPoint(275, 300, lastFPS.ToString());
            }
        }
    }
}

