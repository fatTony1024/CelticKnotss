namespace CelticKnots;

public class LinePointInformationObject {
    //Extended information for the E_LinePoint ThisLine;
    public E_LinePoint ThisLine { get; set; }
    public string Description { get; private set; } = "";
    public string Name { get; private set; } = "";
    public int ThisNumber { get; private set; } = 0;
    public bool CentredXorY { get; private set; } = false;
    public bool CentredX { get; private set; } = false;
    public bool CentredY { get; private set; } = false;
    public bool IsCorner { get; private set; } = false;
    public bool IsLeftRow { get; private set; } = false;
    public bool IsTopRow { get; private set; } = false;
    public bool IsRightRow { get; private set; } = false;
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
    public LinePointInformationObject(E_LinePoint linepoints) {
        ThisLine = linepoints;
        ThisPoint = LinePointHelper.GetGpoint(ThisLine);
        ProcessPoint();
    }
    private void ProcessPoint() {
        Description = EnumerationHelper.GetDescription(ThisLine);
        Name = EnumerationHelper.GetEnumName(ThisLine);
        ThisNumber = (int)ThisLine;
        if (ThisNumber != 0)// 0 is a non-point.
        {
            CentredX = ThisNumber is > 6 and < 12; //7,8,9,10,11
            switch (ThisLine) {
                //CentredY - Vertical centre 2,5,9,13,16
                case E_LinePoint.TopMiddle:
                case E_LinePoint.TopMiddleMiddleSplit:
                case E_LinePoint.MiddleMiddle:
                case E_LinePoint.BottomMiddleMiddleSplit:
                case E_LinePoint.BottomMiddle:
                    CentredY = true;
                    break;
            };
            if (CentredX || CentredY) {
                CentredXorY = true; // in the middle. ie. not a corner.
            }
            if (CentredX && CentredY) {
                IsMiddlePoint = true;
            }
            switch (ThisLine) {
                case E_LinePoint.TopLeft:
                case E_LinePoint.TopLeftMiddleSplit:
                case E_LinePoint.MiddleLeft:
                case E_LinePoint.MiddleLeftsplit:
                case E_LinePoint.BottomLeftMiddleSplit:
                case E_LinePoint.BottomLeft:
                    IsLeftOfCentre = true;
                    break;
            };
            switch (ThisLine) {
                case E_LinePoint.TopRight:
                case E_LinePoint.TopRightMiddleSplit:
                case E_LinePoint.MiddleRight:
                case E_LinePoint.MiddleRightsplit:
                case E_LinePoint.BottomRightMiddleSplit:
                case E_LinePoint.BottomRight:
                    IsRightOfCentre = true;
                    break;
            };
            IsTopOfCentre = ThisNumber < 7;
            IsBottomOfCentre = ThisNumber > 11;
            switch (ThisLine) {
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
            switch (ThisLine) {
                case E_LinePoint.TopLeft:
                case E_LinePoint.MiddleLeft:
                case E_LinePoint.BottomLeft:
                    IsLeftRow = true;
                    break;
            };
            IsTopRow = ThisNumber < 4;
            switch (ThisLine) {
                case E_LinePoint.TopRight:
                case E_LinePoint.MiddleRight:
                case E_LinePoint.BottomRight:
                    IsRightRow = true;
                    break;
            };
            IsBottomRow = ThisNumber > 14; //BottomRightMiddleSplit
            if (IsOutsideEdge) {
                IsCorner = !CentredXorY;
            }
            HasValue = ThisNumber > 0; //0 is just a holder for not defined.
        }
    }
}
