using System;
using System.IO;
using static CelticKnots.GridPoint;
namespace CelticKnots;

public class SVGHelper()
{
    //https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/d#cubic_b%C3%A9zier_curve

    public static string SVGStart()
    {
        string s = $"<svg{Environment.NewLine}";
        s += $"\tviewBox=\"0 0 1000 1000\"{Environment.NewLine}";
        s += $"\txmlns=\"http://www.w3.org/2000/svg\"{Environment.NewLine}";
        s += $"\txmlns:xlink=\"http://www.w3.org/1999/xlink\">{Environment.NewLine}";
        s += $"\t<!-- Cubic Bézier Curves and Lines with absolute coordinates.>{Environment.NewLine}";

        s += $"\t\tMoveTo: M, m{Environment.NewLine}";
        s += $"\t\tLineTo: L, l, H, h, V, v{Environment.NewLine}";
        s += $"\t\tCubic Bézier curve: C, c, S, s - Uppercase absolute coordinates Lowercase relative coordinates.{Environment.NewLine}";
        s += $"\t\tQuadratic Bézier curve: Q, q, T, t{Environment.NewLine}";
        s += $"\t\tElliptical arc curve: A, a{Environment.NewLine}";
        s += $"\t\tClosePath: Z, z{Environment.NewLine}";
        s += $"\t  < StartPoint ControlPoint1 ControlPoint2 EndPoint -->{Environment.NewLine}";
        return s;

    }
    public static string SVGHeader()
    {
        string s = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>{Environment.NewLine}";
        s += $"<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">{Environment.NewLine}";
        s += $"\t<!-- Creator: Tony Cullen -->{Environment.NewLine}";
        return s;
    }
    public static string SVGEnd()
    {
        return $"</svg>{Environment.NewLine}";
    }

    public static string ConvertTwoPointLine(ListOfPointFForDrawing pointFList)
    {
        string s = $"\t\t\t< path{Environment.NewLine}";
        s += $"\t\t\t\tfill=\"none\"{Environment.NewLine}";
        s += $"\t\t\t\tstroke=\"red\"{Environment.NewLine}";
        s += $"\t\t\t\td=\"M {pointFList[0].X},{pointFList[0].Y}{Environment.NewLine}";
        s += $"\t\t\t\tL {pointFList[1].X},{pointFList[1].Y}\"{Environment.NewLine}";
        s += $"\t\t\t/>{Environment.NewLine}";
        return s;
    }
    public static string ConvertTwoPointLine(WorkingTile thisTile, GridPoint startpoint, GridPoint endpoint)
    {
        double XOffset = TileProperties.Width * thisTile.X;
        double YOffset = TileProperties.Height * thisTile.Y;

        string s = $"\t\t\t< path{Environment.NewLine}";
        s += $"\t\t\t\tfill=\"none\"{Environment.NewLine}";
        s += $"\t\t\t\tstroke=\"red\"{Environment.NewLine}";
        s += $"\t\t\t\td=\"M {startpoint.X + XOffset},{startpoint.Y + YOffset}{Environment.NewLine}";
        s += $"\t\t\t\tL {endpoint.X + XOffset},{endpoint.Y + YOffset}\"{Environment.NewLine}";
        s += $"\t\t\t/>{Environment.NewLine}";
        return s;
    }
    public static string ConvertFourPointLine(ListOfPointFForDrawing pointFList)
    {

        string s = $"\t\t\t< path{Environment.NewLine}";
        s += $"\t\t\t\tfill=\"none\"{Environment.NewLine}";
        s += $"\t\t\t\tstroke=\"red\"{Environment.NewLine}";
        s += $"\t\t\t\td=\"M {pointFList[0].X},{pointFList[0].Y}{Environment.NewLine}";
        s += $"\t\t\t\tL {pointFList[1].X},{pointFList[1].Y} {pointFList[2].X},{pointFList[2].Y} {pointFList[3].X},{pointFList[3].Y}\"{Environment.NewLine}";
        s += $"\t\t\t/>{Environment.NewLine}";
        return s;
    }
    public static string ConvertFourPointLine(WorkingTile thisTile, GridPoint p1, GridPoint c1, GridPoint c2, GridPoint p2)
    {
        double XOffset = TileProperties.Width * thisTile.X;
        double YOffset = TileProperties.Height * thisTile.Y;
        string s = $"\t\t\t< path{Environment.NewLine}";
        s += $"\t\t\t\tfill=\"none\"{Environment.NewLine}";
        s += $"\t\t\t\tstroke=\"red\"{Environment.NewLine}";
        s += $"\t\t\t\td=\"M {p1.X + XOffset},{p1.Y + YOffset}{Environment.NewLine}";
        s += $"\t\t\t\tL {c1.X + XOffset},{c1.Y + YOffset} {c2.X + XOffset},{c2.Y + YOffset} {p2.X + XOffset},{p2.Y + YOffset}\"{Environment.NewLine}";
        s += $"\t\t\t/>{Environment.NewLine}";
        return s;
    }
    public static string ConvertCurve(ListOfPointFForDrawing pointFList)
    {
        string s = $"\t\t\t< path{Environment.NewLine}";
        s += $"\t\t\t\tfill=\"none\"{Environment.NewLine}";
        s += $"\t\t\t\tstroke=\"red\"{Environment.NewLine}";
        s += $"\t\t\t\td=\"M {pointFList[0].X },{pointFList[0].Y }{Environment.NewLine}";
        s += $"\t\t\t\tC {pointFList[1].X},{pointFList[1].Y } {pointFList[2].X },{pointFList[2].Y } {pointFList[3].X },{pointFList[3].Y }\"{Environment.NewLine}";
        s += $"\t\t\t/>{Environment.NewLine}";
        return s;
    }

