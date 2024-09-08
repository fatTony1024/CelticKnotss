using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelticKnots;

public class ExtendedLinePoints : List<ExtendedLinePoint> {
    public ExtendedLinePoint AddPoint(E_LinePoint thisPoint) {
        ExtendedLinePoint p = new(thisPoint);
        Add(p);
        return p;
    }
    public int AddPoints(params E_LinePoint[] points) {
        foreach (E_LinePoint p in points) {
            AddPoint(p);
        }
        return points.Length;
    }
}

public class ExtendedLinePoint(E_LinePoint thisPoint) {
    //I thought it was a good Idea to use enums, but then I needed more data on the points so had to make a class to handle them - this was a bad idea.
    //I would have been better using something like the Tile Generator and kept all the data together. 
    public E_LinePoint ThisLinePoint { get; set; } = thisPoint;
    public LinePointInformationObject LinePointProperties { get; set; } = new LinePointInformationObject(thisPoint);
}
