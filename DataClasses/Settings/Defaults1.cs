using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelticKnots;

public static class CanvasProperties {
    //these will probably become dynamic.
    private static double Width { get; set; } = 30;
    private static double Height { get; set; } = 30;
    public static bool RotateOutput { get; set; } = true;  
}
public static class TileProperties {
    public static double Linewidth { get; set; } = 30;
    public static double Width { get; set; } = 100;
    public static double Height { get; set; } = 100;
    //I may change these to Pens
    public static Pen GlyphPen { get; set; } = new(Color.Red);
    public static Pen OutlinePen { get; set; } = new(Color.Blue);
    public static Pen FirstPointPen { get; set; } = new(Color.Green);
    public static Pen BackPen { get; set; } = new(Color.White);

}

