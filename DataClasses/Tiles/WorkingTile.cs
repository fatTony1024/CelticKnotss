using System.Diagnostics;
namespace CelticKnots;

public class WorkingTile {

    TileGeneratior Parent { get; set; } = new TileGeneratior();
    //A holder for a stock tile, with a Canvas position.
    //This Object can draw itself, or Get the Text for it's SVG.

    //This is the tile on the Canvas - the one I draw - the drawing details are in the StockTile.
    public StockTile ThisTile;
    //X an Y cannot be changed - they are set on creation.
    //The tile can change - if it is rotated or the weave changes - it can be completely replaced.
    public int X { get; set; }
    public int Y { get; set; }
    //I set these in init
    public float XOffset { get; set; }
    public float YOffset { get; set; }



    //Just get them when I need to. These are at 0,0

    //I may just add a routine to clone the points.
    private List<ListOfPointFForDrawing> _LastFullOutLinePoints = [];
    //These are in the tile position.
    private List<ListOfPointFForDrawing> _LastFullOutLinePointsInPosition = [];

    public List<ListOfPointFForDrawing> ClonePoints() {
        List<ListOfPointFForDrawing> ReturList = [];
        foreach (ListOfPointFForDrawing pointFList in _LastFullOutLinePoints) {
            ReturList.Add(pointFList.Clone());
        }
        return ReturList;
    }
    public WorkingTile(TileGeneratior parent, StockTile thisTile, int x, int y) {
        Parent = parent;
        ThisTile = thisTile;
        X = x;
        Y = y;
        double DiagonalTileWidth = LinePointHelper.DistanceBetweenPoints(new PointF(0, 0), new PointF((float)TileProperties.Width, (float)TileProperties.Height));
        XOffset = (float)DiagonalTileWidth * X;
        YOffset = (float)DiagonalTileWidth * Y / 2;
        if (Y % 2 == 0) {
            XOffset -= (float)(DiagonalTileWidth / 2);
        }
    }