    public static string ConvertCurve(WorkingTile thisTile, GridPoint p1, GridPoint c1, GridPoint c2, GridPoint p2)
    {
        double XOffset = TileProperties.Width * thisTile.X;
        double YOffset = TileProperties.Height * thisTile.Y;
        string s = $"\t\t\t< path{Environment.NewLine}";
        s += $"\t\t\t\tfill=\"none\"{Environment.NewLine}";
        s += $"\t\t\t\tstroke=\"red\"{Environment.NewLine}";
        s += $"\t\t\t\td=\"M {p1.X + XOffset},{p1.Y + YOffset}{Environment.NewLine}";
        s += $"\t\t\t\tC {c1.X + XOffset},{c1.Y + YOffset} {c2.X + XOffset},{c2.Y + YOffset} {p2.X + XOffset},{p2.Y + YOffset}\"{Environment.NewLine}";
        s += $"\t\t\t/>{Environment.NewLine}";
        return s;
    }
    public static string GetSVGTileDebugOutline(WorkingTile thisTile)
    {
        //https://www.w3schools.com/graphics/svg_rect.asp
        float XOffset = (float)TileProperties.Width * thisTile.X;
        float YOffset = (float)TileProperties.Height * thisTile.Y;
        //g.DrawRectangle(new Pen(Color.Blue), XOffset, YOffset, (int)TileProperties.Width, (int)TileProperties.Height);
        //<rect width="200" height="100" x="10" y="10" rx="20" ry="20" fill="blue" />

        //<rect width="100" height="100" x="0" y="0" rx = "2" ry = "2" fill="none" stroke="blue"/>   //this is from the Web and didn't work
        //<rect width="100" height="100" x="0" y="0" rx="2" ry="2" fill="none" stroke="blue"/>       //This worked
        // I can delete rx and ry but cannot have a value "0"
        return $"\t\t\t\t<rect width=\"{(int)TileProperties.Width}\" height=\"{(int)TileProperties.Height}\" x=\"{XOffset}\" y=\"{YOffset}\" rx=\"2\" ry=\"2\" fill=\"none\" stroke=\"blue\" />{Environment.NewLine}";

    }
    public static string GetSVGTileDebugPointline( ListOfPointFForDrawing pointFList)
    {
        //WorkingTile thisTile,
        //float XOffset = (float)TileProperties.Width * thisTile.X;
        //float YOffset = (float)TileProperties.Height * thisTile.Y;
        double TileWidth = TileProperties.Width;
        double TileHeight = TileProperties.Height;

        string data = string.Empty;
        foreach (var pointF in pointFList) {
            float X = (float)pointF.X;
            float Y = (float)pointF.Y;    
            // draw a vertical line in green 
            data += $"<line x1=\"{X}\" y1=\"0\" x2=\"{X}\" y2=\"{TileHeight}\" style=\"stroke:green;stroke-width:1\" />{Environment.NewLine}";
            data += $"<line x1=\"0\" y1=\"{Y}\" x2=\"{TileWidth}\" y2=\"{Y}\" style=\"stroke:green;stroke-width:1\" />{Environment.NewLine}";

        }

        return data;
    }
}