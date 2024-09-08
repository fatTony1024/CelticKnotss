using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelticKnots;

public class GridLines() : List<GridLine> {
    //GridLine
    public GridLine AddLine(GridLine line) {
        Add(line);
        return line;
    }
    //TC 17/08/24
    public int CountGroup(int group) {
        return this.Count(item => item.LineGroupNumber == group);
    }
}



public class GridLine {
    //I need to know the lines in a tile that are related, there can be up to three lines in a set and maybe two sets.
    public int LineGroupNumber { get; set; } = 0; //Some lines are related. 
    public bool IsCurved { get; set; }
    public ExtendedLinePoints MyLinePoints { get; set; } = [];
    //I finally gave in and added the tile type for the points in the on MidPoints and HalfMid, should have done this a while ago, but these two just draw wrong. 
    public E_TileLineType TType { get; set; } = E_TileLineType.Empty;

    public GridLine(List<E_LinePoint> points) {
        foreach (E_LinePoint p in points) {
            MyLinePoints.AddPoint(p);
        }
        if (MyLinePoints.Count > 2) {
            IsCurved = true;
        }
    }
    public GridLine(E_TileLineType ttype, params E_LinePoint[] points) {
        //TC 10/08/24 curved have 4 points lines have 2!!
        if (MyLinePoints.AddPoints(points) > 2) {
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
        if (IsCurved) {
            //The control points are added for curved, the first is in the centre of the tile, the last is the same as the endpoint.
            MyLinePoints.AddPoints(lineFrom, E_LinePoint.MiddleMiddle, lineTo, lineTo);
        }
        else {
            MyLinePoints.AddPoints(lineFrom, lineTo);
        }
        LineGroupNumber = linegroupnumber;
    }
    //public ListOfPointFForDrawing GetCap(E_OutlineCap outlineCap)
    //{
    //    ListOfPointFForDrawing pointFList = [];
    //    return pointFList;

    //}



    public ListOfPointFForDrawing GetOutlinePointFList(bool flipStart, bool flipDestination, E_OffsetPointHorizontal ofsetpointhorizontal, int midPointRotation)
    //= E_OffsetPointHorizontal.Normal took off the defaut 
    //Added midPointRotation - if (EndPoint == E_LinePoint.MiddleMiddle) I need to rotate it at I would the next line.
    {
        int StartPointOffsetNumber = ofsetpointhorizontal switch {
            E_OffsetPointHorizontal.Left => 11,
            //E_OffsetPointHorizontal.Normal => 12,
            E_OffsetPointHorizontal.Right => 13,
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
        if (w2 == E_TileWeave.UnderWeave) {
            EndPointOffsetNumber += 10;
        }

        //Get the Offset Points for the Start and End -- I may need them for the midpoints as well
        GPointOffsets OffsetStartPoints = new(StartPoint);// I can just make thse as they are standard // now I need to figure out where to move the weave. I wil remove them from the LinePointInformationObject object.
        GPointOffsets OffsetEndPoints = new(EndPoint);

        int StartPointRotation = OffsetStartPoints.Rotation;
        int EndPointRotation = OffsetEndPoints.Rotation;
        //the MiddleMiddle does not have it's own ritation so copy it from the other end. (it will be rotated and flipped like the other End Points)
        if (StartPoint == E_LinePoint.MiddleMiddle) {
            StartPointRotation = EndPointRotation + 180; // as it is inside  it does not want to be rotated + 180;
        }
        else if (EndPoint == E_LinePoint.MiddleMiddle) {
            EndPointRotation = midPointRotation;// it became hard work so I passed it in. StartPointRotation + 180;//+ 180;
        }

        //Rotate the startPoints
        GridPoint centerPoint = OffsetStartPoints[12];// StartPointEX.LinePointProperties.GetOfsetPoint(12);
        //Flip then rotate.
        if (flipStart) {
            OffsetStartPoints.FlipHorizontally();
        }
        OffsetStartPoints.RotateAllPoints(centerPoint, StartPointRotation);

        centerPoint = OffsetEndPoints[12];
        if (flipDestination) {
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

        if (!IsCurved && this.TType == E_TileLineType.MidToCorner) {
            if (ofsetpointhorizontal == E_OffsetPointHorizontal.Left || ofsetpointhorizontal == E_OffsetPointHorizontal.Right) {
                //Debug.Assert(false);
                //Debug.Print($"{ofsetpointhorizontal} P1X {P1X}, P1Y {P1Y}, P2X {P2X}, P2Y {P2Y}");
                //    Debug.Assert(false);
                //Rounding Error made them not always equal so I use greater than or less than.

                //This fixes the End points of Line 1.
                //The Start points of the second line need to be the End points of the first Line - I can't fix that here as I don't know then - so I will do it in  the calling routine. (only the Left and right need fixing.)
                if (P1Y > (float)TileProperties.Width - 1 || P1Y < 1) {
                    P2X = P1X;
                }
                if (P1X > (float)TileProperties.Width - 1 || P1X < 1) {
                    P2Y = P1Y;
                }
            }
        }
        ListOfPointFForDrawing pointFList = new(false, E_OutlineType.Centre, TileProperties.GlyphPen); //this is a List<PointF> pointFList = []; with a IsCurved property.
        //Correct for Left and Right, I use this for debugging.
        if (ofsetpointhorizontal == E_OffsetPointHorizontal.Left) {
            pointFList.OutlineType = E_OutlineType.Left;
        }
        else if (ofsetpointhorizontal == E_OffsetPointHorizontal.Right) {
            pointFList.OutlineType = E_OutlineType.Right;
        }


        //pointFList.Add(new PointF(P1X + xoffset, P1Y + yoffset));
        pointFList.Add(new PointF(P1X, P1Y));
        if (MyLinePoints.Count is < 2 or > 4) {
            Debug.Assert(false);
        }
        if (IsCurved) {
            //Point 3 is the MiddleMiddle offset with the StartPointOffsetNumber.
            GPointOffsets OffsetMiddlePoints = new(E_LinePoint.MiddleMiddle);
            centerPoint = OffsetMiddlePoints[12];
            if (flipStart) {
                OffsetMiddlePoints.FlipHorizontally();
            }
            OffsetMiddlePoints.RotateAllPoints(centerPoint, OffsetStartPoints.Rotation);
            GridPoint MiddlePointOffSetCoordinates = OffsetMiddlePoints[StartPointOffsetNumber];  // use the same point as the start.
            float C1X = (float)MiddlePointOffSetCoordinates.X;
            float C1Y = (float)MiddlePointOffSetCoordinates.Y;
            //Point 2 P2X and P2Y are set above.
            if (ofsetpointhorizontal == E_OffsetPointHorizontal.Left || ofsetpointhorizontal == E_OffsetPointHorizontal.Right) {
                if (this.TType == E_TileLineType.MidPoints || this.TType == E_TileLineType.HalfMid)//
                {
                    //Rounding Error made them not always equal so I use greater than or less than.
                    if (P2X > (float)TileProperties.Width - 1 || P2X < 1) {
                        C1Y = P2Y;
                    }
                    if (P2Y > (float)TileProperties.Height - 1 || P2Y < 1) {
                        C1X = P2X;
                    }
                }

                //float GlyphWidth = (float)Math.Abs(EndPointOffSetCoordinates.X - StartPointOffSetCoordinates.X);
                //float GlyphHeight = (float)Math.Abs(EndPointOffSetCoordinates.Y - StartPointOffSetCoordinates.Y);
                //if (this.TType == E_TileLineType.Corners || this.TType == E_TileLineType.HalfCorners)
                ////These need the middle point to be in the middle of the tile
                //{
                //    //if the width > height   set the Y to tile middle

                //    if (GlyphWidth > GlyphHeight) {
                //        C1X = (float)TileProperties.Width / 2;
                //    }
                //    //If the height is > Width Set the X to the tile middle
                //    else {
                //        C1Y = (float)TileProperties.Height / 2;
                //    }


                //}
            }
            if (IsCurved && (this.TType == E_TileLineType.HalfCorners || this.TType == E_TileLineType.Corners)) {
                // the second lines on Corners needs the other point :(  I'll need to find a better way to do this. but for now....

                // this may be a better way for some of the other curves
                if ((StartPoint == E_LinePoint.TopRight && EndPoint == E_LinePoint.BottomRight) || (StartPoint == E_LinePoint.BottomRight && EndPoint == E_LinePoint.BottomLeft)) {
                    pointFList.Add(TriangulateThirdPoint(StartPointOffSetCoordinates, EndPointOffSetCoordinates, true));
                }
                else {
                    pointFList.Add(TriangulateThirdPoint(StartPointOffSetCoordinates, EndPointOffSetCoordinates, false));
                }
            }
            else {
                pointFList.Add(new PointF(C1X, C1Y));
            }

            //The last point is just the same as final EndPoint + OffSetCoordinates
            //pointFList.Add(new PointF((float)EndPointOffSetCoordinates.X + xoffset, (float)EndPointOffSetCoordinates.Y + yoffset));
            pointFList.Add(new PointF((float)EndPointOffSetCoordinates.X, (float)EndPointOffSetCoordinates.Y));
            pointFList.IsCurved = true;
        }
        //pointFList.Add(new PointF(P2X + xoffset, P2Y + yoffset));
        pointFList.Add(new PointF(P2X, P2Y));


        if (CanvasProperties.RotateOutput) {
            //I do have a routine that rotates PointF
            for (int i = 0; i < pointFList.Count; i++) {
                PointF p = pointFList[i];
                GridPoint gp = new(p.X, p.Y);
                gp.RotatePoint(45);
                p.X = (float)gp.X;
                p.Y = (float)gp.Y;
                pointFList[i] = p;
            }
        }

        return pointFList;
    }
    //create a function to intersect two lines and return a PointF  

    private PointF TriangulateThirdPoint(GridPoint point2, GridPoint point1, bool returnPoint1) {
        //given two GridPoint points on the hypotinuse of a right angle triangle get the third possible two points   

        // Calculate the midpoint of the hypotenuse
        double midX = (point1.X + point2.X) / 2;
        double midY = (point1.Y + point2.Y) / 2;

        // Calculate the vector from point1 to point2
        double dx = point2.X - point1.X;
        double dy = point2.Y - point1.Y;

        // Calculate the length of the hypotenuse
        double hypotenuseLength = Math.Sqrt(dx * dx + dy * dy);

        // Calculate the length of the legs of the right-angle triangle
        double legLength = hypotenuseLength / Math.Sqrt(2);

        // Calculate the two possible third points
        PointF thirdPoint1 = new PointF {
            X = (float)midX + (float)dy / 2,
            Y = (float)midY - (float)dx / 2
        };

        PointF thirdPoint2 = new PointF {
            X = (float)midX - (float)dy / 2,
            Y = (float)midY + (float)dx / 2
        };
        //I always ritate clockwise so we need point 2.
        if (returnPoint1) {
            return thirdPoint1;
        }
        return thirdPoint2;

    }
    public GridLine RotateAndFlipClone(E_TileRotation tileRotation, bool tileflip) {
        List<E_LinePoint> l = [];
        for (int i = 0; i < MyLinePoints.Count; i++) {
            l.Add(FlipPoint(RotatePoint(MyLinePoints[i].ThisLinePoint, tileRotation), tileflip));
        }
        GridLine ReturlLine = new(l);
        return ReturlLine;
    }
    private static E_LinePoint RotatePoint(E_LinePoint thisline, E_TileRotation tileRotation) {
        //It was easier to write a single rotate and call it multiple times.
        int RotCount = tileRotation switch {
            E_TileRotation.NoRitation => 0,
            E_TileRotation.RotatedNinetyDegrees => 1,
            E_TileRotation.RotatedOneEightyDegrees => 2,
            E_TileRotation.RotatedTwoSeventyDegrees => 3,
            _ => 0
        };

        for (int i = 0; i < RotCount; i++) {
            thisline = RotatePointOnce(thisline);
        };

        return thisline;
    }
    private static E_LinePoint RotatePointOnce(E_LinePoint thisline) {
        return thisline switch {
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
    private static E_LinePoint FlipPoint(E_LinePoint thisline, bool tileflip) {
        if (tileflip != true) { return thisline; }
        return thisline switch {
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
    public static GridPoint GetGpoint(E_LinePoint thisline) {
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

        return thisline switch {
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
    public GridLine Clone() {
        //Make a new GridLine using the GridPoint s from this item.
        //GridLine Copy = new GridLine(GridPoint.Item1, GridPoint.Item2, GridPoint.Item3, GridPoint.Item4);
        //return Copy;

        List<E_LinePoint> l = [];
        for (int i = 0; i < MyLinePoints.Count; i++) {
            l.Add(MyLinePoints[i].ThisLinePoint);
        }
        return new GridLine(l);
    }
}