using System.Diagnostics;
namespace CelticKnots;


public class ListOfStockTiles : List<StockTile> {
    //public StockTile AddStickTile(StockTile stockTile) {
    //    Add(stockTile);
    //    return stockTile;
    //}
    //public StockTile GetByIndex(int index) {
    //    return this[index];
    //}
    public int GetIndexByProperties(E_TileLineType ttype, E_TileRotation tilerotation, bool curved, bool flipped) {
        int TileCount = 4 * 2 * 2;
        //This is calculated the same way as the TileIndex in the StockTile class.  
        int TileIndex = (((int)ttype) * TileCount) + (((int)tilerotation / 90)) + ((curved ? 1 : 0) * 8) + ((flipped ? 1 : 0) * 4) + 1;
        if (this.Count > TileIndex) {
            //Debug.Print($"TileIndex {TileIndex} Count {this.Count}"
            return TileIndex;
        }
        else {
            return -1;
        }
    }
    public StockTile GetTileByProperties(E_TileLineType ttype, E_TileRotation tilerotation, bool curved, bool flipped) {
        //int TileCount = 4 * 2 * 2;
        //This is calculated the same way as the TileIndex in the StockTile class.  
        int TileIndex = GetIndexByProperties(ttype, tilerotation, curved, flipped);
        if (this.Count > TileIndex) {
            //Debug.Print($"TileIndex {TileIndex} Count {this.Count}"
            return this[TileIndex];
        }
        else {
            //Make the tile if we don't have one - -it will not have a TileIndex but I'll work that out if I need to.
            //I don't think I should ever get here.
            Debug.Assert(false);
            return new StockTile(ttype, tilerotation, curved, flipped);
        }
    }


}
public class StockTile {
    //This holds basic information on each tile, it knows the lines on the tile and their numbers, then their coodinates.
    public E_TileLineType TType { get; set; }
    //The IsCurved will be set on the line the points will be a count on th elines object, and the TWeave set for each point.
    public bool IsCurved = false;
    public E_TileRotation TRotation { get; set; }
    public bool Flipped { get; set; }
    public GridLineGroup MyGridLines { get; set; }   // These are the lines that I will finally draw - (NOTE: TC08/08/24 not the ones I will fianlly draw but the centrelines)
                                                     // The Weeave, Rotation and Flip determine the line that needs splitting for the
                                                     // Cross and Diagonal cross

    //This will replace MyGridLines
    public TileLineGroups MyTileGroups { get; set; } //This is the group of lines that will be drawn on the canvas. (The lines that will be drawn on the canvas)


    //I need to Get the points from a Valid line of the same shape. I wil make a new one the Caps may need to be altered.
    //I need a funtion to rebuild the points when the caps change -- I will trigger it so it doen't fire more than it needs to.
    public int Redirect { get; set; } = -1;
    // New property to keep track of the tile's index in the StockTilesList list
    public int TileIndex { get; init; }

    public override string ToString() {
        return $"Tile {TileIndex}){TType}\n\t{TRotation} {(IsCurved ? "Curved" : "Straight")}-{(Flipped ? "Flipped" : "Unflipped")}";
    }

