using System;
using MonoTouch.CoreGraphics;
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
            context.SelectFont("Helvetica", 26, CGTextEncoding.MacRoman);
        }

        public static void SetDefaultInfoTextSettings(this CGContext context)
        {
            context.SetLineWidth(1);
            context.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
            context.SetStrokeColor(UIColor.Red.CGColor);
            context.SetFillColor(UIColor.Red.CGColor);
            context.SelectFont("Helvetica", 12, CGTextEncoding.MacRoman);
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

