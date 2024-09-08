using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelticKnots;
public class GridPoint(double x, double y) {
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public GridPoint Merge(GridPoint withPoint) {
        double MidX = (X + withPoint.X) / 2;
        double MidY = (Y + withPoint.Y) / 2;
        return new GridPoint(MidX, MidY);

    }
    public GridPoint Clone() { return new GridPoint(X, Y); }
    //This is only used in calculations.
    public PointF ToPointF() { return new PointF((float)X, (float)Y); }

    public void Flip(bool Horizontal) {
        if (Horizontal) {
            X = (Math.Abs(X - TileProperties.Width));
        }
        else {
            Y = (Math.Abs(Y - TileProperties.Height));
        }
    }

    #region"PointRotating"
    public void RotatePointRoundZeroZero(double angleInDegrees) {
        GridPoint centerPoint = new(0, 0);
        RotatePoint(centerPoint, angleInDegrees);
    }
    public void RotatePoint(double angleInDegrees) {
        GridPoint centerPoint = new(TileProperties.Width / 2, TileProperties.Height / 2);
        RotatePoint(centerPoint, angleInDegrees);
    }
    public void RotatePoint(GridPoint centerPoint, double angleInDegrees) {
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
public class ListOfPointFForDrawing(bool iscurved, E_OutlineType outlinetype, Pen pen) : List<PointF> {
    public bool IsCurved { get; set; } = iscurved;
    public E_OutlineType OutlineType { get; set; } = outlinetype;
    //This is only here because I cannot inherit PointF.
    public Pen Pen { get; set; } = pen;
    //public ListOfPointFForDrawing RotateToDisplay() {//float xoffset, float yoffset
    //    //PointF CentrePoint = new(xoffset + ((float)TileProperties.Width / 2), yoffset + ((float)TileProperties.Height / 2));
    //    PointF CentrePoint = new(((float)TileProperties.Width / 2), ((float)TileProperties.Height / 2));

    //    ListOfPointFForDrawing ReturnPoints = new(this.IsCurved, this.OutlineType, Pen);
    //    for (int i = 0; i < this.Count; i++) {
    //        //Get the Point rotated 45 degrees to display or output to SVG.
    //        PointF NewPoint = RotatePoint(CentrePoint, this[i], 45f);
    //        ReturnPoints.Add(NewPoint);
    //    }
    //    return ReturnPoints;
    //}
    public ListOfPointFForDrawing Clone() {
        ListOfPointFForDrawing ReturnPoints = new(this.IsCurved, this.OutlineType, Pen);
        for (int i = 0; i < this.Count; i++) {
            PointF p = new PointF(this[i].X, this[i].Y);
            ReturnPoints.Add(p);
        }
        //Copy the proerties -- I may overwrite them.
        ReturnPoints.IsCurved = this.IsCurved;
        ReturnPoints.OutlineType = this.OutlineType;
        ReturnPoints.Pen = this.Pen;
        return ReturnPoints;
    }

    public void RotateAllPoints45RoundTileCenter() {
        PointF CentrePoint = new(((float)TileProperties.Width / 2), ((float)TileProperties.Height / 2));
        for (int p = 0; p < this.Count; p++) {
            this[p] = RotatePoint(CentrePoint, this[p], 45);
        }
    }
    //this needs to move to a helper class.
    // or I can add some more Rotations, like int Index to rotate the point at index. 
    public static PointF RotatePoint(PointF centerPoint, PointF pointToRotate, double angleInDegrees) {
        double angleInRadians = angleInDegrees * (Math.PI / 180);
        double cosTheta = Math.Cos(angleInRadians);
        double sinTheta = Math.Sin(angleInRadians);

        //return new GridPoint(
        PointF ReturnPoint = new() {
            X = (float)(cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
            Y = (float)(sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
        };
        return ReturnPoint;
    }
    //I need to move this.


}