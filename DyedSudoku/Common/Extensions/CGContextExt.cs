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

        public static void SetDefaultTextSettings(this CGContext context)
        {
            context.SetLineWidth(1);
            context.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
            context.SelectFont("Helvetica", 26, CGTextEncoding.MacRoman);
        }

        public static void SetDefaultInfoTextSettings(this CGContext context)
        {
            context.SetLineWidth(1);
            context.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
            context.SetStrokeColor(UIColor.Red.CGColor);
            context.SelectFont("Helvetica", 12, CGTextEncoding.MacRoman);
        }
    }
}

