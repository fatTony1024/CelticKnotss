using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Xml.Linq;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using static CelticKnots.GridPoint;
using System.Runtime.CompilerServices;
namespace CelticKnots;
public class GPointOffsets : List<GridPoint>
{
    //These are the points to offset for Left and right outlines, it didn't quite work as I imained, it was close but took some manipulation to make to work.
    //25 points above to the left, right and below the normal draw point, they are flipped and rotated so the same offset number is the same for all points (this didn't work for the middle points)

    public E_LinePoint LinePoint { get; set; } = E_LinePoint.None;
    public int Rotation { get; set; } = 0;
    //These are the offsets used when drawing the "outline" of a path.
    public GPointOffsets(E_LinePoint linepoint)
    {
        LinePoint = linepoint;
        double OffSetGap = (TileProperties.Linewidth - 1);
        double HalfLinewidth = (OffSetGap / 2);
        //Changed my mind 12 is the middle -- or 12 + 25 for the diagonal cross.
        //GridPoint p = new GridPoint(0, 0);
        double YPos = -(OffSetGap + HalfLinewidth);
        double XPos = -(OffSetGap + HalfLinewidth);
        //Make the Offset points
        for (int y = 0; y < 5; y++)
        {
            YPos += HalfLinewidth;
            for (int x = 0; x < 5; x++)
            {
                XPos += HalfLinewidth;
                Add(new GridPoint(XPos, YPos));
            }
            //back to the start.
            XPos = -(OffSetGap + HalfLinewidth);
        }

        int intRotate = 0;
        switch (linepoint)
        {
            case E_LinePoint.None:
            case E_LinePoint.TopMiddle:
            case E_LinePoint.TopMiddleMiddleSplit:
            case E_LinePoint.MiddleMiddle:
                {
                    intRotate = 0;
                    break;
                }
            case E_LinePoint.TopRight:
            case E_LinePoint.TopRightMiddleSplit:
                {
                    intRotate = 45;
                    break;
                }
            case E_LinePoint.MiddleRightsplit:
            case E_LinePoint.MiddleRight:
                {
                    intRotate = 90;
                    break;
                }
            case E_LinePoint.BottomRightMiddleSplit:
            case E_LinePoint.BottomRight:
                {
                    intRotate = 135;
                    break;
                }
            case E_LinePoint.BottomMiddleMiddleSplit:
            case E_LinePoint.BottomMiddle:
                {
                    intRotate = 180;
                    break;
                }
            case E_LinePoint.BottomLeftMiddleSplit:
            case E_LinePoint.BottomLeft:
                {
                    intRotate = 225;
                    break;
                }
            case E_LinePoint.MiddleLeft:
            case E_LinePoint.MiddleLeftsplit:
                {
                    intRotate = 270;
                    break;
                }
            case E_LinePoint.TopLeft:
            case E_LinePoint.TopLeftMiddleSplit:
                {
                    intRotate = 315;
                    break;
                }
        }
        Rotation = intRotate;
        //I will rotate the points when I draw the Line -- there rules are the
        // An End point has its Offsetpoints flipped vertically then the Rotation applied - the centre point needs to rotate independantly based on the line it is either drawing from or to.

        //Rotate the points when I want to get them based on the rotation of the Edge point it was drawn from.
        //if (intRotate != 0)
        //    RotateAllPoints(intRotate, false);

        //If this is E_LinePoint.MiddleMiddle Duplicate all the points and rotate each 45 degrees. This will be used for the diagonal cross.
        //so the diagonal cross centre points are index + 25
        //if (linepoint == E_LinePoint.MiddleMiddle)
        //{

        ////add it to all the objects now, but I may remove the extended points later
        //    int PointCount = this.Count;
        //    for (int pCounter = 0; pCounter < PointCount; pCounter++)
        //    {
        //        p = this[pCounter].Clone();
        //        p.RotatePoint(45);
        //        Add(p);
        //    }
        ////}

        //Add the Actual point to the LinePoins
        PlaceTheOffsetPoints(linepoint);
    }
    public void DebugPoints()
    {
        string strOutput = string.Empty;
        for (int i = 0; i < this.Count; i++)
        {
            GridPoint p = this[i];
            strOutput += $"(X{p.X,4} Y{p.Y,4})";
            if ((i + 1) % 5 == 0)
            {
                strOutput += Environment.NewLine;
            }
            else
            {
                strOutput += ",";
            }
        }
        Debug.Print(strOutput);

        //for (int y = 0; y < 5; y++)
        //{
        //    for (int x = 0; x < 5; x++)
        //    {

        //    }


        //}


    }
    public void RotateAllPoints(double angletoRotate, bool roundCentre)
    {
        for (int i = 0; i < this.Count; i++)
        {
            GridPoint p = this[i];
            if (roundCentre)
            {
                p.RotatePoint(angletoRotate);
            }
            else { p.RotatePointRoundZeroZero(angletoRotate); }
        }
    }
    public void RotateAllPoints(GridPoint centerPoint, double angleInDegrees)
    {
        if (angleInDegrees == 0)
        {
            return;
        }

        for (int i = 0; i < this.Count; i++)
        {
            GridPoint p = this[i];
            p.RotatePoint(centerPoint, angleInDegrees);
        }
    }
    //public void FlipVertically(GridPoint centerPoint)
    //{

    //    List<GridPoint> g = [];
    //    for (int Row = 5; Row > 0; Row--)
    //    {
    //        for (int Col = 0; Col < 5; Col++)
    //        {
    //            int i = ((Row - 1) * 5) + Col;
    //            g.Add(this[i]);
    //        }
    //    }
    //    //Copy the new list back
    //    this.Clear();
    //    this.AddRange(g);
    //}
    public void FlipHorizontally()
    {
        List<GridPoint> g = [];
        for (int Row = 1; Row < 6; Row++)
        {
            for (int Col = 5; Col > 0; Col--)
            {
                int i = ((Row - 1) * 5) + Col - 1;
                g.Add(this[i]);
            }
        }
        //Copy the new list back
        this.Clear();
        this.AddRange(g);
    }

    public GPointOffsets Clone()
    {
        GPointOffsets returnList = new(this.LinePoint);
        //The points are added in the create.
        //for (int i = 0; i < this.Count; i++)
        //{
        //    returnList.Add(this[i].Clone());
        //    returnList.Rotation = this.Rotation;
        //}
        return returnList;

    }
    private void PlaceTheOffsetPoints(E_LinePoint linepoints)
    {
        GridPoint ThisPoint = LinePointHelper.GetGpoint(linepoints);
        for (int i = 0; i < this.Count; i++)
        {
            GridPoint p = this[i];
            p.X += ThisPoint.X;
            p.Y += ThisPoint.Y;
        }
    }

}
public class LinePointObject
{
    //Extended information for the E_LinePoint ThisLine;
    public E_LinePoint ThisLine { get; set; }
    //Get the Description and name from the enumeratoion.
    //private string _Description = ""; //C# hides the auto backing field   “<Description>k__BackingField” so is not accesable.
    //List<GridPoint> _OffsetPoints = new List<GridPoint>();
    //This may become a "stock" entity as it always starts the same and is manipulated for
    //The Start point of a line (rotate to angle)
    //the end point of a Line Rotate to angle - some rotate horizontally 
    //if the start is topmiddle and end bottommiddle
    //  the bottom is  Flipped horizontally and rotated 180'
    //if the start is topmiddle and end MiddleRight
    //  the miffle right is rotated 90'   --lets make a list t determine the rules
    //TopMiddle to TopRight



    //public GPointOffsets OffsetPoints { get; private set; }
    public string Description { get; private set; } = "";
    public string Name { get; private set; } = "";
    public int ThisNunber { get; private set; } = 0;
    public bool CentredXorY { get; private set; } = false;
    public bool CentredX { get; private set; } = false;
    public bool CentredY { get; private set; } = false;
    public bool IsCorner { get; private set; } = false;
    public bool IsLeftRow { get; private set; } = false;
    public bool IsTopRow { get; private set; } = false;
    public bool IsRighRow { get; private set; } = false;
    public bool IsBottomRow { get; private set; } = false;
    public bool IsLeftOfCentre { get; private set; } = false;
    public bool IsRightOfCentre { get; private set; } = false;
    public bool IsMiddlePoint { get; private set; } = false;
    public bool IsTopOfCentre { get; private set; } = false;
    public bool IsBottomOfCentre { get; private set; } = false;
    public bool IsOutsideEdge { get; private set; } = false;
    public bool IsInside { get; private set; } = false;
    public bool UseDiagonalOffsets { get; set; } = false;
    public bool HasValue { get; private set; } = false;
    //Is the Point in the centre of the Block or a on a corner.
    public GridPoint ThisPoint { get; private set; }