    public List<ListOfPointFForDrawing> LastFullOutLinePoints {
        //A capped full outline set of points in Tile Location 0,0.
        get
        {
            if (_LastFullOutLinePoints.Count == 0) { _LastFullOutLinePoints = GetTileLinePoints(E_OutlineType.FullOutline, true); }
            return _LastFullOutLinePoints;
        }
    }
    public List<ListOfPointFForDrawing> LastFullOutLinePointsInPosition {
        get
        {
            if (_LastFullOutLinePointsInPosition.Count == 0) { _LastFullOutLinePointsInPosition = AddTilePositionToPoints(LastFullOutLinePoints); }
            return _LastFullOutLinePointsInPosition;
        }
    }
    public string ToSVGPathString(E_OutlineType requestedPoints, bool capped, bool debugoutput) {
        string data = string.Empty;

        //List<ListOfPointFForDrawing> pointFLists = GetTileLinePoints(requestedPoints, capped);
        ////Finalise the position before I make the SVG, shifting the points by the Tile X and Y.
        //pointFLists = AddTilePositionToPoints(pointFLists);
        List<ListOfPointFForDrawing> pointFLists = LastFullOutLinePointsInPosition;
        data += SVGHelper.SVGText(new PointF(XOffset, YOffset), ThisTile.ToString());
        if (debugoutput) {
            //Draw the outline of the Tile in Blue.
            data += SVGHelper.GetSVGTileDebugOutline(this);
        }
        foreach (ListOfPointFForDrawing pointFList in pointFLists) {
            if (pointFList.IsCurved) //there is a problem here, I can have a 4 point line :(
            {
                data += SVGHelper.ConvertCurve(pointFList);
                if (debugoutput) {
                    //The Line is Red 
                    //Put a vertical and horizontal Line on each point Green.
                    data += SVGHelper.GetSVGTileDebugPointline(pointFList);//this,
                }
            }
            else if (pointFList.Count == 2) {
                data += SVGHelper.ConvertTwoPointLine(pointFList);
                if (debugoutput) {

                }
            }
            else {
                Debug.Assert(false);
            }
        }
        return data;
    }
    public void DrawLeftAndRight(Graphics g) {
        //Testing
        //float XOffset = (float)TileProperties.Width * X;
        //float YOffset = (float)TileProperties.Height * Y;
        //g.DrawRectangle(new Pen(Color.Blue), XOffset, YOffset, (int)TileProperties.Width, (int)TileProperties.Height);
        //List<ListOfPointFForDrawing> pointFLists = GetTileLinePoints(E_OutlineType.FullOutline, true);
        ///Finalise the drawing position.
        //pointFLists = AddTilePositionToPoints(pointFLists);
        List<ListOfPointFForDrawing> pointFLists = LastFullOutLinePointsInPosition;

        DrawText(g, new PointF(XOffset, YOffset), this.ThisTile.ToString());
        foreach (ListOfPointFForDrawing pointFList in pointFLists) {
            if (pointFList.Count > 0) {
                DrawFirstPointRect(g, TileProperties.FirstPointPen, pointFList);
                DrawLine(g, pointFList.Pen, pointFList);
            }
        }
    }
    public static void DrawFirstPointRect(Graphics g, Pen pen, ListOfPointFForDrawing pointFList) {
        RectangleF rect = new(pointFList[0].X - 1, pointFList[0].Y - 1, 3, 3);
        g.DrawRectangle(pen, rect);
    }
    public static void DrawLine(Graphics g, ListOfPointFForDrawing pointFList) {
        if (pointFList.Count > 1) //Some have no points.
        {

            if (pointFList.Count > 2)//Curves are only 4 point
            {
                g.DrawBezier(TileProperties.GlyphPen, pointFList[0], pointFList[1], pointFList[2], pointFList[3]);
            }
            else //Lines are only 2 point
            {
                g.DrawLine(TileProperties.GlyphPen, pointFList[0], pointFList[1]);
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
    //a function to dtaw the text on the tile called DrawText     //a function to dtaw the text on the tile called DrawText  - it will be called in the DrawLeftAndRight
    public void DrawText(Graphics g, PointF position, string text) {
        //, Pen pen
        g.DrawString(text, new System.Drawing.Font("Arial", 4), Brushes.Black, position);
    }


    public List<ListOfPointFForDrawing> AddTilePositionToPoints(List<ListOfPointFForDrawing> listpfpoints) {
        //This adds the X and Y offset to the points for display or output to SVG.

        //The rules for positioning the times is
        //they are rotated 45 degrees when created

        //made them properties
        //double DiagonalTileWidth = LinePointHelper.DistanceBetweenPoints(new PointF(0, 0), new PointF((float)TileProperties.Width, (float)TileProperties.Height));
        //float XOffset = (float)DiagonalTileWidth * X;//= 0;
        //float YOffset = (float)DiagonalTileWidth * Y / 2;//= 0;
        ////if (Y%2 == 0) {

        //if (Y % 2 == 0) {
        //    XOffset -= (float)(DiagonalTileWidth / 2);
        //}

        //YOffset = (float)(TileProperties.Height * Y / 2) -(float)(DiagonalTileWidth/2);
        //}
        //else {
        //    XOffset = (float)TileProperties.Width * X ;
        //    YOffset = (float)TileProperties.Height * Y / 2;
        //}


        List<ListOfPointFForDrawing> ReturnList = [];
        foreach (ListOfPointFForDrawing pointFList in listpfpoints) {
            ListOfPointFForDrawing pointFs = new(pointFList.IsCurved, pointFList.OutlineType, pointFList.Pen);
            for (int i = 0; i < pointFList.Count; i++) {
                pointFs.Add(new PointF(pointFList[i].X + XOffset, pointFList[i].Y + YOffset));
            }
            ReturnList.Add(pointFs);
        }
        return ReturnList;
    }
    //This gets ListOfPointFForDrawing it is a list of PointF with a property to let me know if the points are a curve.
    // I (now) use PointF for drawing or SVG output.
    private List<ListOfPointFForDrawing> GetTileLinePoints(E_OutlineType requestedPoints, bool capped) {
        //This now gets the line points but no loger adds the Tile offset position - this means I can move or rotate the points easily before drawing.
        List<ListOfPointFForDrawing> ReturnPoints = [];
        PointF LeftStorePoint = new();
        PointF RightStorePoint = new();

        if (ThisTile is { }) {
            if (ThisTile.MyGridLines.Count > 0) {
                //I need to add the redirect rules when I generate the stock tiles
                if (this.ThisTile.Redirect > -1) {
                    //Debug.Assert(false);
                    //I want he Points from a different tile - I'll get them from the parent.   
                    StockTile st = Parent.StockTilesList[this.ThisTile.Redirect - 1]; //The Tile Nimbers atart at 1 but the collection 0.
                    WorkingTile TileCopy = new WorkingTile(Parent, st, X, Y);
                    //I may need to clone the list of points.
                    return TileCopy.LastFullOutLinePoints;
                }

                GridLine thisline;
                //float XOffset = (float)TileProperties.Width * X;
                //float YOffset = (float)TileProperties.Height * Y;
                //g.DrawRectangle(new Pen(Color.Blue), XOffset, YOffset, (int)TileProperties.Width, (int)TileProperties.Height);

                int LastGroupNumber = -1;
                int TotalLinesInGroup = 0;
                int LineCounterForGroup = 0;
                for (int i = 0; i < ThisTile.MyGridLines.Count; i++) {
                    thisline = ThisTile.MyGridLines[i];
                    bool FlippDestinationPoints = false;
                    bool FlippStartPoints = false;
                    bool LastDestinationFlip = false;
                    int MidPointrotation = 0;
                    //Flip the Corner starts.
                    if (thisline.MyLinePoints[0].LinePointProperties.IsCorner) {
                        FlippStartPoints = !FlippStartPoints;
                    }

                    //The LineGroup should be a class - I mat create one later but for now I'll get what I need
                    if (thisline.LineGroupNumber == LastGroupNumber) {//use the flip of the last end flip for the start of a joined line
                        FlippStartPoints = LastDestinationFlip;
                        LineCounterForGroup += 1;//first line is 0
                    }
                    else {
                        LastGroupNumber = thisline.LineGroupNumber;
                        TotalLinesInGroup = ThisTile.MyGridLines.CountGroup(LastGroupNumber);
                        LineCounterForGroup = 1;
                    }
                    //I'll test it and optimise it later
                    if (TotalLinesInGroup == 1) {
                        FlippDestinationPoints = !FlippStartPoints;
                    }
                    else if (TotalLinesInGroup == 2) {
                        if (LineCounterForGroup == 1) {
                            FlippDestinationPoints = !FlippStartPoints;
                        }
                        else {
                            //Line two of the diagonal cross needs the start needs flipping again
                            if (ThisTile.TType == E_TileLineType.DiagonalCross) {
                                FlippStartPoints = !FlippStartPoints;
                            }
                            else {
                                FlippDestinationPoints = !FlippStartPoints;
                            }
                        }
                    }
                    else if (TotalLinesInGroup == 3) {
                        if (LineCounterForGroup == 1) {
                            FlippDestinationPoints = FlippStartPoints;
                        }
                        else if (LineCounterForGroup == 2) {
                            if (ThisTile.TType == E_TileLineType.Corners) {
                                FlippStartPoints = !FlippStartPoints;
                            }
                            FlippDestinationPoints = !FlippStartPoints;
                        }
                        else {
                            //MidPoints third line starts wrong
                            if (ThisTile.TType == E_TileLineType.MidPoints || ThisTile.TType == E_TileLineType.HalfMid) {
                                FlippStartPoints = !FlippStartPoints;
                            }
                            FlippDestinationPoints = FlippStartPoints;
                        }
                    }
                    else {
                        Debug.Assert(false); //I should raise and error.
                    }
                    //if the destination is MiddleMiddle I need the rotation of the next start line.
                    if (LineCounterForGroup < TotalLinesInGroup) {
                        if (thisline.MyLinePoints[^1].ThisLinePoint == E_LinePoint.MiddleMiddle) {
                            GPointOffsets OffsetStartPoints = new(ThisTile.MyGridLines[i + 1].MyLinePoints[^1].ThisLinePoint);
                            MidPointrotation = OffsetStartPoints.Rotation;
                        }
                    }

                    //Just try it with a centreline (no offset)
                    //I will need to draw Left then right then bridge any with Weave.
                    //No cap needed for Centrelines
                    ListOfPointFForDrawing pointFList = thisline.GetOutlinePointFList(FlippStartPoints, FlippDestinationPoints, E_OffsetPointHorizontal.Normal, MidPointrotation);

                    //var p = new Pen(Color.Red);
                    //DrawFirstPointRect(g, p, pointFList);
                    //DrawLine(g, p, pointFList);

                    ListOfPointFForDrawing pointFListLeft = thisline.GetOutlinePointFList(FlippStartPoints, FlippDestinationPoints, E_OffsetPointHorizontal.Left, MidPointrotation);

                    //p = new Pen(Color.Green);
                    //DrawFirstPointRect(g, p, pointFListLeft);
                    //DrawLine(g, p, pointFListLeft);

                    ListOfPointFForDrawing pointFListRight = thisline.GetOutlinePointFList(FlippStartPoints, FlippDestinationPoints, E_OffsetPointHorizontal.Right, MidPointrotation);


                    if (this.ThisTile.TType == E_TileLineType.MidToCorner && LineCounterForGroup == 1) {

                        LeftStorePoint = pointFListLeft[^1];
                        RightStorePoint = pointFListRight[^1];
                    }

                    if (this.ThisTile.TType == E_TileLineType.MidToCorner && LineCounterForGroup == 2) {

                        pointFListLeft[0] = LeftStorePoint;
                        pointFListRight[0] = RightStorePoint;
                    }
                    //p = new Pen(Color.Blue);
                    //DrawFirstPointRect(g, p, pointFListRight);
                    //DrawLine(g, p, pointFListRight);

                    //Check the first Line in the group for a cap, left and right will both have the same value.
                    ListOfPointFForDrawing pointFListCap = new(false, E_OutlineType.Cap, TileProperties.GlyphPen);

                    //Check the line for a cap on both sides.
                    if (thisline.MyLinePoints[0].LinePointProperties.Weave == E_TileWeave.UnderWeave) {
                        pointFListCap.Add(pointFListLeft[0]);
                        pointFListCap.Add(pointFListRight[0]);
                    }
                    if (thisline.MyLinePoints[^1].LinePointProperties.Weave == E_TileWeave.UnderWeave) {
                        pointFListCap.Add(pointFListLeft[^1]);
                        pointFListCap.Add(pointFListRight[^1]);
                    }
                    //if (pointFListCap.Count > 0)
                    //{
                    //    DrawLine(g, p, pointFListCap);
                    //}

                    //This didn't work for the middle points :( 
                    //LastDestinationFlip = FlippDestinationPoints;


                    //Add an outline for debugging.
                    ListOfPointFForDrawing Outline = new(false, E_OutlineType.Outline, TileProperties.OutlinePen);
                    Outline.Add(new PointF(0, 0));
                    Outline.Add(new PointF((float)TileProperties.Width, 0));
                    if (CanvasProperties.RotateOutput) {
                        Outline.RotateAllPoints45RoundTileCenter();
                    }
                    ReturnPoints.Add(Outline);

                    Outline = new(false, E_OutlineType.Outline, TileProperties.OutlinePen);
                    Outline.Add(new PointF((float)TileProperties.Width, 0));
                    Outline.Add(new((float)TileProperties.Width, (float)TileProperties.Height));
                    if (CanvasProperties.RotateOutput) {
                        Outline.RotateAllPoints45RoundTileCenter();
                    }

                    ReturnPoints.Add(Outline);

                    Outline = new(false, E_OutlineType.Outline, TileProperties.OutlinePen);
                    Outline.Add(new((float)TileProperties.Width, (float)TileProperties.Height));
                    Outline.Add(new((float)0, (float)TileProperties.Height));
                    if (CanvasProperties.RotateOutput) {
                        Outline.RotateAllPoints45RoundTileCenter();
                    }
                    ReturnPoints.Add(Outline);

                    Outline = new(false, E_OutlineType.Outline, TileProperties.OutlinePen);
                    Outline.Add(new((float)0, (float)TileProperties.Height));
                    Outline.Add(new(0, 0));
                    if (CanvasProperties.RotateOutput) {
                        Outline.RotateAllPoints45RoundTileCenter();
                    }
                    ReturnPoints.Add(Outline);

                    //Add the list of each set of lines.
                    if (requestedPoints == E_OutlineType.Left) {
                        ReturnPoints.Add(pointFListLeft);//.RotateToDisplay(XOffset, YOffset)
                    }
                    else if (requestedPoints == E_OutlineType.Centre) {
                        ReturnPoints.Add(pointFList);//.RotateToDisplay(XOffset, YOffset)
                    }
                    else if (requestedPoints == E_OutlineType.Right) {
                        ReturnPoints.Add(pointFListRight);//.RotateToDisplay(XOffset, YOffset)
                    }
                    else if (requestedPoints == E_OutlineType.FullOutline) {
                        ReturnPoints.Add(pointFListLeft);//.RotateToDisplay(XOffset, YOffset)y
                        ReturnPoints.Add(pointFListRight);//.RotateToDisplay(XOffset, YOffset)
                    }
                    if (pointFListCap.Count > 0) {
                        if (capped) {
                            ReturnPoints.Add(pointFListCap);//.RotateToDisplay(XOffset, YOffset)
                        }
                    }
                }
            }
        }
        return ReturnPoints;
    }
}
