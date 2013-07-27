using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Common;

namespace DyedSudoku
{
    public class GameFieldViewModel
    {
        FPSCounter fpsCounter = new FPSCounter();

        private RectangleF Bounds { get; set; }

        private GameFieldModel Model { get; set; }

        public GameFieldViewModel(RectangleF bounds)
        {
            Bounds = bounds;
            Model = new GameFieldModel();
        }

        public void Draw(CGContext context)
        {            
            fpsCounter.Tick();
            
            context.SetDefaultCTMSettings(Bounds.Height);
            context.SetDefaultTextSettings();

            DrawLines(context);
            DrawFPS(context);
        }

        private void DrawLines(CGContext context)
        {
            context.SetLineWidth(4);

            for (float i = 0; i <= Bounds.Height; i += Bounds.Height / Model.CellLineCount)
                context.AddLines(new[] { new PointF(0, i), new PointF(Bounds.Width, i) });

            for (float i = 0; i <= Bounds.Width; i += Bounds.Width / Model.CellLineCount)
                context.AddLines(new[] { new PointF(i, 0), new PointF(i, Bounds.Height) });

            context.DrawPath(CGPathDrawingMode.Stroke);
        }

        private void DrawFPS(CGContext context)
        {
            context.SetDefaultInfoTextSettings();

            context.ShowTextAtPoint(200, 300, DateTime.Now.ToLongTimeString());
            context.ShowTextAtPoint(282, 300, fpsCounter.GetFPS().ToString());
        }
    }
}