    //This can be set for a corner (When more then two lines meet(4) lines meet) or point in the centre of the cross or diagonal cross to determine the GPointOffsets to use drawing it.
    //where 4 meet 2 are under the other two have Weave
    public E_TileWeave Weave { get; set; } = E_TileWeave.NoWeave;

    //This adds 25 to the offset value - I will test it with the diagonal cross first.
    public LinePointObject(E_LinePoint linepoints)
    {
        ThisLine = linepoints;
        //OffsetPoints = new GPointOffsets(ThisLine);
        ThisPoint = LinePointHelper.GetGpoint(ThisLine);
        //I probably don't need this as I already have it stored.
        //Debug.Assert(ThisPoint == _OffsetPoints[0]);
        //UseDiagonalOffsets=usediagonaloffsets;  
        //Debug.Assert(LinePointHelper.ObjectsAreEqual(ThisPoint, OffsetPoints[12]));
        ProcessPoint();
    }

    private void ProcessPoint()
    {
        Description = EnumerationHelper.GetDescription(ThisLine);
        Name = EnumerationHelper.GetEnumName(ThisLine);
        ThisNunber = (int)ThisLine;

        if (ThisNunber != 0)// 0 is a non-point.
        {
            //these will need updating as I have aded new points.
            //CentredX = ThisNunber % 3 == 1;
            //CentredY = ThisNunber - 3 is > 0 and < 4;
            CentredX = ThisNunber is > 6 and < 12; //7,8,9,10,11
            switch (ThisLine)
            {
                //CentredY - Vertical centre 2,5,9,13,16
                case E_LinePoint.TopMiddle:
                case E_LinePoint.TopMiddleMiddleSplit:
                case E_LinePoint.MiddleMiddle:
                case E_LinePoint.BottomMiddleMiddleSplit:
                case E_LinePoint.BottomMiddle:
                    CentredY = true;
                    break;
            };
            if (CentredX || CentredY)
            {
                CentredXorY = true; // in the middle. ie. not a corner.
            }


            if (CentredX && CentredY)
            {
                IsMiddlePoint = true;
            }

            //IsLeftOfCentre = ThisNunber % 3 ==0; //or <1
            //IsRightOfCentre = ThisNunber % 3 ==3; //or >1
            switch (ThisLine)
            {
                //IsLeftOfCentre - Left 
                case E_LinePoint.TopLeft:
                case E_LinePoint.TopLeftMiddleSplit:
                case E_LinePoint.MiddleLeft:
                case E_LinePoint.MiddleLeftsplit:
                case E_LinePoint.BottomLeftMiddleSplit:
                case E_LinePoint.BottomLeft:
                    IsLeftOfCentre = true;
                    break;
            };
            switch (ThisLine)
            {
                //IsRightOfCentre - Right
                case E_LinePoint.TopRight:
                case E_LinePoint.TopRightMiddleSplit:
                case E_LinePoint.MiddleRight:
                case E_LinePoint.MiddleRightsplit:
                case E_LinePoint.BottomRightMiddleSplit:
                case E_LinePoint.BottomRight:
                    IsRightOfCentre = true;
                    break;
            };

            IsTopOfCentre = ThisNunber < 7;
            IsBottomOfCentre = ThisNunber > 11;

            //if (IsTopOfCentre || IsBottomOfCentre || IsLeftOfCentre || IsRightOfCentre)
            //{
            //    if (!IsMiddlePoint)
            //    {
            //        IsOutsideEdge = true;
            //    }
            //}
            //Got fed up trying to work it out so just specify them.
            switch (ThisLine)
            {
                case E_LinePoint.TopLeft:
                case E_LinePoint.TopMiddle:
                case E_LinePoint.TopRight:
                case E_LinePoint.MiddleLeft:
                case E_LinePoint.MiddleRight:
                case E_LinePoint.BottomLeft:
                case E_LinePoint.BottomMiddle:
                case E_LinePoint.BottomRight:
                    IsOutsideEdge = true;
                    break;
            };
            IsInside = !IsOutsideEdge;

            switch (ThisLine)
            {
                case E_LinePoint.TopLeft:
                case E_LinePoint.MiddleLeft:
                case E_LinePoint.BottomLeft:
                    IsLeftRow = true;
                    break;
            };
            IsTopRow = ThisNunber < 4;
            switch (ThisLine)
            {
                case E_LinePoint.TopRight:
                case E_LinePoint.MiddleRight:
                case E_LinePoint.BottomRight:
                    IsRighRow = true;
                    break;
            };
            IsBottomRow = ThisNunber > 14; //BottomRightMiddleSplit

            if (IsOutsideEdge) // A corner is on the outside edge and not a centre line horizontally or vertically.
            {
                IsCorner = !CentredXorY;
            }

            HasValue = ThisNunber > 0; //0 is just a holder for not defined.
        }
    }
}
public static class LinePointHelper
{
    //I have copied this from the GridLine class, it will eventually be moved here.
    public static GridPoint GetGpoint(E_LinePoint thisline)
    {
        //GridPoint x;
        double MinX = 0;
        double MinY = 0;
        double MaxX = TileProperties.Width;
        double CentreX = MaxX / 2;
        double MaxY = TileProperties.Height;
        double CentreY = MaxY / 2;

        double SplitMinX = CentreX / 2;
        double SplitMaxX = CentreX + SplitMinX;
        double SplitMinY = CentreY / 2;
        double SplitMaxY = CentreY + SplitMinY;

        return thisline switch
        {
            E_LinePoint.None => new GridPoint(MinX, MinY),

            E_LinePoint.TopLeft => new GridPoint(MinX, MinY),
            E_LinePoint.TopMiddle => new GridPoint(CentreX, MinY),
            E_LinePoint.TopRight => new GridPoint(MaxX, MinY),

            //I didn't like this approach.
            //E_LinePoint.TopLeftMiddleSplit => new GridPoint(MinX, MinY).Merge(new GridPoint(CentreX, CentreY)),
            E_LinePoint.TopLeftMiddleSplit => new GridPoint(SplitMinX, SplitMinY),
            E_LinePoint.TopMiddleMiddleSplit => new GridPoint(CentreX, SplitMinY),
            E_LinePoint.TopRightMiddleSplit => new GridPoint(SplitMaxX, SplitMinY),


            E_LinePoint.MiddleLeft => new GridPoint(MinX, CentreY),
            E_LinePoint.MiddleMiddle => new GridPoint(CentreX, CentreY),
            E_LinePoint.MiddleRight => new GridPoint(MaxX, CentreY),

            E_LinePoint.MiddleLeftsplit => new GridPoint(SplitMinX, CentreY),
            E_LinePoint.MiddleRightsplit => new GridPoint(SplitMaxX, CentreY),


            E_LinePoint.BottomLeft => new GridPoint(MinX, MaxY),
            E_LinePoint.BottomMiddle => new GridPoint(CentreX, MaxY),
            E_LinePoint.BottomRight => new GridPoint(MaxX, MaxY),

            E_LinePoint.BottomLeftMiddleSplit => new GridPoint(SplitMinX, SplitMaxY),
            E_LinePoint.BottomMiddleMiddleSplit => new GridPoint(CentreX, SplitMaxY),
            E_LinePoint.BottomRightMiddleSplit => new GridPoint(SplitMaxX, SplitMaxY),


            //I will make it an exception later, but that means more code.
            //_ => throw new NotImplementedException()
            _ => new GridPoint(0, 0)
        };
    }

    public static bool ObjectsAreEqual<T>(T obj1, T obj2)
    {
        var obj1Serialized = GetJSON(obj1);
        var obj2Serialized = GetJSON(obj2);

        return obj1Serialized == obj2Serialized;
    }

