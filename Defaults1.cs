using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelticKnots;

public static class CanvasProperties
{
    //these will probably become dynamic.
    static double Width { get; set; } = 20;
    static double Height { get; set; } = 20;
}
public static class TileProperties
{
    public static double Linewidth { get; set; } = 11;
    public static double Width { get; set; } = 100;
    public static double Height { get; set; } = 100;
    public static Color LineColour { get; set; } = Color.Red;
    public static Color BackColour { get; set; } = Color.White;
}

