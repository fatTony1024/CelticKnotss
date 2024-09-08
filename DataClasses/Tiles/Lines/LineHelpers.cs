using System.Text.Json;
namespace CelticKnots;

public static class LinePointHelper {
    public static GridPoint GetGpoint(E_LinePoint thisline) {
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
    public static double DistanceBetweenPoints(PointF p1, PointF p2) {
        double dx = p2.X - p1.X;
        double dy = p2.Y - p1.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
    public static double DistanceBetweenPoints(GridPoint p1, GridPoint p2) {
        return DistanceBetweenPoints(p1.ToPointF(), p2.ToPointF());
    }
    public static bool ObjectsAreEqual<T>(T obj1, T obj2) {
        var obj1Serialized = GetJSON(obj1);
        var obj2Serialized = GetJSON(obj2);
        return obj1Serialized == obj2Serialized;
    }
    private static readonly JsonSerializerOptions JSON_WwriteOptions = new() {
        WriteIndented = true
    };
    //private static readonly JsonSerializerOptions JSON_ReadOptions = new()
    //{
    //    AllowTrailingCommas = true
    //};
    public static string GetJSON<T>(T obj1) {
        return JsonSerializer.Serialize(obj1, JSON_WwriteOptions);
    }

    //public static PointF? GetIntersection(Line1 Line2) //worry about it when I need it.
    public static PointF? GetIntersection(PointF p1, PointF p2, PointF p3, PointF p4) {
        // Calculate the differences
        float dx1 = p2.X - p1.X;
        float dy1 = p2.Y - p1.Y;
        float dx2 = p4.X - p3.X;
        float dy2 = p4.Y - p3.Y;

        // Calculate the determinants
        float determinant = dx1 * dy2 - dy1 * dx2;

        // If the determinant is zero, the lines are parallel
        if (Math.Abs(determinant) < 1e-10) {
            return null; // No intersection
        }

        // Calculate the intersection point
        float t = ((p3.X - p1.X) * dy2 - (p3.Y - p1.Y) * dx2) / determinant;
        float u = ((p3.X - p1.X) * dy1 - (p3.Y - p1.Y) * dx1) / determinant;

        // Check if the intersection point is within the line segments
        if (t >= 0 && t <= 1 && u >= 0 && u <= 1) {
            float intersectionX = p1.X + t * dx1;
            float intersectionY = p1.Y + t * dy1;
            return new PointF(intersectionX, intersectionY);
        }

        return null; // No intersection within the line segments
    }

}