    private static readonly JsonSerializerOptions JSON_WwriteOptions = new()
    {
        WriteIndented = true
    };

    //private static readonly JsonSerializerOptions JSON_ReadOptions = new()
    //{
    //    AllowTrailingCommas = true
    //};

    public static string GetJSON<T>(T obj1)
    {
        //JsonSerializerOptions options = new() { WriteIndented = false };
        //options.IgnoreReadOnlyFields = true;
        //options.IgnoreReadOnlyProperties = true;
        //options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        //JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = false };
        return JsonSerializer.Serialize(obj1, JSON_WwriteOptions);

    }


}
public class TileGeneratior
{
    public List<StockTile> WorkingTiles { get; set; }
    public TileGeneratior()
    {
        WorkingTiles = GenerateAllTiles();
    }
    public static List<StockTile> GenerateAllTiles()
    {
        List<StockTile> TileBuilder =
        [
            //Generate the sets of each tile
            //Name          Tiles   No lines 
            //Empty         1       0
            new StockTile(E_TileLineType.Empty, E_TileRotation.NoRitation, false, false),
            //Cross         1       2
            new StockTile(E_TileLineType.Cross, E_TileRotation.NoRitation, false, false),
            new StockTile(E_TileLineType.Cross, E_TileRotation.RotatedNinetyDegrees, false, false),
            new StockTile(E_TileLineType.Cross, E_TileRotation.NoRitation, false, true),
            //DiagonalCross 1       2
            new StockTile(E_TileLineType.DiagonalCross, E_TileRotation.NoRitation, false, false),
            //Bar           2       1
            new StockTile(E_TileLineType.Bar, E_TileRotation.NoRitation, false, false),
            //MidPoints     2       1       
            new StockTile(E_TileLineType.MidPoints, E_TileRotation.NoRitation, false, false),
            new StockTile(E_TileLineType.MidPoints, E_TileRotation.RotatedNinetyDegrees, false, false),
            //  Curved
            new StockTile(E_TileLineType.MidPoints, E_TileRotation.NoRitation, true, false),
            new StockTile(E_TileLineType.MidPoints, E_TileRotation.RotatedNinetyDegrees, true, false),
            //Corners       2       1       
            new StockTile(E_TileLineType.Corners, E_TileRotation.NoRitation, false, false),
            new StockTile(E_TileLineType.Corners, E_TileRotation.RotatedNinetyDegrees, false, false),
            //  Curved
            new StockTile(E_TileLineType.Corners, E_TileRotation.NoRitation, true, false),
            new StockTile(E_TileLineType.Corners, E_TileRotation.RotatedNinetyDegrees, true, false),
            //HalfMid       4       1       
            new StockTile(E_TileLineType.HalfMid, E_TileRotation.NoRitation, false, false),
            new StockTile(E_TileLineType.HalfMid, E_TileRotation.RotatedNinetyDegrees, false, false),
            new StockTile(E_TileLineType.HalfMid, E_TileRotation.RotatedOneEightyDegrees, false, false),
            new StockTile(E_TileLineType.HalfMid, E_TileRotation.RotatedTwoSeventyDegrees, false, false),
            //  Curved
            new StockTile(E_TileLineType.HalfMid, E_TileRotation.NoRitation, true, false),
            new StockTile(E_TileLineType.HalfMid, E_TileRotation.RotatedNinetyDegrees, true, false),
            new StockTile(E_TileLineType.HalfMid, E_TileRotation.RotatedOneEightyDegrees, true, false),
            new StockTile(E_TileLineType.HalfMid, E_TileRotation.RotatedTwoSeventyDegrees, true, false),
            //MidToCorner   8       1       
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.NoRitation, false, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedNinetyDegrees, false, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedOneEightyDegrees, false, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedTwoSeventyDegrees, false, false),
            //  Curved
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.NoRitation, true, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedNinetyDegrees, true, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedOneEightyDegrees, true, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedTwoSeventyDegrees, true, false),
            //  flipped
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.NoRitation, true, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedNinetyDegrees, true, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedOneEightyDegrees, true, false),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedTwoSeventyDegrees, true, false),
            //  flipped and Curved
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.NoRitation, true, true),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedNinetyDegrees, true, true),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedOneEightyDegrees, true, true),
            new StockTile(E_TileLineType.MidToCorner, E_TileRotation.RotatedTwoSeventyDegrees, true, true),
            //HalfCorners
            new StockTile(E_TileLineType.HalfCorners, E_TileRotation.NoRitation, false, false),
            new StockTile(E_TileLineType.HalfCorners, E_TileRotation.RotatedNinetyDegrees, false, false),
            new StockTile(E_TileLineType.HalfCorners, E_TileRotation.RotatedOneEightyDegrees, false, false),
            new StockTile(E_TileLineType.HalfCorners, E_TileRotation.RotatedTwoSeventyDegrees, false, false),
            //Curved
            new StockTile(E_TileLineType.HalfCorners, E_TileRotation.NoRitation, true, false),
            new StockTile(E_TileLineType.HalfCorners, E_TileRotation.RotatedNinetyDegrees, true, false),
            new StockTile(E_TileLineType.HalfCorners, E_TileRotation.RotatedOneEightyDegrees, true, false),
            new StockTile(E_TileLineType.HalfCorners, E_TileRotation.RotatedTwoSeventyDegrees, true, false),
        ];

        //All the curved lines can have a sharp counterpart  -MidPoints and Corners will get extra points the rest draw lines rather than curved.

        return TileBuilder;
    }
}
public class StockTile
{
    //This holds basic information on each tile, it knows the lines on the tile and their numbers, then their coodinates.
    public E_TileLineType TType { get; set; }
    //The weave will split one of the lines into two with a gap of the Line width plus a bit of space either side.
    //I want rid of these three.
    //The IsCurved will be set on the line the points will be a count on th elines object, and the TWeave set for each point.
    public bool IsCurved = false;
    //public bool FourPoint = false;
    //public E_TileWeave TWeave { get; set; }  //the werave for the centre - I didn't realise that corner pieces could have connections - if at a corner more than two points meet one pair need to be over the other under.
    public E_TileRotation TRotation { get; set; }
    public bool Flipped { get; set; }
    public GridLines MyGridLines { get; set; }   // These are the lines that I will finally draw - (NOTE: TC08/08/24 not the ones I will fianlly draw but the centrelines)
                                                 // The Weeave, Rotation and Flip determine the line that needs splitting for the
                                                 // Cross and Diagonal cross
    public StockTile(E_TileLineType ttype, E_TileRotation tilerotation, bool curved, bool flipped)
    {
        //Make them all once and reuse them.

        //For the corners Set the Weave when the tile is placed for the first time -- it depends on the corners that are next to the tile --and the opposite end weave (maybe)
        //I could set the weave on the Cross and diagonal cross when the stock tile is created.
        //   I want the weave propertu on the Grid point for each end of the Line. //
        TType = ttype;
        TRotation = tilerotation;
        Flipped = flipped;
        //Make the MyGridLines 
        MyGridLines = [];
        GridLine line;
        IsCurved = curved;
        if (TType == E_TileLineType.Cross)
        {
            //4 lines splits in the middles. (The two without a split could become one).
            //Each line has a start and an end point

            //I dont add any extra rotation to the cross and diagonal gross middle points as they can come from the rotation of the other end flipped

            //I should have had a lineGroup class / /fiddled it with the linegroupnumber paramiter, but it made it more awkward than it should have been.
            line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleMiddle, IsCurved, 0);
            line.MyLinePoints[^1].LinePointProperties.Weave = E_TileWeave.UnderWeave;
            MyGridLines.AddLine(line);
            line = new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomMiddle, IsCurved, 0);
            line.MyLinePoints[0].LinePointProperties.Weave = E_TileWeave.UnderWeave;
            MyGridLines.AddLine(line);


