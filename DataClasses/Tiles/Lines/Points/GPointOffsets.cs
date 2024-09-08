using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelticKnots;
public class GPointOffsets : List<GridPoint> {
    //These are the points to offset for Left and right outlines, it didn't quite work as I imained, it was close but took some manipulation to make to work.
    //25 points above to the left, right and below the normal draw point, they are flipped and rotated so the same offset number is the same for all points (this didn't work for the middle points)

    //write a function to add forty yo the input number
    public E_LinePoint LinePoint { get; set; } = E_LinePoint.None;
    public int Rotation { get; set; } = 0;
    //These are the offsets used when drawing the "outline" of a path.
    public GPointOffsets(E_LinePoint linepoint) {
        LinePoint = linepoint;
        double OffSetGap = (TileProperties.Linewidth - 1);
        double HalfLinewidth = (OffSetGap / 2);
        //Changed my mind 12 is the middle -- or 12 + 25 for the diagonal cross.
        //GridPoint p = new GridPoint(0, 0);
        double YPos = -(OffSetGap + HalfLinewidth);
        double XPos = -(OffSetGap + HalfLinewidth);
        //Make the Offset points
        for (int y = 0; y < 5; y++) {
            YPos += HalfLinewidth;
            for (int x = 0; x < 5; x++) {
                XPos += HalfLinewidth;
                Add(new GridPoint(XPos, YPos));
            }
            //back to the start.
            XPos = -(OffSetGap + HalfLinewidth);
        }

        int intRotate = 0;
        switch (linepoint) {
            case E_LinePoint.None:
            case E_LinePoint.TopMiddle:
            case E_LinePoint.TopMiddleMiddleSplit:
            case E_LinePoint.MiddleMiddle: {
                    intRotate = 0;
                    break;
                }
            case E_LinePoint.TopRight:
            case E_LinePoint.TopRightMiddleSplit: {
                    intRotate = 45;
                    break;
                }
            case E_LinePoint.MiddleRightsplit:
            case E_LinePoint.MiddleRight: {
                    intRotate = 90;
                    break;
                }
            case E_LinePoint.BottomRightMiddleSplit:
            case E_LinePoint.BottomRight: {
                    intRotate = 135;
                    break;
                }
            case E_LinePoint.BottomMiddleMiddleSplit:
            case E_LinePoint.BottomMiddle: {
                    intRotate = 180;
                    break;
                }
            case E_LinePoint.BottomLeftMiddleSplit:
            case E_LinePoint.BottomLeft: {
                    intRotate = 225;
                    break;
                }
            case E_LinePoint.MiddleLeft:
            case E_LinePoint.MiddleLeftsplit: {
                    intRotate = 270;
                    break;
                }
            case E_LinePoint.TopLeft:
            case E_LinePoint.TopLeftMiddleSplit: {
                    intRotate = 315;
                    break;
                }
        }
        Rotation = intRotate;

        PlaceTheOffsetPoints(linepoint);
    }
    // a funtion to return 6
    public void DebugPoints() {
        string strOutput = string.Empty;
        for (int i = 0; i < this.Count; i++) {
            GridPoint p = this[i];
            strOutput += $"(X{p.X,4} Y{p.Y,4})";
            if ((i + 1) % 5 == 0) {
                strOutput += Environment.NewLine;
            }
            else {
                strOutput += ",";
            }
        }
        Debug.Print(strOutput);
    }
    public void RotateAllPoints(double angletoRotate, bool roundCentre) {
        for (int i = 0; i < this.Count; i++) {
            GridPoint p = this[i];
            if (roundCentre) {
                p.RotatePoint(angletoRotate);
            }
            else { p.RotatePointRoundZeroZero(angletoRotate); }
        }
    }
    public void RotateAllPoints(GridPoint centerPoint, double angleInDegrees) {
        if (angleInDegrees == 0) {
            return;
        }

        for (int i = 0; i < this.Count; i++) {
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
    public void FlipHorizontally() {
        List<GridPoint> g = [];
        for (int Row = 1; Row < 6; Row++) {
            for (int Col = 5; Col > 0; Col--) {
                int i = ((Row - 1) * 5) + Col - 1;
                g.Add(this[i]);
            }
        }
        //Copy the new list back
        this.Clear();
        this.AddRange(g);
    }
    public GPointOffsets Clone() {
        GPointOffsets returnList = new(this.LinePoint);
        returnList.Rotation = this.Rotation;
        return returnList;
    }
    private void PlaceTheOffsetPoints(E_LinePoint linepoints) {
        GridPoint ThisPoint = LinePointHelper.GetGpoint(linepoints);
        for (int i = 0; i < this.Count; i++) {
            GridPoint p = this[i];
            p.X += ThisPoint.X;
            p.Y += ThisPoint.Y;
        }
    }
}