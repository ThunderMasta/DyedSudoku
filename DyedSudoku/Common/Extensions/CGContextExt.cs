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
            context.DrawText(source, x, y, "Helvetica", 26f);
        }

        public static void DrawInfoText(this CGContext context, string source, float x, float y)
        {
            context.DrawText(source, x, y, "Helvetica", 12f);
        }

        private static void DrawText(this CGContext context, string source, float x, float y, string fontName, float fontSize)
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
                textLine.Draw(context);
            }
        }

        public static void SetFillEmptyItemColor(this CGContext context)
        {
            context.SetFillColor(UIColor.Red.CGColor);
        }

        public static void SetFillInitializedItemColor(this CGContext context)
        {
            context.SetFillColor(UIColor.Blue.CGColor);
        }

        public static void SetFillVisibleItemColor(this CGContext context)
        {
            context.SetFillColor(UIColor.Green.CGColor);
        }
    }
}