            line = new GridLine(TType, E_LinePoint.MiddleLeft, E_LinePoint.MiddleMiddle, IsCurved, 1);
            MyGridLines.AddLine(line);
            line = new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.MiddleRight, IsCurved, 1);
            MyGridLines.AddLine(line);

        }
        else if (TType == E_TileLineType.DiagonalCross)
        {
            //4 lines
            line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.MiddleMiddle, IsCurved, 0);
            //set the weave on one to under, let the other default to non orover.
            line.MyLinePoints[^1].LinePointProperties.Weave = E_TileWeave.UnderWeave;
            //line.SetUseDiagonalOffset(true); // a test case.  Diagonal needs to be a property of the Line not the point. this sets all the points in the line.
            MyGridLines.AddLine(line);
            line = new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomRight, IsCurved, 0);
            line.MyLinePoints[0].LinePointProperties.Weave = E_TileWeave.UnderWeave;
            MyGridLines.AddLine(line);

            line = new GridLine(TType, E_LinePoint.TopRight, E_LinePoint.MiddleMiddle, IsCurved, 1);
            MyGridLines.AddLine(line);
            line = new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomLeft, IsCurved, 1);
            MyGridLines.AddLine(line);
        }
        else if (TType == E_TileLineType.Bar)
        {
            //1 lines
            line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.BottomMiddle, IsCurved, 0);
            MyGridLines.AddLine(line);
        }
        else if (TType == E_TileLineType.MidPoints)//curve
        {
            if (IsCurved) //4 lines -- but the points are not on my grid  --
            {
                //2 curved lines
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleRight, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.MiddleLeft, E_LinePoint.BottomMiddle, IsCurved, 1);
                MyGridLines.AddLine(line);
            }
            else
            {
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.TopMiddleMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopMiddleMiddleSplit, E_LinePoint.MiddleRightsplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.MiddleRightsplit, E_LinePoint.MiddleRight, IsCurved, 0);
                MyGridLines.AddLine(line);

                line = new GridLine(TType, E_LinePoint.MiddleLeft, E_LinePoint.MiddleLeftsplit, IsCurved, 1);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.MiddleLeftsplit, E_LinePoint.BottomMiddleMiddleSplit, IsCurved, 1);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.BottomMiddleMiddleSplit, E_LinePoint.BottomMiddle, IsCurved, 1);
                MyGridLines.AddLine(line);
            }
        }
        else if (TType == E_TileLineType.Corners)//curve
        {
            if (IsCurved)
            {
                //2 curved lines
                line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.BottomLeft, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopRight, E_LinePoint.BottomRight, IsCurved, 1);
                MyGridLines.AddLine(line);
            }
            else
            {
                //6 straight lines
                line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.TopLeftMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopLeftMiddleSplit, E_LinePoint.BottomLeftMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.BottomLeftMiddleSplit, E_LinePoint.BottomLeft, IsCurved, 0);
                MyGridLines.AddLine(line);

                line = new GridLine(TType, E_LinePoint.TopRight, E_LinePoint.TopRightMiddleSplit, IsCurved, 1);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopRightMiddleSplit, E_LinePoint.BottomRightMiddleSplit, IsCurved, 1);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.BottomRightMiddleSplit, E_LinePoint.BottomRight, IsCurved, 1);
                MyGridLines.AddLine(line);
            }
        }
        else if (TType == E_TileLineType.HalfMid)//curve
        {
            if (IsCurved)
            {
                //1 Line
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleRight, IsCurved, 0);
                MyGridLines.AddLine(line);
            }
            else
            {
                //3 lines
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.TopMiddleMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopMiddleMiddleSplit, E_LinePoint.MiddleRightsplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.MiddleRightsplit, E_LinePoint.MiddleRight, IsCurved, 0);
                MyGridLines.AddLine(line);
            }
        }
        else if (TType == E_TileLineType.MidToCorner)//curve
        {
            if (IsCurved)
            {
                //1 Line
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.BottomRight, IsCurved, 0);
                MyGridLines.AddLine(line);
            }
            else
            {//2 lines
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleMiddle, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomRight, IsCurved, 0);
                MyGridLines.AddLine(line);
            }
        }
        else if (TType == E_TileLineType.HalfCorners)//curve
        {
            if (IsCurved)
            {
                //1 curved line
                line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.BottomLeft, IsCurved, 0);
                MyGridLines.AddLine(line);
            }
            else
            {
                //3 straight lines
                line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.TopLeftMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopLeftMiddleSplit, E_LinePoint.BottomLeftMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.BottomLeftMiddleSplit, E_LinePoint.BottomLeft, IsCurved, 0);
                MyGridLines.AddLine(line);
            }
        }
        else
        {
        }
        if ((tilerotation != E_TileRotation.NoRitation) || (flipped))
        {
            for (int i = 0; i < MyGridLines.Count; i++)
            {
                //TC 24/08/24 don't for get the properties.
                GridLine l = MyGridLines[i];
                MyGridLines[i] = l.RotateAndFlipClone(tilerotation, flipped);
                MyGridLines[i].LineGroupNumber = l.LineGroupNumber;
                MyGridLines[i].IsCurved = l.IsCurved;
                MyGridLines[i].TType = l.TType;
                for (int p = 0; p < MyGridLines[i].MyLinePoints.Count; p++)
                {
                    MyGridLines[i].MyLinePoints[p].LinePointProperties.Weave = l.MyLinePoints[p].LinePointProperties.Weave;
                }
            }
        }
    }
    //There will be 8 baseTiles - some have rotations 
}
public class WorkingTile(StockTile thisTile, int x, int y)
{
    //A holder for a stock tile, with a Canvas position.
    //This Object can draw itself, or Get the Test for it's SVG.

    //This is the tile on the Canvas - the one I draw - the drawing details are in the StockTile.
    public StockTile ThisTile = thisTile;
    //X an y cannot be changed - they are set on creation.
    //The tile can change - if it is rotated or the weave changes - it can be completely replaced.
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public string ToSVGPathString(E_OutlineType requestedPoints, bool capped, bool debugoutput)
    {
        string data = string.Empty;

        List<ListOfPointFForDrawing> pointFLists = GetLinePoints(requestedPoints, capped);
        if (debugoutput)
        {
            //Draw the outline of the Tile in Blue.
            data = SVGHelper.GetSVGTileDebugOutline(this);
        }

        foreach (ListOfPointFForDrawing pointFList in pointFLists)
        {
            if (pointFList.IsCurved) //there is a problem here, I can have a 4 point line :(
            {
                data += SVGHelper.ConvertCurve(pointFList);
                if (debugoutput)
                {
                    //The Line is Red 
                    //Put a vertical and horizontal Line on each point Green.
                    data += SVGHelper.GetSVGTileDebugPointline(pointFList);//this,
                }

            }
            else if (pointFList.Count == 2)
            {
                data += SVGHelper.ConvertTwoPointLine(pointFList);
                if (debugoutput)
                {

                }
            }
            else
            {
                Debug.Assert(false);
            }
        }
        return data;
    }
    private List<ListOfPointFForDrawing> GetOutline()
    {
        float XOffset = (float)TileProperties.Width * X;
        float YOffset = (float)TileProperties.Height * Y;
        float X1 = XOffset - (float)(TileProperties.Width / 2);
        float X2 = XOffset + (float)(TileProperties.Width / 2);
        float Y1 = YOffset - (float)(TileProperties.Height / 2);
        float Y2 = YOffset + (float)(TileProperties.Height / 2);
        List<ListOfPointFForDrawing> ReturnList = new();
        ListOfPointFForDrawing ThisLine = new(false, E_OutlineType.None);
        ThisLine.Add(new(X1, Y1));
        ThisLine.Add(new(X2, Y1));
        ReturnList.Add(ThisLine.RotateToDisplay(XOffset,YOffset));

        ThisLine = new(false, E_OutlineType.None);
        ThisLine.Add(new(X2, Y1));
        ThisLine.Add(new(X2, Y2));
        ReturnList.Add(ThisLine.RotateToDisplay(XOffset, YOffset));

        ThisLine = new(false, E_OutlineType.None);
        ThisLine.Add(new(X2, Y2));
        ThisLine.Add(new(X1, Y2));
        ReturnList.Add(ThisLine.RotateToDisplay(XOffset, YOffset));

        ThisLine = new(false, E_OutlineType.None);
        ThisLine.Add(new(X1, Y2));
        ThisLine.Add(new(X1, Y1));
        ReturnList.Add(ThisLine.RotateToDisplay(XOffset, YOffset));

        return ReturnList;
    }