    public bool CompareLines() {
        //Compare the lines in the group to the lines in MyGridLines having the same group number (I may need to change this as the group may not be the same number - although I think it is.)
        foreach (GridLine l in MyGridLines) {
            GridLine? gl = MyTileGroups.GetLine(l.LineGroupNumber, l.TType, l.MyLinePoints[0].ThisLinePoint,l.MyLinePoints[^1].ThisLinePoint);
            if (gl == null) {
                return false;
            }
        }
        //all found
        return true;
    }
    public StockTile(E_TileLineType ttype, E_TileRotation tilerotation, bool curved, bool flipped) {
        //Make them all once and reuse them, (it didn't work like that)

        //For the corners Set the Weave when the tile is placed for the first time -- it depends on the corners that are next to the tile --and the opposite end weave (maybe)
        //I could set the weave on the Cross and diagonal cross when the stock tile is created.
        //   I want the weave property on the Grid point for each end of the Line. //
        TType = ttype;
        TRotation = tilerotation;
        Flipped = flipped;
        //Make the MyGridLines 
        MyGridLines = [];
        MyTileGroups = [];
        GridLine line;
        IsCurved = curved;

        //int TileCount = Enum.GetValues(typeof(E_TileLineType)).Cast<int>().Max();
        //There are 4 rotations, Straight and curved, unflipped or flipped -- 16 tiles in a set.
        int TileCount = 4 * 2 * 2;
        TileIndex = (((int)ttype) * TileCount) + (((int)tilerotation / 90)) + ((curved ? 1 : 0) * 8) + ((flipped ? 1 : 0) * 4) + 1;

        if (TType == E_TileLineType.Cross) {
            //4 lines splits in the middles. (The two without a split could become one).
            //Each line has a start and an end point

            //I dont add any extra rotation to the cross and diagonal cross middle points as they can come from the rotation of the other end flipped

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


            //New -- IsCurved on the group defaults to False
            MyTileGroups.AddLinesToNewGroup( //Should be Group 0
                        new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleMiddle),
                        new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomMiddle, E_LinwWeave.UnderWeaveEnd)
                        );
            MyTileGroups.AddLinesToNewGroup(//Should be Group 1
                        new GridLine(TType, E_LinePoint.MiddleLeft, E_LinePoint.MiddleMiddle, E_LinwWeave.UnderWeaveStart),
                        new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.MiddleRight)
                        );
        }
        else if (TType == E_TileLineType.DiagonalCross) {
            //4 lines
            line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.MiddleMiddle, IsCurved, 0);
            //set the weave on one to under, let the other default to non orover.
            line.MyLinePoints[^1].LinePointProperties.Weave = E_TileWeave.UnderWeave;
            MyGridLines.AddLine(line);
            line = new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomRight, IsCurved, 0);
            line.MyLinePoints[0].LinePointProperties.Weave = E_TileWeave.UnderWeave;
            MyGridLines.AddLine(line);
            line = new GridLine(TType, E_LinePoint.TopRight, E_LinePoint.MiddleMiddle, IsCurved, 1);
            MyGridLines.AddLine(line);
            line = new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomLeft, IsCurved, 1);
            MyGridLines.AddLine(line);

            //New
            MyTileGroups.AddLinesToNewGroup(
                        new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.MiddleMiddle),
                        new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomRight, E_LinwWeave.UnderWeaveEnd)
                        );
            MyTileGroups.AddLinesToNewGroup(
                        new GridLine(TType, E_LinePoint.TopRight, E_LinePoint.MiddleMiddle, E_LinwWeave.UnderWeaveStart),
                        new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomLeft)
                        );
        }
        else if (TType == E_TileLineType.Bar) {
            //1 line
            line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.BottomMiddle, IsCurved, 0);
            MyGridLines.AddLine(line);

            //New
            MyTileGroups.AddLinesToNewGroup(
                        new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.BottomMiddle)
                        );

        }
        else if (TType == E_TileLineType.DiagonalBar) {
            //1 line
            line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.BottomRight, IsCurved, 0);
            MyGridLines.AddLine(line);
            //New
            MyTileGroups.AddLinesToNewGroup(
                        new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.BottomRight)
                        );
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

                //New
                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleRight, E_LinwWeave.Default, IsCurved)
                            );
                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.MiddleLeft, E_LinePoint.BottomMiddle, E_LinwWeave.Default, IsCurved)
                            );
            }
            else {
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

                //New
                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.TopMiddleMiddleSplit),
                            new GridLine(TType, E_LinePoint.TopMiddleMiddleSplit, E_LinePoint.MiddleRightsplit),
                            new GridLine(TType, E_LinePoint.MiddleRightsplit, E_LinePoint.MiddleRight)
                            );
                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.MiddleLeft, E_LinePoint.MiddleLeftsplit),
                            new GridLine(TType, E_LinePoint.MiddleLeftsplit, E_LinePoint.BottomMiddleMiddleSplit),
                            new GridLine(TType, E_LinePoint.BottomMiddleMiddleSplit, E_LinePoint.BottomMiddle)
                            );
            }
        }
        else if (TType == E_TileLineType.Corners)//curve
        {
            if (IsCurved) {
                //2 curved lines
                line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.BottomLeft, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopRight, E_LinePoint.BottomRight, IsCurved, 1);
                MyGridLines.AddLine(line);

                //New
                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.BottomLeft, E_LinwWeave.Default, IsCurved)
                            );
                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopRight, E_LinePoint.BottomRight, E_LinwWeave.Default, IsCurved)
                            );
            }
            else {
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

                //New
                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.TopLeftMiddleSplit),
                            new GridLine(TType, E_LinePoint.TopLeftMiddleSplit, E_LinePoint.BottomLeftMiddleSplit),
                            new GridLine(TType, E_LinePoint.BottomLeftMiddleSplit, E_LinePoint.BottomLeft)
                            );
                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopRight, E_LinePoint.TopRightMiddleSplit),
                            new GridLine(TType, E_LinePoint.TopRightMiddleSplit, E_LinePoint.BottomRightMiddleSplit),
                            new GridLine(TType, E_LinePoint.BottomRightMiddleSplit, E_LinePoint.BottomRight)
                            );
            }
        }
        else if (TType == E_TileLineType.HalfMid)//curve
        {
            if (IsCurved) {
                //1 Line
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleRight, IsCurved, 0);
                MyGridLines.AddLine(line);

                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleRight, E_LinwWeave.Default, IsCurved)
                            );

            }
            else {
                //3 lines
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.TopMiddleMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopMiddleMiddleSplit, E_LinePoint.MiddleRightsplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.MiddleRightsplit, E_LinePoint.MiddleRight, IsCurved, 0);
                MyGridLines.AddLine(line);

                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.TopMiddleMiddleSplit),
                            new GridLine(TType, E_LinePoint.TopMiddleMiddleSplit, E_LinePoint.MiddleRightsplit),
                            new GridLine(TType, E_LinePoint.MiddleRightsplit, E_LinePoint.MiddleRight)
                            );
            }
        }
        else if (TType == E_TileLineType.MidToCorner)//curve
        {
            if (IsCurved) {
                //1 Line
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.BottomRight, IsCurved, 0);
                MyGridLines.AddLine(line);

                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.BottomRight, E_LinwWeave.Default, IsCurved)
                            );
            }
            else {//2 lines
                line = new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleMiddle, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomRight, IsCurved, 0);
                MyGridLines.AddLine(line);

                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopMiddle, E_LinePoint.MiddleMiddle),
                            new GridLine(TType, E_LinePoint.MiddleMiddle, E_LinePoint.BottomRight)
                            );
            }
        }
        else if (TType == E_TileLineType.HalfCorners)//curve
        {
            if (IsCurved) {
                //1 curved line
                line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.BottomLeft, IsCurved, 0);
                MyGridLines.AddLine(line);

                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.BottomLeft, E_LinwWeave.Default, IsCurved)
                            );
            }
            else {
                //3 straight lines
                line = new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.TopLeftMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.TopLeftMiddleSplit, E_LinePoint.BottomLeftMiddleSplit, IsCurved, 0);
                MyGridLines.AddLine(line);
                line = new GridLine(TType, E_LinePoint.BottomLeftMiddleSplit, E_LinePoint.BottomLeft, IsCurved, 0);
                MyGridLines.AddLine(line);

                MyTileGroups.AddLinesToNewGroup(
                            new GridLine(TType, E_LinePoint.TopLeft, E_LinePoint.TopLeftMiddleSplit),
                            new GridLine(TType, E_LinePoint.TopLeftMiddleSplit, E_LinePoint.BottomLeftMiddleSplit),
                            new GridLine(TType, E_LinePoint.BottomLeftMiddleSplit, E_LinePoint.BottomLeft)
                            );
            }
        }
        else {
        }
        if ((tilerotation != E_TileRotation.NoRitation) || (flipped)) {
            for (int i = 0; i < MyGridLines.Count; i++) {
                //TC 24/08/24 don't for get the properties.
                GridLine l = MyGridLines[i];
                MyGridLines[i] = l.RotateAndFlipClone(tilerotation, flipped);
                MyGridLines[i].LineGroupNumber = l.LineGroupNumber;
                MyGridLines[i].IsCurved = l.IsCurved;
                MyGridLines[i].TType = l.TType;
                for (int p = 0; p < MyGridLines[i].MyLinePoints.Count; p++) {
                    MyGridLines[i].MyLinePoints[p].LinePointProperties.Weave = l.MyLinePoints[p].LinePointProperties.Weave;
                }
            }

            for (int g = 0; g < MyTileGroups.Count; g++) {
                GridLineGroup glg = MyTileGroups[g];
                for (int lg = 0; lg < glg.Count; lg++) {
                    GridLine l = glg[lg];
                    glg[lg] = l.RotateAndFlipClone(tilerotation, flipped);
                    glg[lg].TType = l.TType; //TC 08/08/24 I will keep onlt the TType, I don't want to keep the LineGroupNumber or the IsCurved on the group.
                    for (int p = 0; p < glg[lg].MyLinePoints.Count; p++) {
                        glg[lg].MyLinePoints[p].LinePointProperties.Weave = l.MyLinePoints[p].LinePointProperties.Weave;
                    }
                }
            }

        }
    }
    //There will be 9ish baseTiles - some have rotations 
}
// I have 2 line coordinate systems - 1 - 17 (there were 9) for the points and X,Y for the (same) draw points. These are added to the Canvas position and multiplied by the zoom.


