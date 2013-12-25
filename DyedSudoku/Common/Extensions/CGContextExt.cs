using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreText;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Common
{
    public static class CGContextExt
    {
        public static void SetDefaultCTMSettings(this CGContext context, float height)
        {
            context.ScaleCTM(1, -1);
            context.TranslateCTM(0, -height);
        }

        public static void SetDefaultLineSettings(this CGContext context)
        {
            context.SetLineWidth(1);
            context.SetStrokeColor(UIColor.Black.CGColor);
        }

        public static void SetDefaultTextSettings(this CGContext context)
        {
            context.SetLineWidth(1);
            context.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
            context.SetStrokeColor(UIColor.Black.CGColor);
            context.SetFillColor(UIColor.Black.CGColor);
        }

        public static void SetDefaultInfoTextSettings(this CGContext context)
        {
            context.SetLineWidth(1);
            context.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
            context.SetStrokeColor(UIColor.Red.CGColor);
            context.SetFillColor(UIColor.Red.CGColor);
        }

        public static void DrawText(this CGContext context, string source, float x, float y)
        {
            context.DrawText(source, x, y, "HelveticaNeue-UltraLight", 26f, false);
        }

        public static void DrawInfoText(this CGContext context, string source, float x, float y)
        {
            context.DrawText(source, x, y, "HelveticaNeue-UltraLight", 12f, false);
        }

        public static void DrawResultText(this CGContext context, string source, float x, float y, bool isCenter)
        {
            context.DrawText(source, x, y, "HelveticaNeue-UltraLight", 100f, isCenter);
        }

        public static void DrawDialogText(this CGContext context, string source, float x, float y, bool isCenter)
        {
            context.DrawText(source, x, y, "HelveticaNeue-UltraLight", 36f, isCenter);
        }

        private static void DrawText(this CGContext context, string source, float x, float y, string fontName, float fontSize, bool isCenter)
        {
            var stringAttributes = new CTStringAttributes
            {
                ForegroundColorFromContext = true,
                Font = new CTFont(fontName, fontSize)
            };

            var attributedString = new NSAttributedString(source, stringAttributes);
            
            context.TextPosition = new PointF(x, y);

            using (var textLine = new CTLine(attributedString))
            {
                if (isCenter)
                    context.TextPosition = new PointF((float)(x - (textLine.GetTypographicBounds() / 2)), y);

                textLine.Draw(context);
            }
        }

        public static void SetFillEmptyItemColor(this CGContext context)
        {
			context.SetFillColor(((float)0xff/0xff), ((float)0x66/0xff), ((float)0x66/0xff), ((float)0xff/0xff));
        }

        public static void SetFillInitializedItemColor(this CGContext context)
        {
			context.SetFillColor(((float)0x00/0xff), ((float)0x99/0xff), ((float)0xff/0xff), ((float)0xff/0xff));
        }

        public static void SetFillVisibleItemColor(this CGContext context)
        {
			context.SetFillColor(((float)0x33/0xff), ((float)0xff/0xff), ((float)0x66/0xff), ((float)0xff/0xff));
        }

        public static void SetFillSelectedItemColor(this CGContext context)
        {
            context.SetFillColor(UIColor.Orange.CGColor);
        }

        public static void SetFillDialogBorderColor(this CGContext context)
        {
            context.SetFillColor(UIColor.Blue.CGColor);
        }

        public static void SetFillDialogBackgroungColor(this CGContext context)
        {
            context.SetFillColor(UIColor.White.CGColor);
        }

        public static void SetFillWinInfoBackgroundColor(this CGContext context)
        {
            context.SetFillColor(UIColor.Green.CGColor);
        }

        public static void SetFillLoseInfoBackgroundColor(this CGContext context)
        {
            context.SetFillColor(UIColor.Red.CGColor);
        }
    }
}