    public void DrawLeftAndRight(Graphics g)
    {
        float XOffset = (float)TileProperties.Width * X;
        float YOffset = (float)TileProperties.Height * Y;

        //This worked but they weren't where I wanted them.
        //List<ListOfPointFForDrawing> Outline = GetOutline();
        //foreach (ListOfPointFForDrawing pointFList in Outline)
        //{
        //    if (pointFList.Count > 0)
        //    {
        //        var p = new Pen(Color.Blue);
        //        DrawFirstPointRect(g, p, pointFList);
        //        DrawLine(g, p, pointFList);
        //    }
        //}

        //just draw without rotation.
        g.DrawRectangle(new Pen(Color.Blue), XOffset, YOffset, (int)TileProperties.Width, (int)TileProperties.Height);
        List<ListOfPointFForDrawing> pointFLists = GetLinePoints(E_OutlineType.FullOutline, true);
        //pointFLists.AddRange(Outline);  // This works but draws them in the same colour.

        //I can do the same and use the Lists to generate the SVG file.
        foreach (ListOfPointFForDrawing pointFList in pointFLists)
        {
            if (pointFList.Count > 0)
            {
                var p = new Pen(TileProperties.LineColour);
                DrawFirstPointRect(g, p, pointFList);
                DrawLine(g, p, pointFList);
            }
        }
    }
    public static void DrawFirstPointRect(Graphics g, Pen pen, ListOfPointFForDrawing pointFList)
    {
        RectangleF rect = new(pointFList[0].X - 1, pointFList[0].Y - 1, 3, 3);
        g.DrawRectangle(pen, rect);
    }
    public static void DrawLine(Graphics g, ListOfPointFForDrawing pointFList)
    {
        if (pointFList.Count > 1) //Some have no points.
        {
            Pen pen = new(TileProperties.LineColour);

            if (pointFList.Count > 2)//Curves are only 4 point
            {
                g.DrawBezier(pen, pointFList[0], pointFList[1], pointFList[2], pointFList[3]);
            }
            else //Lines are only 2 point
            {
                g.DrawLine(pen, pointFList[0], pointFList[1]);
            }
        }
    }
    public static void DrawLine(Graphics g, Pen pen, ListOfPointFForDrawing pointFList)
    //This just redirects the draw to a curve or line.
    {
        if (pointFList.Count > 0) //Some have no points.
        {
            if (pointFList.IsCurved)//Curves are only 4 point
            {
                g.DrawBezier(pen, pointFList[0], pointFList[1], pointFList[2], pointFList[3]);
            }
            else //Lines are only 2 point
            {
                g.DrawLine(pen, pointFList[0], pointFList[1]);
            }
        }
    }

    //This gets ListOfPointFForDrawing it is a list of PointF with a property to let me know if the points are a curve.
    // I use PointF for drawing or SVG output.
    private List<ListOfPointFForDrawing> GetLinePoints(E_OutlineType requestedPoints, bool capped)
    {
        List<ListOfPointFForDrawing> ReturnPoints = [];
        PointF LeftStorePoint = new();
        PointF RightStorePoint = new();

        if (ThisTile is { })
        {
            if (ThisTile.MyGridLines.Count > 0)
            {
                GridLine thisline;
                float XOffset = (float)TileProperties.Width * X;
                float YOffset = (float)TileProperties.Height * Y;
                //g.DrawRectangle(new Pen(Color.Blue), XOffset, YOffset, (int)TileProperties.Width, (int)TileProperties.Height);

                int LastGroupNumber = -1;
                int TotalLinesInGroup = 0;
                int LineCounterForGroup = 0;
                for (int i = 0; i < ThisTile.MyGridLines.Count; i++)
                {
                    thisline = ThisTile.MyGridLines[i];
                    bool FlippDestinationPoints = false;
                    bool FlippStartPoints = false;
                    bool LastDestinationFlip = false;
                    int MidPointrotation = 0;
                    //Flip the Corner starts.
                    if (thisline.MyLinePoints[0].LinePointProperties.IsCorner)
                    {
                        FlippStartPoints = !FlippStartPoints;
                    }

                    //The LineGroup should be a class - I mat create one later but for now I'll get what I need
                    if (thisline.LineGroupNumber == LastGroupNumber)
                    {//use the flip of the last end flip for the start of a joined line
                        FlippStartPoints = LastDestinationFlip;
                        LineCounterForGroup += 1;//first line is 0
                    }
                    else
                    {
                        LastGroupNumber = thisline.LineGroupNumber;
                        TotalLinesInGroup = ThisTile.MyGridLines.CountGroup(LastGroupNumber);
                        LineCounterForGroup = 1;
                    }
                    //I'll test it and optimise it later
                    if (TotalLinesInGroup == 1)
                    {
                        FlippDestinationPoints = !FlippStartPoints;
                    }
                    else if (TotalLinesInGroup == 2)
                    {
                        if (LineCounterForGroup == 1)
                        {
                            FlippDestinationPoints = !FlippStartPoints;
                        }
                        else
                        {
                            //Line two of the diagonal cross needs the start needs flipping again
                            if (ThisTile.TType == E_TileLineType.DiagonalCross)
                            {
                                FlippStartPoints = !FlippStartPoints;
                            }
                            else
                            {
                                FlippDestinationPoints = !FlippStartPoints;
                            }
                        }
                    }
                    else if (TotalLinesInGroup == 3)
                    {
                        if (LineCounterForGroup == 1)
                        {
                            FlippDestinationPoints = FlippStartPoints;
                        }
                        else if (LineCounterForGroup == 2)
                        {
                            if (ThisTile.TType == E_TileLineType.Corners)
                            {
                                FlippStartPoints = !FlippStartPoints;
                            }
                            FlippDestinationPoints = !FlippStartPoints;
                        }
                        else
                        {
                            //MidPoints third line starts wrong
                            if (ThisTile.TType == E_TileLineType.MidPoints || ThisTile.TType == E_TileLineType.HalfMid)
                            {
                                FlippStartPoints = !FlippStartPoints;
                            }
                            FlippDestinationPoints = FlippStartPoints;
                        }
                    }
                    else
                    {
                        Debug.Assert(false); //I should raise and error.
                    }
                    //if the destination is MiddleMiddle I need the rotation of the next start line.
                    if (LineCounterForGroup < TotalLinesInGroup)
                    {
                        if (thisline.MyLinePoints[^1].ThisLinePoint == E_LinePoint.MiddleMiddle)
                        {
                            GPointOffsets OffsetStartPoints = new(ThisTile.MyGridLines[i + 1].MyLinePoints[^1].ThisLinePoint);
                            MidPointrotation = OffsetStartPoints.Rotation;
                        }
                    }

                    //Just try it with a centreline (no offset)
                    //I will need to draw Left then right then bridge any with Weave.
                    //No cap needed for Centrelines
                    ListOfPointFForDrawing pointFList = thisline.GetOutlinePointFList(XOffset, YOffset, FlippStartPoints, FlippDestinationPoints, E_OfsetPointHorizontal.Normal, MidPointrotation);

                    //var p = new Pen(Color.Red);
                    //DrawFirstPointRect(g, p, pointFList);
                    //DrawLine(g, p, pointFList);

                    ListOfPointFForDrawing pointFListLeft = thisline.GetOutlinePointFList(XOffset, YOffset, FlippStartPoints, FlippDestinationPoints, E_OfsetPointHorizontal.Left, MidPointrotation);

                    //p = new Pen(Color.Green);
                    //DrawFirstPointRect(g, p, pointFListLeft);
                    //DrawLine(g, p, pointFListLeft);

                    ListOfPointFForDrawing pointFListRight = thisline.GetOutlinePointFList(XOffset, YOffset, FlippStartPoints, FlippDestinationPoints, E_OfsetPointHorizontal.Right, MidPointrotation);


                    if (this.ThisTile.TType == E_TileLineType.MidToCorner && LineCounterForGroup == 1)
                    {

                        LeftStorePoint = pointFListLeft[^1];
                        RightStorePoint = pointFListRight[^1];
                    }

                    if (this.ThisTile.TType == E_TileLineType.MidToCorner && LineCounterForGroup == 2)
                    {

                        pointFListLeft[0] = LeftStorePoint;
                        pointFListRight[0] = RightStorePoint;
                    }
                    //p = new Pen(Color.Blue);
                    //DrawFirstPointRect(g, p, pointFListRight);
                    //DrawLine(g, p, pointFListRight);

                    //Check the first Line in the group for a cap, left and right will both have the same value.
                    ListOfPointFForDrawing pointFListCap = new(false, E_OutlineType.Cap);

                    //Check the line for a cap on both sides.
                    if (thisline.MyLinePoints[0].LinePointProperties.Weave == E_TileWeave.UnderWeave)
                    {
                        pointFListCap.Add(pointFListLeft[0]);
                        pointFListCap.Add(pointFListRight[0]);
                    }
                    if (thisline.MyLinePoints[^1].LinePointProperties.Weave == E_TileWeave.UnderWeave)
                    {
                        pointFListCap.Add(pointFListLeft[^1]);
                        pointFListCap.Add(pointFListRight[^1]);
                    }
                    //if (pointFListCap.Count > 0)
                    //{
                    //    DrawLine(g, p, pointFListCap);
                    //}

                    //This didn't work for the middle points :( 
                    //LastDestinationFlip = FlippDestinationPoints;

                    //Add the list of each set of lines.
                    if (requestedPoints == E_OutlineType.Left)
                    {
                        ReturnPoints.Add(pointFListLeft);//.RotateToDisplay(XOffset, YOffset)
                    }
                    else if (requestedPoints == E_OutlineType.Centre)
                    {
                        ReturnPoints.Add(pointFList);//.RotateToDisplay(XOffset, YOffset)
                    }
                    else if (requestedPoints == E_OutlineType.Right)
                    {
                        ReturnPoints.Add(pointFListRight);//.RotateToDisplay(XOffset, YOffset)
            }
                    else if (requestedPoints == E_OutlineType.FullOutline)
                    {
                        ReturnPoints.Add(pointFListLeft);//.RotateToDisplay(XOffset, YOffset)y
                        ReturnPoints.Add(pointFListRight);//.RotateToDisplay(XOffset, YOffset)
                    }
                    if (pointFListCap.Count > 0)
                    {
                        if (capped)
                        {
                            ReturnPoints.Add(pointFListCap.RotateToDisplay(XOffset, YOffset));
                        }
                    }
                }
            }
        }
        return ReturnPoints;
    }

}
// I have 2 line coordinate systems - 1 - 17 (there were 9) for the points and X,Y for the (same) draw points. These are added to the Canvas position and multiplied by the zoom.
public class GridLines() : List<GridLine>
{
    //GridLine
    public GridLine AddLine(GridLine line)
    {
        Add(line);
        return line;
    }
    //TC 17/08/24
    public int CountGroup(int group)
    {
        return this.Count(item => item.LineGroupNumber == group);
    }
}
public class ExtendedLinePoints : List<ExtendedLinePoint>
{
    public ExtendedLinePoint AddPoint(E_LinePoint thisPoint)
    {
        ExtendedLinePoint p = new(thisPoint);
        Add(p);
        return p;
    }
    public int AddPoints(params E_LinePoint[] points)
    {
        foreach (E_LinePoint p in points)
        {
            AddPoint(p);
        }
        return points.Length;
    }
}
public class ExtendedLinePoint(E_LinePoint thisPoint)
{
    //I thought it was a good Idea to use enums, but then I needed more data on the points so had to make a class to handle them - this was a bad idea.
    //I would have been better using something like the Tile Generator and kept all the data together. 
    public E_LinePoint ThisLinePoint { get; set; } = thisPoint;
    public LinePointObject LinePointProperties { get; set; } = new LinePointObject(thisPoint);
}
public class GridLine
{
    //I need to know the lines in a tile that are related, there can be up to three lines in a set and maybe two sets.
    public int LineGroupNumber { get; set; } = 0; //Some lines are related. 
    public bool IsCurved { get; set; }
    public ExtendedLinePoints MyLinePoints { get; set; } = [];
    //I finally gave in and added the tile type for the points in the on MidPoints and HalfMid, should have done this a while ago, but these two just draw wrong. 
    public E_TileLineType TType { get; set; } = E_TileLineType.Empty;

