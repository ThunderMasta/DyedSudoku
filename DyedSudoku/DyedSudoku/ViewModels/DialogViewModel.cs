using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using Common;

namespace DyedSudoku
{
    public class DialogViewModel
    {
        private float Border { get; set; }

        private RectangleF Frame { get; set; }

        private readonly float cellHeight;
        private readonly float cellWidth;
        private readonly DateTime CreatedTime;
        private readonly TimeSpan delaySpan = new TimeSpan(0, 0, 0, 0, 500);
        private const int cellCount = 3;
        private const float cellContentLeft = 13;
        private const float cellContentBottom = 10;

		public bool IsInvisible
		{
			get { return DateTime.Now - CreatedTime < delaySpan; }
		}

        public DialogViewModel(RectangleF frame, float border)
        {
            Frame = frame;
            Border = border;

            cellHeight = Frame.Height / cellCount;
            cellWidth = Frame.Width / cellCount;

			CreatedTime = DateTime.Now;
        }

        public void Draw(CGContext context)
        {
            if (DateTime.Now - CreatedTime < delaySpan)
                return;

            DrawBackground(context);
            DrawLines(context);
            DrawModel(context);
        }

        private void DrawModel(CGContext context)
        {
            context.SetDefaultTextSettings();

            var number = 0;
            for (int j = cellCount - 1; j >= 0; j--)
                for (int i = 0; i < cellCount; i++)
                {
                    number++;
                    context.DrawDialogText(number.ToString(), Frame.X + i * cellWidth + cellContentLeft, Frame.Y + j * cellHeight + cellContentBottom, false);
                }
        }

        private void DrawLines(CGContext context)
        {
            context.SetDefaultLineSettings();

            for (float i = 0; i <= Frame.Height; i += cellHeight)
                context.AddLines(new[] { new PointF(Frame.X, Frame.Y + i), new PointF(Frame.X + Frame.Width, Frame.Y + i) });

            for (float i = 0; i <= Frame.Width; i += cellWidth)
                context.AddLines(new[] { new PointF(Frame.X + i, Frame.Y), new PointF(Frame.X + i, Frame.Y + Frame.Height) });

            context.DrawPath(CGPathDrawingMode.Stroke);
        }

        private void DrawBackground(CGContext context)
        {
            context.SetFillDialogBorderColor();
            context.AddRect(
                new RectangleF(Frame.X - Border, 
                               Frame.Y - Border, 
                               Frame.Width + 2 * Border, 
                               Frame.Height + 2 * Border));

            context.DrawPath(CGPathDrawingMode.Fill);

            context.SetFillDialogBackgroungColor();
            context.AddRect(Frame);
            context.DrawPath(CGPathDrawingMode.Fill);
        }

        public bool IsInside(PointF point)
        {
            return Frame.Contains(point);
        }

        public int GetNumber(PointF point)
        {
            var x = (int)((point.X - Frame.X) / cellWidth);
            var y = (int)((point.Y - Frame.Y) / cellHeight);

            return x + cellCount * (cellCount - 1 - y) + 1;
        }
    }
}

