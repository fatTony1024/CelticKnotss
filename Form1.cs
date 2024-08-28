using System.Diagnostics;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace CelticKnots;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void PictureBox1_Paint(object sender, PaintEventArgs e)
    {

        //  The curves have 4 points, the Lines 2 -- I'll work on this/
        //50,0
        //50,50
        //100,100
        //100,100

        //Point p1 = new Point(50, 0);   // Start point
        //Point c1 = new Point(50, 50);   // First control point
        //Point c2 = new Point(100, 100);  // Second control point
        //Point p2 = new Point(100, 100);  // Endpoint

        //Pen pen = new Pen(Color.FromArgb(255, 0, 0, 255));
        //e.Graphics.DrawBezier(pen, p1, c1, c2, p2);


        // pen = new Pen(Color.FromArgb(255, 0, 0, 0));
        //e.Graphics.DrawLine(pen, 20, 10, 300, 100);


        //These seems to work as expected.
        //StockTile test = new StockTile(E_TileLineType.MidToCorner, E_TileWeave.UnderWeave, E_TileRotation.NoRitation, false);
        //StockTile test = new StockTile(E_TileLineType.Cross, E_TileWeave.UnderWeave, E_TileRotation.NoRitation, false);
        ////StockTile test = new StockTile(E_TileLineType.Cross, E_TileWeave.OverWeave, E_TileRotation.NoRitation, false);
        //WorkingTile DrawGlyph = new WorkingTile(test, 0, 0);
        //DrawGlyph.DrawGlyph( e.Graphics);

        //test = new StockTile(E_TileLineType.Cross, E_TileWeave.OverWeave, E_TileRotation.NoRitation, false);
        //DrawGlyph = new WorkingTile(test, 1, 0);
        //DrawGlyph.DrawGlyph(e.Graphics);

        //test = new StockTile(E_TileLineType.DiagonalCross, E_TileWeave.UnderWeave, E_TileRotation.NoRitation, false);
        //DrawGlyph = new WorkingTile(test, 2, 1);
        //DrawGlyph.DrawGlyph(e.Graphics);

        //test = new StockTile(E_TileLineType.DiagonalCross, E_TileWeave.OverWeave, E_TileRotation.NoRitation, false);
        //DrawGlyph = new WorkingTile(test, 3, 2);
        //DrawGlyph.DrawGlyph(e.Graphics);

    }

    private void ButtonDrawAll_Click(object sender, EventArgs e)
    {
        TileGeneratior t = new();

        int Column = 1;
        int Row = 1;
        WorkingTile DrawTile;
        Graphics g = pictureBox1.CreateGraphics();

        string sOutput = SVGHelper.SVGStart();
        sOutput += SVGHelper.SVGHeader();

        foreach (StockTile s in t.WorkingTiles)
        {
            Debug.Print(Enum.GetName(s.TType));


            DrawTile = new WorkingTile(s, Column, Row);
            //DrawTile.DrawGlyph(g);
            //if(DrawTile.ThisTile.TType == E_TileLineType.MidPoints || DrawTile.ThisTile.TType == E_TileLineType.HalfMid)
            //{
            //    Debug.Assert(false);
            //}

            DrawTile.DrawLeftAndRight(g);
            //DrawTile.DrawLeftAndRight(g,false);
            //pictureBox1.Refresh();  
            Column += 1;
            if (Column > 5)
            {
                //Down one and back to the start.
                Column = 1; Row += 1;
            }

            sOutput += DrawTile.ToSVGPathString(E_OutlineType.FullOutline, true, false);

        }

        sOutput += SVGHelper.SVGEnd();

        if (sOutput.Length > 0)
        {
            Clipboard.Clear();
            Clipboard.SetText(sOutput);
        }

        //Debug.Assert(false);
    }

    private void Button2_Click(object sender, EventArgs e)
    {

        TileGeneratior t = new();
        int Column = 1;
        int Row = 1;
        WorkingTile DrawTile;
        //Graphics g = pictureBox1.CreateGraphics();
        string sOutput = SVGHelper.SVGStart();
        sOutput += SVGHelper.SVGHeader();
        foreach (StockTile s in t.WorkingTiles)
        {
            Debug.Print(Enum.GetName(s.TType));

            DrawTile = new WorkingTile(s, Column, Row);
            Column += 1;
            if (Column > 5)
            {
                //Down one and back to the start.
                Column = 1; Row += 1;
            }

            sOutput += DrawTile.ToSVGPathString(E_OutlineType.FullOutline, true, true);

        }
        sOutput += SVGHelper.SVGEnd();
        Clipboard.Clear();
        Clipboard.SetText(sOutput);
        //Debug.Assert(false);
    }

    private void ButtonOverlayed_Click(object sender, EventArgs e)
    {
        TileGeneratior t = new ();
        WorkingTile DrawTile;
        //Graphics g = pictureBox1.CreateGraphics();
        string sOutput = SVGHelper.SVGStart();
        sOutput += SVGHelper.SVGHeader();

        foreach (StockTile s in t.WorkingTiles)
        {
            DrawTile = new WorkingTile(s, 1, 1);
            //DrawTile.DrawGlyph(g);
            sOutput += DrawTile.ToSVGPathString(E_OutlineType.FullOutline, true,false);
        }
        sOutput += SVGHelper.SVGEnd();
        Clipboard.Clear();
        Clipboard.SetText(sOutput);
    }

    private void ButtonTest_Click(object sender, EventArgs e)
    {

        //int x = 10; Func<int> func = () => x * 2; x = 20;
        ////Debug.Print(func().ToString);

        //LinePointObject p = new LinePointObject(E_LinePoint.TopMiddle);
        //Debug.Assert(false);

        //Just get a Cross and draw it.
        //StockTile test = new StockTile(E_TileLineType.HalfMid, E_TileRotation.NoRitation, true, false);
        StockTile test = new(E_TileLineType.MidPoints, E_TileRotation.RotatedNinetyDegrees, true, false);
        //StockTile test = new StockTile(E_TileLineType.MidToCorner, E_TileRotation.NoRitation, true, false);
        // StockTile test = new StockTile(E_TileLineType.Cross, E_TileRotation.NoRitation, false, false);
        //StockTile test = new StockTile(E_TileLineType.MidToCorner, E_TileRotation.NoRitation, false, false);
        //StockTile test = new StockTile(E_TileLineType.Corners, E_TileRotation.NoRitation, true, false);
        ///StockTile test = new StockTile(eTileLineType.MidPoints, eTileRotation.NoRitation, true, false);

        // TODO I am still not 100% happy with the E_TileLineType.MidToCorner The midpoints do not align with the end line.+
        WorkingTile DrawTile = new(test, 0, 0);
        Graphics g = pictureBox1.CreateGraphics();
        //DrawTile.DrawGlyph(g);
        //DrawTile.DrawLeftAndRight(g, false);
        DrawTile.DrawLeftAndRight(g);

        string sOutput = SVGHelper.SVGStart();
        sOutput += SVGHelper.SVGHeader();

        //sOutput += DrawTile.ToSVGPathString(E_OutlineType.Centre, true, true);
        //DrawTile.X += 1;
        //sOutput += DrawTile.ToSVGPathString(E_OutlineType.Left, true, true);
        //DrawTile.X += 1;
        //sOutput += DrawTile.ToSVGPathString(E_OutlineType.Right, true, true);
        DrawTile.X += 1;
        sOutput += DrawTile.ToSVGPathString(E_OutlineType.FullOutline, true, false);
        sOutput += SVGHelper.SVGEnd();
        Clipboard.Clear();
        Clipboard.SetText(sOutput);

    }
}