    public GridLine(List<E_LinePoint> points)
    {
        foreach (E_LinePoint p in points)
        {
            MyLinePoints.AddPoint(p);
        }
        if (MyLinePoints.Count > 2)
        {
            IsCurved = true;
        }
    }
    public GridLine(E_TileLineType ttype, params E_LinePoint[] points)
    {
        //TC 10/08/24 curved have 4 points lines have 2!!
        if (MyLinePoints.AddPoints(points) > 2)
        {
            IsCurved = true;
        }
        TType = ttype;
    }
    public GridLine(E_TileLineType ttype, E_LinePoint lineFrom, E_LinePoint lineTo, bool iscurved, int linegroupnumber)
    //if IsCurved then we have a control point for each point
    // the first point has a control point in the centre, the second control point is the same as the end point.
    {
        IsCurved = iscurved;
        TType = ttype;
        if (lineFrom > lineTo)//Sort them.
        {
            (lineFrom, lineTo) = (lineTo, lineFrom);  // lineTo and lineFrom are 1 to 9 Top left to bottom right, so the Ccordinates are naturally sorted.
        }
        if (IsCurved)
        {
            //The control points are added for curved, the first is in the centre of the tile, the last is the same as the endpoint.
            MyLinePoints.AddPoints(lineFrom, E_LinePoint.MiddleMiddle, lineTo, lineTo);
        }
        else
        {
            MyLinePoints.AddPoints(lineFrom, lineTo);
        }
        LineGroupNumber = linegroupnumber;
    }
    //public ListOfPointFForDrawing GetCap(E_OutlineCap outlineCap)
    //{
    //    ListOfPointFForDrawing pointFList = [];
    //    return pointFList;

