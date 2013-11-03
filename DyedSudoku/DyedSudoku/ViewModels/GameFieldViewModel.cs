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
        private const float cellContentLeft = 10;
        private const float cellContentBottom = 8;

        public GameFieldViewModel(RectangleF frame)
        {
            InitModel();
            InitCellSizes(frame);
        }

        public void InitModel()
        {
            Model = new GameFieldModel();
        }

        public void Cancel()
        {
            Model.Cancel();
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

            if (Model.IsInitializing)
            {
                DrawBackground();
                DrawCellsBackground();
            }
            else
            {
                DrawModel();
            }

            DrawLines();
            DrawFPS();
        }

        private void SetCurrentContext(CGContext ctx)
        {
            context = ctx;

            context.SetDefaultCTMSettings(Frame.Height);
            context.SetDefaultTextSettings();
        }

        private void DrawBackground()
        {
            context.SetFillEmptyItemColor();
            context.AddRect(new RectangleF(0, 0, Frame.Width, Frame.Height));
            context.DrawPath(CGPathDrawingMode.Fill);
        }

        private void DrawCellsBackground()
        {
            foreach (var pair in Model.GetAllPairs())
            {
                if (Model.IsItemEmpty(pair))
                    continue;

                if (Model.GetItemVisible(pair))
                    context.SetFillVisibleItemColor();
                else
                    context.SetFillInitializedItemColor();

                context.AddRect(new RectangleF(pair.X * cellWidth, pair.Y * cellHeight, cellWidth, cellHeight));
                context.DrawPath(CGPathDrawingMode.Fill);
            }
        }

        private void DrawLines()
        {
            context.SetDefaultLineSettings();

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

            foreach (var pair in Model.GetAllPairs())
            {
                if (!Model.GetItemVisible(pair))
                    continue;

                context.DrawText(Model.GetItemNumber(pair).ToString(), pair.X * cellWidth + cellContentLeft, pair.Y * cellHeight + cellContentBottom);
            }
        }

        private void DrawFPS()
        {
            context.SetDefaultInfoTextSettings();

            context.DrawInfoText(DateTime.Now.ToLongTimeString(), Frame.Width - 100, Frame.Height - 20);
            context.DrawInfoText(fpsCounter.GetFPS().ToString(), Frame.Width - 30, Frame.Height - 20);
        }
    }
}