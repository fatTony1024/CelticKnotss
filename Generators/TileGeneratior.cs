namespace CelticKnots;

public class TileGeneratior {
    public ListOfStockTiles StockTilesList { get; set; }
    public TileGeneratior() {
        StockTilesList = GenerateAllTiles();
    }

    public static ListOfStockTiles GenerateAllTiles() {
        //I am going to Generate All the tiles all the rotations, Square and curved with all the flips.
        //Even though there are a lot I don't care about. and things like a curved cross will not render properly anyway.
        //I will have to find a way to redirect the ones I don't want to draw.

        //Effectively just ignore the tiles I don't want to draw by pointint to a tile the same shape.
        //Capping could trip me up.
        ListOfStockTiles TileBuilder = [];
        foreach (E_TileLineType ttype in Enum.GetValues(typeof(E_TileLineType))) {
            foreach (bool curved in new bool[] { false, true }) {
                foreach (bool flipped in new bool[] { false, true }) {
                    foreach (E_TileRotation tilerotation in Enum.GetValues(typeof(E_TileRotation))) {

                        E_TileRotation NewRotation = tilerotation;
                        if (ttype == E_TileLineType.MidPoints || ttype == E_TileLineType.Corners) {
                            NewRotation = tilerotation switch {
                                E_TileRotation.RotatedOneEightyDegrees => E_TileRotation.NoRitation,
                                E_TileRotation.RotatedTwoSeventyDegrees => E_TileRotation.RotatedNinetyDegrees,
                                _ => tilerotation
                            };
                        }
                        bool CanbeFlipped = false;
                        if (ttype == E_TileLineType.MidToCorner) {
                            CanbeFlipped = true;
                        }
                        bool CanBecurved = ttype switch {
                            E_TileLineType.Cross => false,
                            E_TileLineType.DiagonalCross => false,
                            E_TileLineType.Bar => false,
                            E_TileLineType.DiagonalBar => false,
                            _ => true
                        };
                        //Make the tile anyway
                        StockTile st = new(ttype, tilerotation, curved, flipped);
                        //CanBecurved is the value we want for curved, but only redirect if it is a different tile.
                        if (flipped && !CanbeFlipped) {
                            if (curved && !CanBecurved) {
                                st.Redirect = TileBuilder.GetIndexByProperties(ttype, NewRotation, false, false);
                            }
                            else { st.Redirect = TileBuilder.GetIndexByProperties(ttype, NewRotation, curved, false); }
                        }
                        else if (curved && !CanBecurved) {
                            st.Redirect = TileBuilder.GetIndexByProperties(ttype, NewRotation, false, flipped);
                        }
                        else if (tilerotation != NewRotation) {
                            st.Redirect = TileBuilder.GetIndexByProperties(ttype, NewRotation, curved, flipped);
                        }
                        TileBuilder.Add(st);
                    }
                }
            }
        }
        return TileBuilder;

    }
    public static List<StockTile> GenerateAllTilesX() {
        //List<StockTile>
        ListOfStockTiles TileBuilder =
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
            new StockTile(E_TileLineType.Bar, E_TileRotation.RotatedNinetyDegrees, false, false),
            //DiagonalBar   2       1
            new StockTile(E_TileLineType.DiagonalBar, E_TileRotation.NoRitation, false, false),
            new StockTile(E_TileLineType.DiagonalBar, E_TileRotation.RotatedNinetyDegrees, false, false),
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