    //}
    public ListOfPointFForDrawing GetOutlinePointFList(float xoffset, float yoffset, bool flipStart, bool flipDestination, E_OfsetPointHorizontal ofsetpointhorizontal, int midPointRotation)
    //= E_OfsetPointHorizontal.Normal took off the defaut 
    //Added midPointRotation - if (EndPoint == E_LinePoint.MiddleMiddle) I need to rotate it at I would the next line.
    {
        int StartPointOffsetNumber = ofsetpointhorizontal switch
        {
            E_OfsetPointHorizontal.Left => 11,
            //E_OfsetPointHorizontal.Normal => 12,
            E_OfsetPointHorizontal.Right => 13,
            _ => 12
        };
        int EndPointOffsetNumber = StartPointOffsetNumber;

        //These contain ThisLinePoint that is the E_LinePoint.
        ExtendedLinePoint StartPointEX = MyLinePoints[0];
        ExtendedLinePoint EndPointEX = MyLinePoints[^1];

        //get the Weave 
        E_TileWeave w1 = StartPointEX.LinePointProperties.Weave;
        E_TileWeave w2 = EndPointEX.LinePointProperties.Weave;

        //For underweave Move closer to the origin by two rows (5 per row).
        E_LinePoint StartPoint = StartPointEX.ThisLinePoint;
        if (w1 == E_TileWeave.UnderWeave) // Only midpoints or  corners on the edge can use underweave (if more than two lines meet at the corner).
        {
            StartPointOffsetNumber += 10;
        }
        E_LinePoint EndPoint = EndPointEX.ThisLinePoint;
        if (w2 == E_TileWeave.UnderWeave)
        {
            EndPointOffsetNumber += 10;
        }

        //Get the Offset Points for the Start and End -- I may need them for the midpoints as well
        GPointOffsets OffsetStartPoints = new(StartPoint);// I can just make thse as they are standard // now I need to figure out where to move the weave. I wil remove them from the LinePointObject object.
        GPointOffsets OffsetEndPoints = new(EndPoint);

        int StartPointRotation = OffsetStartPoints.Rotation;
        int EndPointRotation = OffsetEndPoints.Rotation;
        //the MiddleMiddle does not have it's own ritation so copy it from the other end. (it will be rotated and flipped like the other End Points)
        if (StartPoint == E_LinePoint.MiddleMiddle)
        {
            StartPointRotation = EndPointRotation + 180; // as it is inside  it does not want to be rotated + 180;
        }
        else if (EndPoint == E_LinePoint.MiddleMiddle)
        {
            EndPointRotation = midPointRotation;// it became hard work so I passed it in. StartPointRotation + 180;//+ 180;
        }

        //Rotate the startPoints
        GridPoint centerPoint = OffsetStartPoints[12];// StartPointEX.LinePointProperties.GetOfsetPoint(12);
        //Flip then rotate.
        if (flipStart)
        {
            OffsetStartPoints.FlipHorizontally();
        }
        OffsetStartPoints.RotateAllPoints(centerPoint, StartPointRotation);

        centerPoint = OffsetEndPoints[12];
        if (flipDestination)
        {
            OffsetEndPoints.FlipHorizontally();
        }
        OffsetEndPoints.RotateAllPoints(centerPoint, EndPointRotation);

        //All the Lines need to shift by this X and Y 
        GridPoint StartPointOffSetCoordinates = OffsetStartPoints[StartPointOffsetNumber];
        GridPoint EndPointOffSetCoordinates = OffsetEndPoints[EndPointOffsetNumber];

        float P1X = (float)StartPointOffSetCoordinates.X;
        float P1Y = (float)StartPointOffSetCoordinates.Y;
        float P2X = (float)EndPointOffSetCoordinates.X;
        float P2Y = (float)EndPointOffSetCoordinates.Y;

        if (!IsCurved && this.TType == E_TileLineType.MidToCorner)
        {
            if (ofsetpointhorizontal == E_OfsetPointHorizontal.Left || ofsetpointhorizontal == E_OfsetPointHorizontal.Right)
            {
                //Debug.Assert(false);
                //Debug.Print($"{ofsetpointhorizontal} P1X {P1X}, P1Y {P1Y}, P2X {P2X}, P2Y {P2Y}");
                //    Debug.Assert(false);
                //Rounding Error made them not always equal so I use greater than or less than.

                //This fixes the End points of Line 1.
                //The Start points of the second line need to be the End points of the first Line - I can't fix that here as I don't know then - so I will do it in  the calling routine. (only the Left and right need fixing.)
                if (P1Y > (float)TileProperties.Width - 1 || P1Y < 1)
                {
                    P2X = P1X;
                }
                if (P1X > (float)TileProperties.Width - 1 || P1X < 1)
                {
                    P2Y = P1Y;
                }
            }
        }
        ListOfPointFForDrawing pointFList = new(false, E_OutlineType.Centre); //this is a List<PointF> pointFList = []; with a IsCurved property.
        //Correct for Left and Right, I use this for debugging.
        if (ofsetpointhorizontal == E_OfsetPointHorizontal.Left)
        {
            pointFList.OutlineType = E_OutlineType.Left;
        }
        else if (ofsetpointhorizontal == E_OfsetPointHorizontal.Right)
        {
            pointFList.OutlineType = E_OutlineType.Right;
        }


        pointFList.Add(new PointF(P1X + xoffset, P1Y + yoffset));
        if (MyLinePoints.Count is < 2 or > 4)
        {
            Debug.Assert(false);
        }
        if (IsCurved)
        {
            //Point 3 is the MiddleMiddle offset with the StartPointOffsetNumber.
            GPointOffsets OffsetMiddlePoints = new(E_LinePoint.MiddleMiddle);
            centerPoint = OffsetMiddlePoints[12];
            if (flipStart)
            {
                OffsetMiddlePoints.FlipHorizontally();
            }
            OffsetMiddlePoints.RotateAllPoints(centerPoint, OffsetStartPoints.Rotation);
            GridPoint MiddlePointOffSetCoordinates = OffsetMiddlePoints[StartPointOffsetNumber];  // use the same point as the start.
            float C1X = (float)MiddlePointOffSetCoordinates.X;
            float C1Y = (float)MiddlePointOffSetCoordinates.Y;
            //Point 2 P2X and P2Y are set above.
            if (ofsetpointhorizontal == E_OfsetPointHorizontal.Left || ofsetpointhorizontal == E_OfsetPointHorizontal.Right)
            {
                if (this.TType == E_TileLineType.MidPoints || this.TType == E_TileLineType.HalfMid)//
                {
                    //Rounding Error made them not always equal so I use greater than or less than.
                    if (P2X > (float)TileProperties.Width - 1 || P2X < 1)
                    {
                        C1Y = P2Y;
                    }
                    if (P2Y > (float)TileProperties.Height - 1 || P2Y < 1)
                    {
                        C1X = P2X;
                    }
                }
                if (this.TType == E_TileLineType.Corners || this.TType == E_TileLineType.HalfCorners)
                //These need the middle point to be in the middle of the tile
                {
                    //if the width > height   set the Y to tile middle
                    float GlyphWidth = (float)Math.Abs(EndPointOffSetCoordinates.X - StartPointOffSetCoordinates.X);
                    float GlyphHeight = (float)Math.Abs(EndPointOffSetCoordinates.Y - StartPointOffSetCoordinates.Y);
                    if (GlyphWidth > GlyphHeight)
                    {
                        C1X = (float)TileProperties.Width / 2;
                    }
                    //If the height is > Width Set the X to the tile middle
                    else
                    {
                        C1Y = (float)TileProperties.Height / 2;
                    }
                }
            }
            pointFList.Add(new PointF(C1X + xoffset, C1Y + yoffset));
            //The last point is just the same as final EndPoint + OffSetCoordinates
            pointFList.Add(new PointF((float)EndPointOffSetCoordinates.X + xoffset, (float)EndPointOffSetCoordinates.Y + yoffset));
            pointFList.IsCurved = true;
        }
        pointFList.Add(new PointF(P2X + xoffset, P2Y + yoffset));
        return pointFList;
    }

