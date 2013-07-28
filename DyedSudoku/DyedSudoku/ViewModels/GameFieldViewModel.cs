using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Common;

namespace DyedSudoku
{
    public class GameFieldViewModel
    {
        private FPSCounter fpsCounter = new FPSCounter();

        public RectangleF Frame { get; private set; }

        private GameFieldModel Model { get; set; }

        private float cellHeight;
        private float cellWidth;
        private float blockHeight;
        private float blockWidth;

        private CGContext context;

        private const float cellLeft = 10;
        private const float cellBottom = 8;

        public GameFieldViewModel(RectangleF frame)
        {
            InitModel();
            InitCellSizes(frame);
        }

        public void InitModel()
        {
            Model = new GameFieldModel();
        }

        public void InitCellSizes(RectangleF frame)
        {
            Frame = frame;

            cellHeight = Frame.Height / Model.CellLineCount;
            cellWidth = Frame.Width / Model.CellLineCount;
            blockHeight = Frame.Height / Model.BlockLineCount;
            blockWidth = Frame.Width / Model.BlockLineCount;
        }

        public void Draw(CGContext ctx)
        {            
            fpsCounter.Tick();

            SetCurrentContext(ctx);

            DrawLines();
            DrawModel();
            DrawFPS();
        }

        private void SetCurrentContext(CGContext ctx)
        {
            context = ctx;

            context.SetDefaultCTMSettings(Frame.Height);
            context.SetDefaultTextSettings();
        }

        private void DrawLines()
        {
            DrawCellLines();
            DrawBlockLines();
        }

        private void DrawCellLines()
        {
            DrawGrid(cellHeight, cellWidth, 1);
        }

        private void DrawBlockLines()
        {
            DrawGrid(blockHeight, blockWidth, 2);
        }

        private void DrawGrid(float height, float width, float lineWidth)
        {
            context.SetLineWidth(lineWidth);

            for (float i = 0; i <= Frame.Height; i += height)
                context.AddLines(new[] { new PointF(0, i), new PointF(Frame.Width, i) });

            for (float i = 0; i <= Frame.Width; i += width)
                context.AddLines(new[] { new PointF(i, 0), new PointF(i, Frame.Height) });

            context.DrawPath(CGPathDrawingMode.Stroke);
        }

        private void DrawModel()
        {
            context.SetDefaultTextSettings();

            for (byte i = 0; i < Model.CellLineCount; i++)
            {
                for (byte j = 0; j < Model.CellLineCount; j++)
                {
                    context.ShowTextAtPoint(i * cellWidth + cellLeft, j * cellHeight + cellBottom, Model.GetItem(i, j).ToString());
                }
            }
        }

        private void DrawFPS()
        {
            context.SetDefaultInfoTextSettings();

            context.ShowTextAtPoint(Frame.Width - 100, Frame.Height - 20, DateTime.Now.ToLongTimeString());
            context.ShowTextAtPoint(Frame.Width - 30, Frame.Height - 20, fpsCounter.GetFPS().ToString());
        }
    }
}