    public GridLine RotateAndFlipClone(E_TileRotation tileRotation, bool tileflip)
    {
        List<E_LinePoint> l = [];
        for (int i = 0; i < MyLinePoints.Count; i++)
        {
            l.Add(FlipPoint(RotatePoint(MyLinePoints[i].ThisLinePoint, tileRotation), tileflip));
        }
        GridLine ReturlLine = new(l);
        return ReturlLine;
    }
    private static E_LinePoint RotatePoint(E_LinePoint thisline, E_TileRotation tileRotation)
    {
        //It was easier to write a single rotate and call it multiple times.
        int RotCount = tileRotation switch
        {
            E_TileRotation.NoRitation => 0,
            E_TileRotation.RotatedNinetyDegrees => 1,
            E_TileRotation.RotatedOneEightyDegrees => 2,
            E_TileRotation.RotatedTwoSeventyDegrees => 3,
            _ => 0
        };

        for (int i = 0; i < RotCount; i++)
        {
            thisline = RotatePointOnce(thisline);
        };

        return thisline;
    }
    private static E_LinePoint RotatePointOnce(E_LinePoint thisline)
    {
        return thisline switch
        {
            E_LinePoint.None => E_LinePoint.None,
            E_LinePoint.TopLeft => E_LinePoint.TopRight,
            E_LinePoint.TopMiddle => E_LinePoint.MiddleRight,
            E_LinePoint.TopRight => E_LinePoint.BottomRight,

            E_LinePoint.TopLeftMiddleSplit => E_LinePoint.TopRightMiddleSplit,
            E_LinePoint.TopMiddleMiddleSplit => E_LinePoint.MiddleRightsplit,
            E_LinePoint.TopRightMiddleSplit => E_LinePoint.BottomRightMiddleSplit,


            E_LinePoint.MiddleLeft => E_LinePoint.TopMiddle,
            E_LinePoint.MiddleMiddle => E_LinePoint.MiddleMiddle,
            E_LinePoint.MiddleRight => E_LinePoint.BottomMiddle,

            E_LinePoint.MiddleLeftsplit => E_LinePoint.TopMiddleMiddleSplit,
            E_LinePoint.MiddleRightsplit => E_LinePoint.BottomMiddleMiddleSplit,

            E_LinePoint.BottomLeft => E_LinePoint.TopLeft,
            E_LinePoint.BottomMiddle => E_LinePoint.MiddleLeft,
            E_LinePoint.BottomRight => E_LinePoint.BottomLeft,

            E_LinePoint.BottomLeftMiddleSplit => E_LinePoint.TopLeftMiddleSplit,
            E_LinePoint.BottomMiddleMiddleSplit => E_LinePoint.MiddleLeftsplit,
            E_LinePoint.BottomRightMiddleSplit => E_LinePoint.BottomLeftMiddleSplit,
            //I will make it an exception later, but that means more code.
            //_ => throw new NotImplementedException()
            _ => E_LinePoint.None
        }; ;
    }
    private static E_LinePoint FlipPoint(E_LinePoint thisline, bool tileflip)
    {
        if (tileflip != true) { return thisline; }
        return thisline switch
        {
            E_LinePoint.None => E_LinePoint.None,
            E_LinePoint.TopLeft => E_LinePoint.TopRight,
            E_LinePoint.TopMiddle => E_LinePoint.TopMiddle,
            E_LinePoint.TopRight => E_LinePoint.TopLeft,
            E_LinePoint.MiddleLeft => E_LinePoint.MiddleRight,
            E_LinePoint.MiddleMiddle => E_LinePoint.MiddleMiddle,
            E_LinePoint.MiddleRight => E_LinePoint.MiddleLeft,
            E_LinePoint.BottomLeft => E_LinePoint.BottomRight,
            E_LinePoint.BottomMiddle => E_LinePoint.BottomMiddle,
            E_LinePoint.BottomRight => E_LinePoint.BottomLeft,
            //I will make it an exception later, but that means more code.
            //_ => throw new NotImplementedException()
            _ => E_LinePoint.None
        };
    }
    public static GridPoint GetGpoint(E_LinePoint thisline)
    {
        //GridPoint x;
        double MinX = 0;
        double MinY = 0;
        double MaxX = TileProperties.Width;
        double CentreX = MaxX / 2;
        double MaxY = TileProperties.Height;
        double CentreY = MaxY / 2;

        double SplitMinX = CentreX / 2;
        double SplitMaxX = CentreX + SplitMinX;
        double SplitMinY = CentreY / 2;
        double SplitMaxY = CentreY + SplitMinY;

        return thisline switch
        {
            E_LinePoint.None => new GridPoint(MinX, MinY),

            E_LinePoint.TopLeft => new GridPoint(MinX, MinY),
            E_LinePoint.TopMiddle => new GridPoint(CentreX, MinY),
            E_LinePoint.TopRight => new GridPoint(MaxX, MinY),

            //I didn't like this approach.
            //E_LinePoint.TopLeftMiddleSplit => new GridPoint(MinX, MinY).Merge(new GridPoint(CentreX, CentreY)),
            E_LinePoint.TopLeftMiddleSplit => new GridPoint(SplitMinX, SplitMinY),
            E_LinePoint.TopMiddleMiddleSplit => new GridPoint(CentreX, SplitMinY),
            E_LinePoint.TopRightMiddleSplit => new GridPoint(SplitMaxX, SplitMinY),

            E_LinePoint.MiddleLeft => new GridPoint(MinX, CentreY),
            E_LinePoint.MiddleMiddle => new GridPoint(CentreX, CentreY),
            E_LinePoint.MiddleRight => new GridPoint(MaxX, CentreY),

            E_LinePoint.MiddleLeftsplit => new GridPoint(SplitMinX, CentreY),
            E_LinePoint.MiddleRightsplit => new GridPoint(SplitMaxX, CentreY),

            E_LinePoint.BottomLeftMiddleSplit => new GridPoint(SplitMinX, SplitMaxY),
            E_LinePoint.BottomMiddleMiddleSplit => new GridPoint(CentreX, SplitMaxY),
            E_LinePoint.BottomRightMiddleSplit => new GridPoint(SplitMaxX, SplitMaxY),

            E_LinePoint.BottomLeft => new GridPoint(MinX, MaxY),
            E_LinePoint.BottomMiddle => new GridPoint(CentreX, MaxY),
            E_LinePoint.BottomRight => new GridPoint(MaxX, MaxY),

            //I will make it an exception later, but that means more code.
            //_ => throw new NotImplementedException()
            _ => new GridPoint(0, 0)
        };
        //return x;
    }
    public GridLine Clone()
    {
        //Make a new GridLine using the GridPoint s from this item.
        //GridLine Copy = new GridLine(GridPoint.Item1, GridPoint.Item2, GridPoint.Item3, GridPoint.Item4);
        //return Copy;

        List<E_LinePoint> l = [];
        for (int i = 0; i < MyLinePoints.Count; i++)
        {
            l.Add(MyLinePoints[i].ThisLinePoint);
        }
        return new GridLine(l);
    }
}
public class GridPoint(double x, double y)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public GridPoint Merge(GridPoint withPoint)
    {
        double MidX = (X + withPoint.X) / 2;
        double MidY = (Y + withPoint.Y) / 2;
        return new GridPoint(MidX, MidY);

    }
    public GridPoint Clone() { return new GridPoint(X, Y); }
    public void Flip(bool Horizontal)
    {
        if (Horizontal)
        {
            X = (Math.Abs(X - TileProperties.Width));
        }
        else
        {
            Y = (Math.Abs(Y - TileProperties.Height));
        }
    }

    #region"PointRotating"
    //public void RotatePoint45()
    //{
    //    RotatePoint(45);
    //}
    //public void RotatePoint90()
    //{
    //    RotatePoint(90);
    //}
    //private void RotatePoint180()
    //{
    //    RotatePoint(180);
    //}
    //public void RotatePoint270()
    //{
    //    RotatePoint(270);
    //}
    public void RotatePointRoundZeroZero(double angleInDegrees)
    {
        GridPoint centerPoint = new(0, 0);
        RotatePoint(centerPoint, angleInDegrees);
    }
    public void RotatePoint(double angleInDegrees)
    {
        GridPoint centerPoint = new(TileProperties.Width / 2, TileProperties.Height / 2);
        RotatePoint(centerPoint, angleInDegrees);
    }
    public void RotatePoint(GridPoint centerPoint, double angleInDegrees)
    {
        double angleInRadians = angleInDegrees * (Math.PI / 180);
        double cosTheta = Math.Cos(angleInRadians);
        double sinTheta = Math.Sin(angleInRadians);
        //Copy the point as I don't want to use the modified X in the second calculation.
        GridPoint pointToRotate = new(X, Y);
        //return new GridPoint(
        X = (cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X);
        Y = (sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y);
    }
    #endregion

}

//I need t know if the PointF is part of a curved line.
//I added some more properties to aid with the debugging.
public class ListOfPointFForDrawing(bool iscurved, E_OutlineType outlinetype) : List<PointF>
{
    public bool IsCurved { get; set; } = iscurved;
    public E_OutlineType OutlineType { get; set; } = outlinetype;
    //This is only here because I cannot inherit PointF.
    public ListOfPointFForDrawing RotateToDisplay(float xoffset, float yoffset)
    {
        PointF CentrePoint = new(xoffset + ((float)TileProperties.Width / 2), yoffset + ((float)TileProperties.Height / 2));
        ListOfPointFForDrawing ReturnPoints = new(this.IsCurved, this.OutlineType);
        for (int i = 0; i < this.Count; i++)
        {
            //Get the Point rotated 45 degrees to display or output to SVG.
            PointF NewPoint = RotatePoint(CentrePoint, this[i], 45f);
            ReturnPoints.Add(NewPoint);
        }
        return ReturnPoints;
    }
    private PointF RotatePoint(PointF centerPoint, PointF pointToRotate, double angleInDegrees)
    {
        double angleInRadians = angleInDegrees * (Math.PI / 180);
        double cosTheta = Math.Cos(angleInRadians);
        double sinTheta = Math.Sin(angleInRadians);

        //return new GridPoint(
        PointF ReturnPoint = new();
        ReturnPoint.X = (float)(cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X);
        ReturnPoint.Y = (float)(sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y);
        return ReturnPoint;
    }
    //I need to move this.
    static double DistanceBetweenPoints(PointF p1, PointF p2)
    {
        double dx = p2.X - p1.X;
        double dy = p2.Y - p1.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

}
